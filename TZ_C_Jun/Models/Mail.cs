using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TZ_C_Jun.Models
{
    /// <summary>
    /// Модель описывающая данные для взаимодействия с базой данных
    /// </summary>
    public class Mail
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Recipient { get; set; }
        public DateTime Date { get; set; }
        public string Result { get; set; }
        public string FailedMessage { get; set; }
    }
}
