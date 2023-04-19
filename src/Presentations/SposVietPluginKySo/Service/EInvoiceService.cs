using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SposVietPluginKySo.Service
{
    public class EInvoiceService
    {
        public EInvoiceService() { }
        public string SignHashEInvoiceToken(string hash)
        {
            X509Certificate2Helper x509Certificate2Helper = new X509Certificate2Helper();
            X509Certificate2 certificate = x509Certificate2Helper.GetCertFromStore();
            byte[] hashBytes = Convert.FromBase64String(hash);

            //https://www.c-sharpcorner.com/article/visual-studio-creating-and-managing-digital-certificates-in-c-sharp/
            byte[] signature = x509Certificate2Helper.GetDigitalSignature(hashBytes);
            var sSecret = Convert.ToBase64String(signature);
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode nVersion = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDoc.AppendChild(nVersion);
            XmlNode DLieu = xmlDoc.CreateElement("SendInv");
            xmlDoc.AppendChild(DLieu);

            XmlNode Base64Hash = xmlDoc.CreateElement("Base64Hash");
            Base64Hash.InnerText = hash;
            DLieu.AppendChild(Base64Hash);

            XmlNode SignValue = xmlDoc.CreateElement("SignValue");
            SignValue.InnerText = sSecret;
            DLieu.AppendChild(SignValue);
            return xmlDoc.InnerXml;
        }
    }
}
