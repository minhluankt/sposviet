using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Domain.XmlDataModel
{
    [Serializable()]

    [System.Xml.Serialization.XmlRoot("Item")]
    public class Item
    {
        //[XmlElement(ElementName = "STT")]
        //public string STT { get; set; }
        [XmlElement(ElementName = "index")]
        public string Index { get; set; }

        [XmlElement(ElementName = "cusCode")]
        public string CusCode { get; set; }

        [XmlElement(ElementName = "month")]
        public string Month { get; set; }

        [XmlElement(ElementName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "publishDate")]
        public string PublishDate { get; set; }

        [XmlElement(ElementName = "signStatus")]
        public int? signStatus { get; set; }

        [XmlElement(ElementName = "pattern")]
        public string Pattern { get; set; }

        [XmlElement(ElementName = "serial")]
        public string Serial { get; set; }

        [XmlElement(ElementName = "invNum")]
        public int InvNum { get; set; }

        [XmlElement(ElementName = "amount")]
        public decimal Amount { get; set; }

        [XmlElement(ElementName = "status")]
        public int Status { get; set; }



        [XmlElement(ElementName = "cusname")]
        public string Cusname { get; set; }

        [XmlElement(ElementName = "payment")]
        public int? Payment { get; set; }
    }
    [Serializable()]
    [System.Xml.Serialization.XmlRoot("Data")]
    public class XmlListInvByCusFkey
    {
        [XmlElement(ElementName = "Item")]
        public List<Item> Item { get; set; }
    }

}
