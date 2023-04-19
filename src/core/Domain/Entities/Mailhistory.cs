using AspNetCoreHero.Abstractions.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Mailhistory : AuditableEntity
    {
        public string Title { get; set; }
        public string Subject { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Body { get; set; }

        public DateTime? CreateAgain { get; set; }

        public int IdTypeUserSend { get; set; } // bangr khasch ahfng hoajcw user  laf user 1 la khach hang
        public string IdUserSend { get; set; } // người gửi
        public string FullNameUserSend { get; set; } // người gửi
        public string FileAttach { get; set; }

        public bool Status { get; set; }
        public bool StatusAgain { get; set; }
        public int CountSendAgain { get; set; }
        public bool SendMuti { get; set; }
    }
}
