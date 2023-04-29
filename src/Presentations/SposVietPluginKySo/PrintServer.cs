using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Printing;
using PdfiumViewer;
using ESC_POS_USB_NET.Printer;
using HtmlToImageCore;
using ESC_POS_USB_NET.Enums;
using System.Windows.Forms;
using iTextSharp.text.pdf.qrcode;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Html;
using System.Diagnostics.Metrics;

using GrapeCity.Documents.Html;

using CoreHtmlToImage;
using Aspose.Html;
using Aspose.Html.Converters;
using Aspose.Html.Saving;
using System.Reflection;

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
    public class PrintServer
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
                            // Margins = new Margins(0, 0, 0, 0),
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
            try
            {
                PrinterSettings settings = new PrinterSettings();
                Printer printer = new Printer(settings.PrinterName);



                //------------in hình
                var converter = new HtmlConverter();
                //------------cách 2
                var bytes = converter.FromHtmlString(html);
                //cách 1
                //----------
              
                //---------
                //Font font = new Font("Arial", 18.0f);
                //var size = Measurement.Graphics.MeasureString(html, font);
                Stream stream = new MemoryStream(bytes);
                //Bitmap image = new Bitmap(Bitmap.FromStream(stream));
                Bitmap image = new Bitmap(stream);



                System.Text.EncodingProvider ppp = System.Text.CodePagesEncodingProvider.Instance;
                Encoding.RegisterProvider(ppp);

                printer.Image(image);
                printer.Separator();//dòng kẻ ngang
                printer.FullPaperCut();
                printer.PrintDocument();
            }
            catch (Exception e)
            {
                LogControl.Write(e.ToString());

                throw new Exception("IN thất bại", e);
            }


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
