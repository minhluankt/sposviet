using AspNetCoreHero.Abstractions.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class MailSettings : AuditableEntity
    {
        public string From { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool SSL { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
    }
}
