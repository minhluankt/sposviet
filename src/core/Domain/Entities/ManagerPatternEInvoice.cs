using Application.Enums;
using AspNetCoreHero.Abstractions.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ManagerPatternEInvoice : AuditableEntity
    {
        public ENumSupplierEInvoice TypeSupplierEInvoice { get; set; }// vi dụ VNPT
        public int ComId { get; set; }//
        public int IdSupplierEInvoice { get; set; }// của nhà cung cấp nào
        [StringLength(20)]
        public string Pattern { get; set; }// 
        [StringLength(20)]
        public string Serial { get; set; }// 
        [JsonIgnore]
        public string VFkey { 
            get {
                if (!string.IsNullOrEmpty(Pattern))
                {
                    return $"{TypeSupplierEInvoice}{ComId}{Pattern.Replace("/","")}{Serial.Replace("/", "")}";
                }
                return "";
            }
            set { }
        }// 
        public bool Active { get; set; }// 
        public bool Selected { get; set; }// 
        [JsonIgnore]
        public SupplierEInvoice SupplierEInvoice { get; set; }// 
        [NotMapped]
        public string screct { get; set; }//
    }
}
