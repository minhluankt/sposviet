using System;
using System.Collections.Generic;
using System.Web;
namespace Application.DTOs.Mail
{
    public class MailRequest
    {
        public string Title { get; set; }
        public string[] CC { get; set; }
        public string[] BCC { get; set; }
        public string[] To { get; set; }
        public string EmailTo { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public MimeKit.MimeEntity Body { get; set; }
        public string From { get; set; }
        public int IdTypeUserSend { get; set; } // bangr khasch ahfng hoajcw user  laf user 1 la khach hang
        public string IdUserSend { get; set; }
        public string FullNameUserSend { get; set; }
        public DateTime? CreateAgain { get; set; }
    }
}