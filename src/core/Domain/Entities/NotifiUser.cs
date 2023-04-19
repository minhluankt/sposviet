using AspNetCoreHero.Abstractions.Domain;

namespace Domain.Entities
{
    public class NotifiUser : AuditableEntity
    {
        public int IdUser { get; set; }
        public string? IdUserGuid { get; set; }
        public string? OrderCode { get; set; } // mã đơn h
        public string Title { get; set; }
        public string Content { get; set; }
        public string? Email { get; set; }
        public int Type { get; set; }//loại thông báo mặc định cho user
        public bool IsReview { get; set; } // đã xem
        public bool IsSendEmail { get; set; } // đã xem
    }
}
