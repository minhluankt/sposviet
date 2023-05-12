using Application.Enums;
using Domain.Entities;
using Domain.ViewModel;
using HelperLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Hepers
{
    public static class PrintTemplate
    {
        public static string PrintOrder(TemplateInvoiceParameter templateInvoiceParameter,List<OrderTableItem> orderTableItems, string templateInvoice)
        {
           // templateInvoice= templateInvoice.Replace("\n", "").Replace("\r", "");
            //-----xử lý dòng sản phẩm
            //string trproductregex = @"<tbody>(?<xValue>(.|\n)*)<\/tbody>";//lấy tbody *?\n.*\s(?<a>(.|\n)*){tenhanghoa}
            //string trproductregex = @"<tbody>(.*?{tenhanghoa}.*?)<\/tbody>";//lấy tbody
            string trproductregex = @"<\/thead>(\n|\s)*<tbody>(?<xValue>(.|\n)*)<\/tbody>(\n|\s)*<tfoot>";//lấy tbody
            Regex rg = new Regex(trproductregex);
            var match = rg.Match(templateInvoice); 
            string result = match.Groups["xValue"].Value;
            string tableProduct = string.Empty;
            if (!string.IsNullOrEmpty(result))
            {
                foreach (var item in orderTableItems)
                {
                    tableProduct += result.Replace("{tenhanghoa}", item.Name)
                        .Replace("{donvitinh}", item.Unit)
                        .Replace("{dongia}", item.Price.ToString("N0"))
                        .Replace("{thanhtien}", item.Amount.ToString("N0"))
                        .Replace("{soluong}", item.Quantity.ToString("N0"));
                }
                templateInvoice = templateInvoice.Replace(result, tableProduct);
            }
            //-------end
            //---------tiền thuế
            trproductregex = @"<tr>(\n|\s\.*)*<td.*{thuesuat}(?<xValue>(.|\n)*){tienthue}.*(\n|\s\.*)*<\/tr>";//lấy tr thuế
            rg = new Regex(trproductregex);
            match = rg.Match(templateInvoice);
            result = match.Value;
            string tienthue = string.Empty;
            if (templateInvoiceParameter.isVAT)
            {
                tienthue = result.Replace("{thuesuat}", templateInvoiceParameter.thuesuat).Replace("{tienthue}", templateInvoiceParameter.tienthue);
            }
            templateInvoice = templateInvoice.Replace(result, tienthue);
            //----------end
           
            //-----------mẫu số ký hiệu hóa đơn
            string content = LibraryCommon.GetTemplate(templateInvoiceParameter, templateInvoice, EnumTypeTemplate.INVOICEPOS);
            return System.Net.WebUtility.HtmlDecode(content);
        }
        public static string PrintInvoice(TemplateInvoiceParameter templateInvoiceParameter, List<InvoiceItem> InvoiceItems, string templateInvoice)
        {
          
            // templateInvoice= templateInvoice.Replace("\n", "").Replace("\r", "");
            //-----xử lý dòng sản phẩm
            //string trproductregex = @"<tbody>(?<xValue>(.|\n)*)<\/tbody>";//lấy tbody *?\n.*\s(?<a>(.|\n)*){tenhanghoa}
            //string trproductregex = @"<tbody>(.*?{tenhanghoa}.*?)<\/tbody>";//lấy tbody
            string trproductregex = @"<\/thead>(\n|\s)*<tbody>(?<xValue>(.|\n)*)<\/tbody>(\n|\s)*<tfoot>";//lấy tbody
            Regex rg = new Regex(trproductregex);
            var match = rg.Match(templateInvoice);
            string result = match.Groups["xValue"].Value;
            string tableProduct = string.Empty;
            if (!string.IsNullOrEmpty(result))
            {
                foreach (var item in InvoiceItems)
                {
                    tableProduct += result.Replace("{tenhanghoa}", item.Name)
                        .Replace("{donvitinh}", item.Unit)
                        .Replace("{dongia}", item.Price.ToString("N0"))
                        .Replace("{thanhtien}", item.Amonut.ToString("N0"))
                        .Replace("{soluong}", item.Quantity.ToString("N0"));
                }
                templateInvoice = templateInvoice.Replace(result, tableProduct);
            }
            //-------end
            //---------tiền thuế
            trproductregex = @"<tr>(\n|\s\.*)*<td.*{thuesuat}(?<xValue>(.|\n)*){tienthue}.*(\n|\s\.*)*<\/tr>";//lấy tr thuế
            rg = new Regex(trproductregex);
            match = rg.Match(templateInvoice);
            result = match.Value;
            string tienthue = string.Empty;
            if (templateInvoiceParameter.isVAT)
            {
                if (!string.IsNullOrEmpty(result)) { 
                    tienthue = result.Replace("{thuesuat}", templateInvoiceParameter.thuesuat).Replace("{tienthue}", templateInvoiceParameter.tienthue);
            }
            }
            if (!string.IsNullOrEmpty(result))
            {
                templateInvoice = templateInvoice.Replace(result, tienthue);
            }
            
            //----------end
           
            if (templateInvoiceParameter.TypeTemplatePrint == EnumTypeTemplatePrint.IN_BILL)
            { //-----------mẫu số ký hiệu hóa đơn xóa bỏ đi vì nếu k có
                if (string.IsNullOrEmpty(templateInvoiceParameter.kyhieuhoadon))
                {
                    //đoạn này là xóa đi vì chưa phát hành hóa đơn điện tử dc
                    string regex = @"<.*?{kyhieuhoadon}.*?>";
                    rg = new Regex(regex);
                    match = rg.Match(templateInvoice);
                    result = match.Groups[0].Value;
                    if (!string.IsNullOrEmpty(result))
                    {
                        templateInvoice = templateInvoice.Replace(result, "");
                    }
                    string regexsohoadon = @"<.*?{sohoadon}.*?>";
                    rg = new Regex(regexsohoadon);
                    match = rg.Match(templateInvoice);
                    result = match.Groups[0].Value;
                    if (!string.IsNullOrEmpty(result))
                    {
                        templateInvoice = templateInvoice.Replace(result, "");
                    }
                }
                //-----------mẫu số ký hiệu hóa đơn
                //------------thông tin tra cứu hóa đơn xóa bỏ đi nếu k có
                if (string.IsNullOrEmpty(templateInvoiceParameter.MCQT))
                {
                    string regex = @"<.*?{linktracuu}.*?>";
                    rg = new Regex(regex);
                    match = rg.Match(templateInvoice);
                    result = match.Groups[0].Value;
                    if (!string.IsNullOrEmpty(result))
                    {
                        templateInvoice = templateInvoice.Replace(result, "");
                    }
                }
                //----------end
            }
            

            string content = LibraryCommon.GetTemplate(templateInvoiceParameter, templateInvoice, EnumTypeTemplate.INVOICEPOS);
            return System.Net.WebUtility.HtmlDecode(content);
        }
        public static string PrintBaoBep(TemplateInvoiceParameter templateInvoiceParameter, List<NotifyOrderNewModel> notifylist, string templateInvoice)
        {
            string trproductregex = @"<table.*id=""tablehanghoa""((.|\n)*)<tbody>(?<xValue>(.|\n)*)<\/tbody>";
            string tableProduct = string.Empty;
            Regex rg = new Regex(trproductregex);
            var match = rg.Match(templateInvoice);
            string result = match.Groups["xValue"].Value;
            if (!string.IsNullOrEmpty(result))
            {
                foreach (var item in notifylist)
                {
                    tableProduct += result.Replace("{tenhanghoa}", item.Name).Replace("{donvitinh}", item.Unit).Replace("{dongia}", item.Price.ToString("N0")).Replace("{ghichu}", item.Note).Replace("{soluong}", item.Quantity.ToString("N0"));
                }
            }
            templateInvoice = templateInvoice.Replace(result, tableProduct);
            string content = LibraryCommon.GetTemplate(templateInvoiceParameter, templateInvoice, EnumTypeTemplate.PRINT_BEP);
            return System.Net.WebUtility.HtmlDecode(content);
        }
    }
}
