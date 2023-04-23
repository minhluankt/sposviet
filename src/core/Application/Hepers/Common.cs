using Application.Constants;
using Application.Enums;
using Domain.ViewModel;
using HelperLibrary;
using Newtonsoft.Json;
using Slugify;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Application.Hepers
{
    public class Common
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
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            var kq = JsonConvert.DeserializeObject<int?[]>(json);
            return kq;
        }
        public static int[] ConverJsonToArrIntByNotNull(string jsonint)
        {
            if (string.IsNullOrEmpty(jsonint))
            {
                throw new NotImplementedException();
            }
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
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
       
        public static string ToSlug(string s)
        {
            if (s == null)
            {
                return s;
            }
            String[][] symbols = {
                                 new String[] { "[áàảãạăắằẳẵặâấầẩẫậ]", "a" },
                                 new String[] { "[đ]", "d" },
                                 new String[] { "[éèẻẽẹêếềểễệ]", "e" },
                                 new String[] { "[íìỉĩị]", "i" },
                                 new String[] { "[óòỏõọôốồổỗộơớờởỡợ]", "o" },
                                 new String[] { "[úùủũụưứừửữự]", "u" },
                                 new String[] { "[ýỳỷỹỵ]", "y" },
                                 new String[] { "[+./(),-?]", "" },
                                 new String[] { "[\\s'\";,]", "-" },


                             };
            s = s.ToLower();
            foreach (var ss in symbols)
            {
                s = Regex.Replace(s, ss[0], ss[1]);
            }
            return s.Replace("/", "").Replace(".", "-");
        }
        public static string ToSlugNoDauToupper(string s)
        {
            if (s == null)
            {
                return s;
            }
            String[][] symbols = {
                                 new String[] { "[áàảãạăắằẳẵặâấầẩẫậ]", "a" },
                                 new String[] { "[đ]", "d" },
                                 new String[] { "[éèẻẽẹêếềểễệ]", "e" },
                                 new String[] { "[íìỉĩị]", "i" },
                                 new String[] { "[óòỏõọôốồổỗộơớờởỡợ]", "o" },
                                 new String[] { "[úùủũụưứừửữự]", "u" },
                                 new String[] { "[ýỳỷỹỵ]", "y" },
                                 new String[] { "[\\s'\";,]", " " }
                             };
            s = s.ToLower();
            foreach (var ss in symbols)
            {
                s = Regex.Replace(s, ss[0], ss[1]);
            }
            return s.ToUpper();
        }
        public static IEnumerable<IEnumerable<T>> Split<T>(T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }
        public static IEnumerable<List<T>> SplitList<T>(List<T> locations, int nSize=2)
        {
            for (int i = 0; i < locations.Count; i += nSize)
            {
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
            }
        }
        public static string ToStringNospace(string s)
        {
            String[][] symbols = {
                                 new String[] { "[áàảãạăắằẳẵặâấầẩẫậ]", "a" },
                                 new String[] { "[đ]", "d" },
                                 new String[] { "[éèẻẽẹêếềểễệ]", "e" },
                                 new String[] { "[íìỉĩị]", "i" },
                                 new String[] { "[óòỏõọôốồổỗộơớờởỡợ]", "o" },
                                 new String[] { "[úùủũụưứừửữự]", "u" },
                                 new String[] { "[ýỳỷỹỵ]", "y" },
                                 new String[] { "[\\s'\";,]", "" }
                             };
            s = s.ToLower();
            foreach (var ss in symbols)
            {
                s = Regex.Replace(s, ss[0], ss[1]);
            }
            return s;
        }
        public static string ConvertSlugURLCustom(double from, double to, CommonEnumSpecifications type, string name = "")
        {
            string slug = string.Empty;
            switch (type)
            {
                case CommonEnumSpecifications.DienTich:
                    if (from == 0)
                    {
                        slug = $"dien tich duoi {to}m2";
                    }
                    else
                    {
                        slug = $"dien tich tư {from}m2 den {to}m2";
                    }
                    break;
                case CommonEnumSpecifications.MucGia:
                    if (from == 0)
                    {
                        slug = $"muc gia duoi {LibraryCommon.AmountToWord(to)}";
                    }
                    else
                    {
                        slug = $"muc gia tư {LibraryCommon.AmountToWord(from)} den {LibraryCommon.AmountToWord(to)}";
                    }
                    break;
                case CommonEnumSpecifications.Huong:
                    slug = $"huong {name}";
                    break;
                default:
                    break;
            }
            return ConvertToSlugCore(slug);
        }
        public static string ConvertToSlug(string title)
        {
            //SlugHelper helper = new SlugHelper();
            //return ToSlug(title);
            return ConvertToSlugCore(title);
        }

        public static string ConvertToSlugNoSpage(string title)
        {
            return ConvertToSlugCore(title).Replace("-", "");
        }
        public static string ConvertToSlugCore(string title)
        {
            var config = new SlugHelperConfiguration();

            //// Replace spaces with a dash
            //config.StringReplacements.Add(" ", "-");

            //// We want a lowercase Slug
            //config.ForceLowerCase = true;

            //// Will collapse multiple seqential dashes down to a single one
            //config.CollapseDashes = true;

            //// Will trim leading and trailing whitespace
            //config.TrimWhitespace = true;

            //// Colapse consecutive whitespace chars into one
            //config.CollapseWhiteSpace = true;

            //// Remove everything that's not a letter, number, hyphen, dot, or underscore
            //config.DeniedCharactersRegex = @"[^a-zA-Z0-9\-\._]";

            // Create a helper instance with our new configuration
            SlugHelper helper = new SlugHelper(config);
            return helper.GenerateSlug(title);
        }
        public async static Task<string> GetImageAsBase64Url(string url)
        {
            //var credentials = new NetworkCredential(user, pw);
            using (var handler = new HttpClientHandler())
            using (var client = new HttpClient(handler))
            {
                var bytes = await client.GetByteArrayAsync(url);
                string a = Convert.ToBase64String(bytes);
                return "image/jpeg;base64," + a;
            }
        }
        public static string GenerateBuidString()
        {
            Guid g;
            g = Guid.NewGuid();
            return g.ToString();
        }
        public static Guid NewGuid()
        {
            Guid g;
            g = Guid.NewGuid();
            return g;
        }
        public static List<KeyValuePair<string, int>> GetListStatus()
        {
            SortedList<string, int> fslist = new SortedList<string, int>();
            fslist.Add("AwaitingConfirmation", 0);
            fslist.Add("Processing", 1);
            fslist.Add("Shipping", 2);
            fslist.Add("Delivered", 3);
            fslist.Add("Cancel", 4);
            var orderByVal = fslist.OrderBy(kvp => kvp.Value).ToList();
            return orderByVal;
        }
        public static DateTime? ConvertStringToDateTime(string date, string format = "dd/MM/yyyy")
        {
            if (!string.IsNullOrEmpty(date))
            {
                string cn = "en-US"; //Vietnamese
                var _cultureInfo = new CultureInfo(cn);
                DateTime dt = DateTime.ParseExact(date, format, _cultureInfo, DateTimeStyles.None);
                return dt;
            }
            return null;
        }
       
        //public static string GetTypeIssueImg(string name)
        //{
        //    string png = "";
        //    switch (name)
        //    {
        //        case var s when name.Contains("Change Request"):
        //            png = "cr";
        //            break;
        //        case var s when name.Contains("Service Order"):
        //            png = "od";
        //            break;
        //        case var s when name.Contains("Service Request"):
        //            png = "sr";
        //            break;
        //        case var s when name.Contains("Sub-Task"):
        //            png = "subtask";
        //            break; 
        //        case var s when name.Contains("Internal_Bug"):
        //            png = "Internal_Bug";
        //            break; 
        //        case var s when name.Contains("Issue"):
        //            png = "Issue";
        //            break; 
        //        case var s when name.Contains("Pre sale"):
        //            png = "Presale";
        //            break;
        //        case "Task":
        //            png = "task";
        //            break;   
        //        case "Story":
        //            png = "story";
        //            break; 
        //        case "Bug":
        //            png = "bug";
        //            break;
        //        default:

        //            break;
        //    }
        //    return png;
        //} 
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        
        public static bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
    public static class UserClaimCustom
    {
        public static ClaimsModel GetUserClaimLogin(this IIdentity identity)
        {
            if (identity==null)
            {
                return null;
            }
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim FULLNAME = claimsIdentity?.FindFirst(ClaimUser.FULLNAME);
            Claim COMID = claimsIdentity?.FindFirst(ClaimUser.COMID);
            Claim IDDICHVU = claimsIdentity?.FindFirst(ClaimUser.IDDICHVU);
            Claim IDGUID = claimsIdentity?.FindFirst(ClaimUser.IDGUID);
            Claim UserName = claimsIdentity?.FindFirst(ClaimTypes.Name);
            Claim Id = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            var clame = new ClaimsModel()
            {
                ComId = int.Parse(COMID.Value),
                Id = IDGUID!=null? IDGUID.Value:Id.Value,
                UserName = UserName.Value,
                FullName = FULLNAME.Value,
                IdDichVu = (EnumTypeProduct)int.Parse(IDDICHVU.Value),
            };

            return clame;
        }
    }
}
