using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SposVietPluginKySo.Model
{
    public class XmlInvoice
    {
        public XmlInvoice() { }

    }
    [Serializable()]
    [System.Xml.Serialization.XmlRoot("Invoices")]
    public class XmlInvoices
    {
        [XmlElement(ElementName = "Inv")]
        public List<Inv> Item { get; set; }
    }
    [Serializable()]

    [System.Xml.Serialization.XmlRoot("Inv")]
    public class Inv
    {
        //[XmlElement(ElementName = "STT")]
        //public string STT { get; set; }
        [XmlElement(ElementName = "key")]
        public string key { get; set; }
        [XmlElement(ElementName = "idInv")]
        public string idInv { get; set; }
        [XmlElement(ElementName = "hashValue")]
        public string hashValue { get; set; }
        [XmlElement(ElementName = "pattern")]
        public string pattern { get; set; }
        [XmlElement(ElementName = "serial")]
        public string serial { get; set; }


    }
}
