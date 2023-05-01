using Application.Enums;
using AspNetCoreHero.Abstractions.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Kitchen : AuditableEntity // bar/bếp
    {
        public Kitchen()
        {
            this.IdKitchen = Guid.NewGuid();
        }
        public Guid IdKitchen { get; set; }
        [Required]
        [StringLength(250)]
        public string ProName { get; set; }
        [Required]
        public string ProCode { get; set; }
        [StringLength(250)]
        public string Note { get; set; }
        [StringLength(200)]
        public string Buyer { get; set; }
        public int? IdProduct { get; set; }
        public int ComId { get; set; }
        public EnumTypeProduct IdDichVu { get; set; }
        public Guid IdOrder { get; set; }
        public string IdCashername { get; set; }
        [StringLength(100)]
        public string Cashername { get; set; }
        [Required]
        [StringLength(50)]
        public string OrderCode { get; set; }
        [Required]
        public string RoomTableName { get; set; }
        public Guid? IdRoomTable { get; set; }
        public bool IsBingBack { get; set; }//mang về
        public bool IsCancelAll { get; set; }
        public decimal Quantity { get; set; }
        public DateTime? DateReady { get; set; }
        public DateTime? DateDone { get; set; }
        public EnumStatusKitchenOrder Status { get; set; }
        [JsonIgnore]
        public virtual ICollection<Product> Products { get; set; }
        [JsonIgnore]
        public virtual ICollection<DetailtKitchen> DetailtKitchens { get; set; }
    }
    public class DetailtKitchen//dùng để hủy
    {
        public DetailtKitchen()
        {
            this.Status = EnumStatusKitchenOrder.CANCEL;
        }

        [Required]
        public int Id { get; set; }
        [Required]
        public int IdKitchen { get; set; }
        [Required]
        public decimal Quantity { get; set; }
        public string IdCashername { get; set; }
        [Required]
        public string Cashername { get; set; }
        public string Note { get; set; }
        public EnumTypeKitchenOrder TypeKitchenOrder { get; set; }
        public DateTime? DateCancel { get; set; }
        public bool IsRemove { get; set; }//là đánh dấu là remove k cần hiển thị khi bấm từ bếp dg nấu sang đã nấu chờ cung ứng
        public bool IsSpitOrder { get; set; }//là đánh dấu là remove k cần hiển thị khi bấm từ bếp dg nấu sang đã nấu chờ cung ứng
        public EnumStatusKitchenOrder Status { get; set; }
        [ForeignKey("IdKitchen")]
        public virtual Kitchen Kitchen { get; set; }
    }
}
