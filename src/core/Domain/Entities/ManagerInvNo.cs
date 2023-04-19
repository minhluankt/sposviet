using Application.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class ManagerInvNo
    {
        [Key]
        public int Id { get; set; }
        public int ComId { get; set; }
        public int InvNo { get; set; }
        public string VFkey
        {
            get
            {
                return $"{ComId}PK{InvNo}T{(int)Type}";
            }
            set { }
        }
        public ENumTypeManagerInv Type { get; set; } = ENumTypeManagerInv.Invoice;
    }
}
