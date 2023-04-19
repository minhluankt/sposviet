using Application.Enums;
using AspNetCoreHero.Abstractions.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Area : AuditableEntity // khu vực , ví dụ bàn thuộc khu vực nào
    {
        public Area()
        {
            this.IdGuid = Guid.NewGuid();
            this.RoomAndTables = new List<RoomAndTable>();
        }
        public int ComId { get; set; }//
        public Guid IdGuid { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Note { get; set; }

        [DefaultValue(EnumStatusArea.DANG_HOAT_DONG)]
        public EnumStatusArea Status { get; set; }
        [NotMapped]
        public string IdString { get; set; }

        [NotMapped]
        public bool Active { get; set; }
        [NotMapped]
        public int NumberTable { get; set; }
        [JsonIgnore]
        public List<RoomAndTable> RoomAndTables { get; set; }
    }
}
