using AspNetCoreHero.Abstractions.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class RoomAndTable : AuditableEntity // bàn hoặc phong
    {
        public RoomAndTable()
        {
            this.IdGuid = Guid.NewGuid();
            this.OrderTables = new List<OrderTable>();
            this.Invoices = new List<Invoice>();
        }
        public string Name
        {
            get;
            set;
        }
        public Guid IdGuid { get; set; }
        public string Slug { get; set; }
        public short STT { get; set; }
        public int? IdArea { get; set; }
        public int NumberSeats { get; set; }//sô ghế
        public string Note { get; set; }//
        public bool Active { get; set; }// ẩn hiện bàn đặt
        public bool IsUse { get; set; }// đang được sử dụng, tức là đang bán
        public int ComId { get; set; }//

        [NotMapped]
        public string IdString
        {
            get; set;
        }//
        [NotMapped]
        public string AreaName
        {
            get; set;
        }//
        [NotMapped]
        public string NameSelect
        {
            get;
            set;
        }// tên trong đoạn chọn tạo nhiều bàn
        [NotMapped]
        public bool IsCreateMuti { get; set; }// tạo nhiều bàn
        [NotMapped]
        public int Fromno { get; set; }// tạo nhiều bàn
        [NotMapped]
        public int Tono { get; set; }// tạo nhiều bàn
        [JsonIgnore]
        public virtual ICollection<OrderTable> OrderTables { get; set; } // trạng thái
        [JsonIgnore]
        public virtual ICollection<Invoice> Invoices { get; set; } // trạng thái
        [JsonIgnore]
        [ForeignKey("IdArea")]
        public virtual Area Area { get; set; }

    }
}
