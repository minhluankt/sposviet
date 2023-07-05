using Newtonsoft.Json;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Library
{
    public class ConvertSupport
    {
        public static T ConvertXMLToModel<T>(string item)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(T));
            var data = (T)deserializer.Deserialize(new StringReader(item));
            return data;
        }
        public static T ConverJsonToModel<T>(object json, JsonSerializerSettings _options = null)
        {
            var kq = JsonConvert.DeserializeObject<T>(json.ToString(), _options);
            return kq;
        }
        public static int?[] ConverJsonToArrInt(string json)
        {
            var kq = JsonConvert.DeserializeObject<int?[]>(json);
            return kq;
        }
        public static int[] ConverJsonToArrInt2(string jsonint)
        {
            var kq = JsonConvert.DeserializeObject<int[]>(jsonint);
            return kq;
        }
        public static string ConverObjectToJsonString(object json)
        {
            var kq = JsonConvert.SerializeObject(json);
            return kq;
        }
        public static string ConverStringToQrcode(string data,int pixelsPerModule =20, Bitmap logo = null, int iconSizePercent = 15, int iconBorderWidth = 6)
        {
            //https://github.com/codebude/QRCoder/wiki/Advanced-usage---QR-Code-renderers#25-pngbyteqrcode-renderer-in-detail
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            //QRCode qrCode = new QRCode(qrCodeData);
            // Bitmap qrCodeImage = qrCode.GetGraphic(20);
            Base64QRCode qrCode = new Base64QRCode(qrCodeData);
            var imgType = Base64QRCode.ImageType.Png;
            if (logo!=null)
            {
                var qrCodeImageAsBase64inlofo = qrCode.GetGraphic(pixelsPerModule, Color.Black, Color.White,logo, iconSizePercent, iconBorderWidth, false, imgType);
                return $"data:image/{imgType.ToString().ToLower()};base64,{qrCodeImageAsBase64inlofo}";
            }
            var qrCodeImageAsBase64 = qrCode.GetGraphic(pixelsPerModule, Color.Black, Color.White, false, imgType);
            return $"data:image/{imgType.ToString().ToLower()};base64,{qrCodeImageAsBase64}";
           
        }
        public static string ConverModelToJson<T>(T json, JsonSerializerSettings _options = null)
        {
            var kq = JsonConvert.SerializeObject(json, _options);
            return kq;
        } 
        public static string ConverDoaminVNPTPortal<T>(string json)
        {
            var kq = json.Replace("admindemo.vnpt-invoice.com.vn", ".vnpt-invoice.com.vn").Replace("admin.vnpt-invoice.com.vn", ".vnpt-invoice.com.vn");
            return kq;
        }

    }
}
