using AspNetCoreHero.Abstractions.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class City : AuditableEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Prefix { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<Customer> Customers { get; set; }
        public ICollection<District> Districts { get; set; }
        public ICollection<Ward> Wards { get; set; }
    }
    public class District : AuditableEntity // Quận
    {
        [ForeignKey("idCity")]
        public int idCity { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Prefix { get; set; }//là quận hay huyện

        public City City { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<Customer> Customers { get; set; }
        public ICollection<Ward> Wards { get; set; }
    }
    public class Ward : AuditableEntity
    {
        public int idDistrict { get; set; }
        public int idCity { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Prefix { get; set; }
        public District District { get; set; }
        public City City { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<Customer> Customers { get; set; }
    }

}
