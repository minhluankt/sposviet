using AspNetCoreHero.Abstractions.Domain;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Consultation : AuditableEntity// đăng ký tư vấn
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Code { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }

    }
}
