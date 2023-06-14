using SposVietPluginKySo.Helper;
using SposVietPluginKySo.Model;
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
        public string SignHashEInvoiceToken(string hash)//cái này gửi thuế thôi cho mtt
        {
            X509Certificate2Helper x509Certificate2Helper = new X509Certificate2Helper();
            X509Certificate2 certificate = x509Certificate2Helper.GetCertFromStore();
            if (certificate == null)
            {
                return "-1";//người dùng hủy bỏ
            }
            byte[] hashBytes = Convert.FromBase64String(hash);

            //https://www.c-sharpcorner.com/article/visual-studio-creating-and-managing-digital-certificates-in-c-sharp/
            byte[] signature = x509Certificate2Helper.GetDigitalSignature(hashBytes);
            if (signature == null)
            {
                return "-2";//ký thất bại
            }
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
        public string SignHashPublishEInvoiceToken(string xmldata, string serialCert)//ký hóa đơn = tonen
        {
            X509Certificate2Helper x509Certificate2Helper = new X509Certificate2Helper();
            X509Certificate2 certificate = x509Certificate2Helper.GetCertFromStore();
            if (certificate == null)
            {
                return "-1";//người dùng hủy bỏ
            }
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode nVersion = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDoc.AppendChild(nVersion);
            XmlNode DLieu = xmlDoc.CreateElement("Invoices");
            xmlDoc.AppendChild(DLieu);

            XmlNode XmlNodeSerialCert = xmlDoc.CreateElement("SerialCert");
            XmlNodeSerialCert.InnerText = serialCert;
            DLieu.AppendChild(XmlNodeSerialCert);


            var model = SupportCommont.ConvertXMLToModel<XmlInvoices>(xmldata);
            foreach (var item in model.Item)
            {
                byte[] hashBytes = Convert.FromBase64String(item.hashValue);
                byte[] signature = x509Certificate2Helper.GetDigitalSignature(hashBytes);
                if (signature == null)
                {
                    return "-2";//ký thất bại
                }
                var sSecret = Convert.ToBase64String(signature);

                XmlNode XmlInv = xmlDoc.CreateElement("Inv");
                DLieu.AppendChild(XmlInv);

                XmlNode Xmlkey = xmlDoc.CreateElement("key");
                Xmlkey.InnerText = item.key;
                XmlInv.AppendChild(Xmlkey);

                XmlNode XmlidInv = xmlDoc.CreateElement("idInv");
                XmlidInv.InnerText = item.idInv;
                XmlInv.AppendChild(XmlidInv);

                XmlNode XmlsignValue = xmlDoc.CreateElement("signValue");
                XmlsignValue.InnerText = sSecret;
                XmlInv.AppendChild(XmlsignValue);

            }
            return xmlDoc.InnerXml;
        }
    }
}
