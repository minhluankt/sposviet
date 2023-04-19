using AspNetCoreHero.Abstractions.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class NotificationNewsEmail : AuditableEntity
    {
        public string Email { get; set; }
    }
}
