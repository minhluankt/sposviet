using AspNetCoreHero.Abstractions.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ConfigSystem : AuditableEntity
    {
        public ConfigSystem()
        {
            this.ConfigSystems = new List<ConfigSystem>();
        }
        public string Name { get; set; }//tên mô tả
        public int ComId { get; set; }
        public string Parent { get; set; }//các chi tiết con thuộc chi tiết cha
        public string TypeValue { get; set; }//kiểu dữ liệu
        public string Type { get; set; }//loại là giao dịch hay hàng hóa, hay hóa đơn,,,,
        public string Key { get; set; }
        public string Value { get; set; }
        public string Note { get; set; }
        [NotMapped]
        public List<ConfigSystem> ConfigSystems { get; set; }
    }
}
