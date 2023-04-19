using Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Domain.XmlDataModel
{
    public class HDon
    {
        public string KHMSHDon { get; set; }
        public string KHHDon { get; set; }
        public string SHDon { get; set; }
        public string DLKy { get; set; }
        public string MCCQThue { get; set; }
        public string MTLoi { get; set; }
        public string Fkey { get; set; }
        public int TThai { get; set; }
        public ENumTypeStausSendCQT Status {
            get {
                return (ENumTypeStausSendCQT)TThai;
            }
        }
    }
 
    [Serializable()]
    [System.Xml.Serialization.XmlRoot("DSHDon")]
    public class DSHDon
    {
        [XmlElement(ElementName = "HDon")]
        public List<HDon> HDons { get; set; }
    }
}
