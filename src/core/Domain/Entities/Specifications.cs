using AspNetCoreHero.Abstractions.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    // thông số kỹ thuật
    public class Specifications : AuditableEntity
    {
        public int idTypeSpecifications { get; set; }
        public int Sort { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public double From { get; set; }
        public double To { get; set; }
        public string Slug { get; set; }

        public ICollection<StyleProduct> StyleProducts { get; set; }
        public ICollection<Product> JuridicalICs { get; set; }
        public ICollection<Product> DirectionICs { get; set; }
        public ICollection<Product> LandareaICs { get; set; }
        public ICollection<Product> PriceICs { get; set; }
        [ForeignKey("idTypeSpecifications")]
        public TypeSpecifications TypeSpecifications { get; set; }
    }
}