using Application.Enums;
using AspNetCoreHero.Abstractions.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CategoryCevenue : AuditableEntity
    {
        public EnumTypeRevenueExpenditure Type { get; set; }
        public int ComId { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(20)]
        public string Code { get; set; }
        [StringLength(200)]
        public string Slug { get; set; }
        public string Content { get; set; }
        [NotMapped]
        public string IdString { get; set; }//
        [NotMapped]
        public string CasherName { get; set; }//
        public virtual ICollection<RevenueExpenditure> RevenueExpenditures { get; set; }
    }
}
