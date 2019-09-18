using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TZ_C_Jun.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;

namespace TZ_C_Jun.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailsController : ControllerBase
    {
        private MailsContext db = new MailsContext();

        private IConfiguration Configuration { get; set; }

        private string smtp;
        private string from;
        private string name;
        private string pass;

        public MailsController(MailsContext context)
        {
            this.db = context;
        }

        /// <summary>
        /// Отвечает на Get запросы, списком сообщений логированных в бд, в формате json
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<Mail> Get()
        {
            return db.Mails.ToList();
        }

        /// <summary>
        /// Принимает Post запросы в формате json из которых извлекает тему, тело и адресатов
        /// для формирования и отправки сообщений, по результатам которых результаты
        /// заносяться в базу данных
        /// </summary>
        /// <param name="json"></param>
        [HttpPost]
        public void Post([FromBody] string json)
        {
            if(ValidationJson(json))
            {
                ReqJson rj = JsonConvert.DeserializeObject<ReqJson>(json);

                SendLatters(rj);
            }
        }

        private void SendLatters(ReqJson rj)
        {
            SettingSmtp();

            using (MailMessage mm = new MailMessage())
            {
                string recipients = null;

                Mail mail = new Mail();
                string result = "OK";

                try
                {
                    for (int i = 0; i < rj.recipients.Length; i++)
                    {
                        mm.CC.Add(rj.recipients[i]);

                        recipients += string.Format($"{rj.recipients[i]}, ");
                    }

                    mm.From = new MailAddress($"{from}", $"{name}");
                    mm.Subject = rj.subject;
                    mm.Body = rj.body;

                    using (SmtpClient sc = new SmtpClient(smtp))
                    {
                        sc.EnableSsl = true;
                        sc.DeliveryMethod = SmtpDeliveryMethod.Network;
                        sc.UseDefaultCredentials = false;
                        sc.Credentials = new NetworkCredential(from, pass);

                        try
                        {
                            sc.SendAsync(mm, null);
                        }
                        catch (ObjectDisposedException ex)
                        {
                            mail.FailedMessage = ex.Message;
                            result = "Failed";
                        }
                        catch (SmtpException ex)
                        {
                            mail.FailedMessage = ex.Message;
                            result = "Failed";
                        }
                        catch (InvalidOperationException ex)
                        {
                            mail.FailedMessage = ex.Message;
                            result = "Failed";
                        }
                    }
                }
                catch (ArgumentNullException ex)
                {
                    mail.FailedMessage = ex.Message;
                    result = "Failed";
                }
                catch (ArgumentException ex)
                {
                    mail.FailedMessage = ex.Message;
                    result = "Failed";
                }
                catch (FormatException ex)
                {
                    mail.FailedMessage = ex.Message;
                    result = "Failed";
                }
                catch (Exception ex)
                {
                    mail.FailedMessage = ex.Message;
                    result = "Failed";
                }
                finally
                {
                    mail.Recipient = recipients;
                    mail.Subject = rj.subject;
                    mail.Body = rj.body;
                    mail.Date = DateTime.Now;
                    mail.Result = result;

                    db.Mails.Add(mail);
                    db.SaveChanges();
                }
            }
        }

        private void SettingSmtp()
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

            configurationBuilder.AddJsonFile("appsettings.json");
            Configuration = configurationBuilder.Build();

            smtp = Configuration["SmtpServer:smtp"];
            from = Configuration["SmtpServer:from"];
            pass = Configuration["SmtpServer:pass"];
            name = Configuration["SmtpServer:name"];
        }

        private bool ValidationJson(string json)
        {
            JSchema schema = JSchema.Parse(@"{
                            'type': 'object',
                            'properties': {
                                'subject': {
                                    'type':'string', 
                                    'description': 'subject'
                                    },
                                'body': {
                                    'type':'string', 
                                    'description': 'body'
                                    },
                                'recipients': {
                                    'type':'array', 
                                    'description': 'recipients',
                                    'items': {
                                        'type':'string',
                                        'format': 'email'
                                        }
                                    }
                                },
                            'required': [ 'subject', 'body', 'recipients' ],
                            'additionalProperties': false
                            }");
            
            JObject jsonO = JObject.Parse(json);

            return jsonO.IsValid(schema);
        }
    }
}
