using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace TZ_C_Jun.Models
{
    /// <summary>
    /// Контекст данных для взаимодействия с базой данных в EF Core
    /// </summary>
    public class MailsContext : DbContext
    {

        public DbSet<Mail> Mails { get; set; }
        public MailsContext(DbContextOptions<MailsContext> options) 
            : base(options)
        { }
        public MailsContext()
        { }
    }
}
