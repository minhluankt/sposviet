using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Printing;
using PdfiumViewer;
using ESC_POS_USB_NET.Printer;
using CoreHtmlToImage;
using ESC_POS_USB_NET.Enums;
using System.Windows.Forms;
using iTextSharp.text.pdf.qrcode;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Html;
using System.Diagnostics.Metrics;

namespace SposVietPluginKySo
{
    public static class Measurement
    {
        private static Bitmap measurementBitmap;
        private static Graphics measurementGraphics;

        static Measurement()
        {
            measurementBitmap = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
            measurementGraphics = Graphics.FromImage(measurementBitmap);
        }

        public static Graphics Graphics
        {
            get
            {
                return measurementGraphics;
            }
        }
    }
    public  class PrintServer
    {

        public static void Print(string data)
        {
            PrinterSettings settings = new PrinterSettings();
            //Console.WriteLine(settings.PrinterName);
           string defaultPrinter = settings.PrinterName;
            NReco.PdfGenerator.HtmlToPdfConverter htmlToPdfConverter = new NReco.PdfGenerator.HtmlToPdfConverter();
            var bytes = htmlToPdfConverter.GeneratePdf(data);

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                using (var document = PdfDocument.Load(ms))
                {
                    // cài NuGet\Install-Package PdfiumViewer.Native.x86_64.v8-xfa -Version 2018.4.8.256
                    // và NuGet\Install-Package PdfiumViewer.Native.x86.v8-xfa -Version 2018.4.8.256
                    using (var printDocument = document.CreatePrintDocument())
                    {
                        //var path = Path.GetDirectoryName(Application.ExecutablePath);
                        //settings.PrintFileName = Path.Combine(path, "file" + count++ + ".pdf");
                        //settings.PrintToFile = true;
                        printDocument.PrinterSettings = settings;
                        //printDocument.DocumentName = Path.Combine(path, "file" + count++ + ".pdf");
                        printDocument.DefaultPageSettings = new PageSettings(printDocument.PrinterSettings)
                        {
                            Margins = new Margins(0, 0, 0, 0),
                            PaperSize = GetPaperSize(settings, (int)PaperKind.A6),
                            //PaperSize = new PaperSize("Custom",250,750),
                            
                        };
                        //printDocument.DefaultPageSettings.PaperSize.RawKind = (int)PaperKind.Custom;
                        //printDocument.PrinterSettings.DefaultPageSettings.PaperSize.RawKind = (int)PaperKind.Custom;
                        //printDocument.DefaultPageSettings.Landscape = false;

                        printDocument.PrintController = new StandardPrintController();
                        printDocument.Print();
                    }
                }
            }

        }
        public static void PrintReceiptForTransaction()
        {
            PrinterSettings settings = new PrinterSettings();
            PrintDocument recordDoc = new PrintDocument();

            //recordDoc.PrintPage += new PrintPageEventHandler(PrintPageEventHandler); // function below    
            //recordDoc.PrintController = new StandardPrintController(); // hides status dialog popup    
                                                                       // Comment if debugging     
            PrinterSettings ps = new PrinterSettings();
            ps.PrinterName = settings.PrinterName; 
            recordDoc.PrinterSettings = ps;
            recordDoc.Print();
            // --------------------------------------    
            // Uncomment if debugging - shows dialog instead    
            //PrintPreviewDialog printPrvDlg = new PrintPreviewDialog();    
            //printPrvDlg.Document = recordDoc;    
            //printPrvDlg.Width = 1200;    
            //printPrvDlg.Height = 800;    
            //printPrvDlg.ShowDialog();    
            // --------------------------------------    
            recordDoc.Dispose();

        }

        public static void PrintPageBaoBep(string html)
        {
            PrinterSettings settings = new PrinterSettings();
            Printer printer = new Printer(settings.PrinterName);


            //------------in hình
            var converter = new HtmlConverter();
            //var html = "<!DOCTYPE html>\r\n<html lang='vi'>\r\n<head>\r\n    <meta charset='UTF-8'>\r\n    <meta name='viewport' content='width=device-width, initial-scale=1.0'>\r\n    <meta http-equiv='X-UA-Compatible' content='ie=edge'>\r\n    <title>Vé điện tử</title>\r\n    <script type=\"text/javascript\" charset=\"UTF-8\"></script>\r\n\t\r\n\t<style>\r\n        body {\r\n           \r\n            font-family: Arial;\r\n        }\r\n\r\n        hr {\r\n            margin: 0px;\r\n            border-top: 1px solit #000;\r\n        }\r\n\r\n        .ticket {\r\n          \r\n            padding: 0mm;\r\n            margin: 0 auto;\r\n            height: auto;\r\n   width: 300mm;\r\n            background: #FFF;\r\n            transform-origin: left;\r\n        }\r\ntable { \r\n    border-collapse: collapse; \r\n}\r\ntable td,table th{\r\npadding:2px 2px 2px 0px;\r\n}\r\n        img {\r\n            max-width: inherit;\r\n            width: inherit;\r\n        }\r\n\r\n        @media print {\r\n\r\n            .hidden-print,\r\n            .hidden-print * {\r\n                display: none !important;\r\n            }\r\n\r\n            .ticket {\r\n                page-break-after: always;\r\n            }\r\n        }\r\n    </style>\r\n</head>\r\n\r\n<body>\r\n    <div class='ticket'>\r\n\t\r\n        <table style='width:100%;'>\r\n            <tr>\r\n                <td style='text-align: center;'>\r\n\t\t\t\t\t<span style='font-weight: bold;font-size: 50pt;'>{comname}</span>\r\n\t\t\t\t\t<span style='font-size: 40pt; display: block; text-align: center;margin-bottom:10px'>----------***----------</span>\r\n\t\t\t\t\t\r\n\t\t\t\t</td>\r\n            </tr>\r\n            <tr>\r\n                <td style='font-size: 18px; text-align: center; padding-top: 7px; padding-bottom: 7px;'>\r\n\t\t\t\t\t<span style='display: block; font-size: 45pt; font-weight: bold;'>THÔNG BÁO CHẾ BIẾN</span>\r\n\t\t\t\t\t<span style='font-size: 40pt; display: block;'>Thời gian: {ngaythangnamxuat}</span>\r\n                </td>\r\n            </tr>\r\n        </table>\r\n\t\r\n        \r\n\t\t<hr style=\"font-size:40pt\" />\r\n\t<table style='width:100%;margin-top:10px;margin-bottom:10px'>\t\t\t\r\n\t    <thead>\r\n\t\t<tr  style=\"border-botom-style: dotted;border-width: 1pt\">\r\n\t\t<th style='font-size: 35pt; text-align: left;    PADDING-BOTTOM: 12PT;'>Tên hàng hóa</th>\r\n\t\t<th style='font-size: 35pt; text-align: right;    PADDING-BOTTOM: 12PT;'>Số lượng</th>\r\n\t\t</tr>\r\n\t\t</thead>\r\n\t\t<tbody>\r\n\t\t<tr><td style=\" padding-top: 10pt;padding-bottom: 10pt;\"colspan=\"2\">\r\n\t\t\t<span style='border-top: dotted #000 4px;display: block;'></span>\r\n\t\t</td>\r\n\t\t</tr>\r\n\t\t\t<tr  style=\"border-botom-style: dotted;border-width: 1pt\">\r\n\t\t\t\t<td style='font-size: 40pt; text-align: left'>Cafe</td>\r\n\t\t\t\t<td style='font-size: 40pt; text-align: right'>10</td>\r\n\t\t\t</tr>\r\n\t<tr  style=\"border-botom-style: dotted;border-width: 1pt\">\r\n\t\t\t\t<td style='font-size: 40pt; text-align: left'>Cafe</td>\r\n\t\t\t\t<td style='font-size: 40pt; text-align: right'>10</td>\r\n\t\t\t</tr>\r\n\t<tr  style=\"border-botom-style: dotted;border-width: 1pt\">\r\n\t\t\t\t<td style='font-size: 40pt; text-align: left'>Cafe</td>\r\n\t\t\t\t<td style='font-size: 40pt; text-align: right'>10</td>\r\n\t\t\t</tr>\r\n\t\r\n\t\t</tbody>\r\n\t\t<tfoot>\r\n\t\t<tr><td style=\" padding-top: 10pt;padding-bottom: 10pt;\"colspan=\"2\">\r\n\t\t\t<span style='border-top: dotted #000 4px;display: block;'></span>\r\n\t\t</td></tr>\r\n\t\t<tr style='font-size: 35pt;text-align: left;margin-top:4px;border-top-style: dotted;border-width: 0.1px;'>\r\n\t\t\t<td style='font-size: 50pt; text-align: left'>Tổng</td>\r\n\t\t\t<td style=\"text-align: right;font-size: 50pt;\">8</td>\r\n\t\t</tr>\r\n\t\t\r\n\t\t</tfoot>\r\n\t</table>\r\n\t\r\n</body>\r\n</html>";
           
            var bytes = converter.FromHtmlString(html);
            //Font font = new Font("Arial", 18.0f);
            //var size = Measurement.Graphics.MeasureString(html, font);
            Stream stream = new MemoryStream(bytes);
            //Bitmap image = new Bitmap(Bitmap.FromStream(stream));
            Bitmap image = new Bitmap(stream);
            //----------

            System.Text.EncodingProvider ppp = System.Text.CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(ppp);

            printer.Image(image);
            printer.Separator();//dòng kẻ ngang
            printer.FullPaperCut();
            printer.PrintDocument();

        }
        public static void PrintHmtl()
        {
            PrinterSettings settings = new PrinterSettings();
            var converter = new HtmlConverter();
            var html = "<div><strong>Hello</strong> World!</div>";
            var bytes = converter.FromHtmlString(html);
            Printer printer = new Printer(settings.PrinterName);
            //Stream stream = new MemoryStream(bytes);
           // Bitmap image = new Bitmap(Bitmap.FromStream(stream));
           // Bitmap newImage = ResizeBitmap(image, 512, 512);
            //   printer.Image(newImage);
            // printer.PrintDocument(
            //  printer.FullPaperCut();
            //        printer.PrintDocument();

            System.Text.EncodingProvider ppp = System.Text.CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(ppp);
            printer.Append("NORMAL - 48 COLUMNS");
            printer.Append("1...5...10...15...20...25...30...35...40...45.48");
            printer.Separator();
            printer.Append("Text Normal");
            printer.BoldMode("Bold Text");
            printer.UnderlineMode("Underlined text");
            printer.Separator();
            printer.ExpandedMode(PrinterModeState.On);
            printer.Append("Expanded - 23 COLUMNS");
            printer.Append("1...5...10...15...20..23");
            printer.ExpandedMode(PrinterModeState.Off);
            printer.Separator();
            printer.CondensedMode(PrinterModeState.On);
            printer.Append("Condensed - 64 COLUMNS");
            printer.Append("1...5...10...15...20...25...30...35...40...45...50...55...60..64");
            printer.CondensedMode(PrinterModeState.Off);
            printer.Separator();
            printer.DoubleWidth2();
            printer.Append("Font Width 2");
            printer.DoubleWidth3();
            printer.Append("Font Width 3");
            printer.NormalWidth();
            printer.Append("Normal width");
            printer.Separator();
            printer.AlignRight();
            printer.Append("Right aligned text");
            printer.AlignCenter();
            printer.Append("Center-aligned text");
            printer.AlignLeft();
            printer.Append("Left aligned text");
            printer.Separator();
            printer.Font("Font A", Fonts.FontA);
            printer.Font("Font B", Fonts.FontB);
            printer.Font("Font C", Fonts.FontC);
            printer.Font("Font D", Fonts.FontD);
            printer.Font("Font E", Fonts.FontE);
            printer.Font("Font Special A", Fonts.SpecialFontA);
            printer.Font("Font Special B", Fonts.SpecialFontB);
            printer.Separator();
            printer.InitializePrint();
            printer.SetLineHeight(24);
            printer.Append("This is first line with line height of 30 dots");
            printer.SetLineHeight(40);
            printer.Append("This is second line with line height of 24 dots");
            printer.Append("This is third line with line height of 40 dots");
            printer.NewLines(3);
            printer.Append("End of Test :)");
            printer.Separator();
            printer.FullPaperCut();
            printer.PrintDocument();

        }
        private static PaperSize GetPaperSize(PrinterSettings ps, int rawKind)
        {
            PaperSize papersize = null;
            foreach (PaperSize item in ps.PaperSizes)
            {
                if (item.RawKind == rawKind)
                {
                    papersize = item;
                    break;
                }
            }
            return papersize;
        }
        private static Bitmap ResizeBitmap(Bitmap bmp, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bmp, 0, 0, width, height);
            }

            return result;
        }
    }
}
