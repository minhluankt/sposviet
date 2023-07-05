using Application.Enums;
using AspNetCoreHero.Abstractions.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class TemplateInvoice : AuditableEntity //mauaxx giấy in
    {
        public TemplateInvoice() { }
        public EnumTypeTemplatePrint TypeTemplatePrint { get; set; }//loại mẫu in
        public int ComId { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(200)]
        public string Slug { get; set; }
        public string Template { get; set; }
        public bool Active { get; set; }
       
        public bool IsShowQrCodeVietQR { get; set; }//cái này dùng để lưu cho tempalte mẫu có dùng hay k
        public string HtmlQrCodeVietQR { get; set; }
        public string Note { get; set; }
        [NotMapped]
        public bool IsRegisterQrCodeVietQR { get; set; }//cái này dùng để gọi lên hệ thống là hệ thống có đăng ký qrcode chưa
        [NotMapped]
        public List<SelectListItem> Selectlist { get; set; }
    }
}
