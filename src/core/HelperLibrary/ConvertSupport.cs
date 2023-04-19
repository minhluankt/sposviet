using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public static T ConverJsonToModel<T>(string json, JsonSerializerSettings _options = null)
        {
            var kq = JsonConvert.DeserializeObject<T>(json, _options);
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
