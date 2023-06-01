using Application.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace HelperLibrary
{
    public static class KeyGenerator
    {
        internal static readonly char[] chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

        public static string GetUniqueKey(int size)
        {
            byte[] data = new byte[4 * size];
            using (var crypto = RandomNumberGenerator.Create())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(size);
            for (int i = 0; i < size; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;

                result.Append(chars[idx]);
            }

            return result.ToString().ToUpper();
        }

        public static string GetUniqueKeyOriginal_BIASED(int size)
        {
            char[] chars =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[size];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }
    }
    public static class LibraryCommon
    {
        private static Random random = new Random();
        public static int GetInt(char ss)
        {
            int a = 0;
            a = Convert.ToInt32(ss) - 48;
            return a;
        }
        public static CultureInfo GetIFormatProvider()
        {
            string cn = "en-US"; //Vietnamese
            return new CultureInfo(cn);
        }
        public static string GetNameInvoice(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return "";
            }
            string nameinvoice = "HÓA ĐƠN GIÁ TRỊ GIA TĂNG";
           
            var invname = int.Parse(pattern.Split('/')[0]);
            switch (invname)
            {
                case 1:
                    nameinvoice = "HÓA ĐƠN GIÁ TRỊ GIA TĂNG";
                    break;
                case 2:
                    nameinvoice = "HÓA ĐƠN BÁN HÀNG";
                    break;
                default:
                    break;
            }
            return nameinvoice;
        }

        public static bool CheckMST(string mST)
        {
            bool istrue = false;
            if (mST.Length != 14 && mST.Length != 10) return false;
            if (mST.Length == 14 && !mST[10].Equals('-')) return false;
            int index = mST.Length;
            if (!char.IsNumber(mST[index - 1]) || !char.IsNumber(mST[index - 2]) || !char.IsNumber(mST[index - 3])) return false;
            int value = GetInt(mST[0]) * 31 + GetInt(mST[1]) * 29 + GetInt(mST[2]) * 23 + GetInt(mST[3]) * 19 + GetInt(mST[4]) * 17 + GetInt(mST[5]) * 13 + GetInt(mST[6]) * 7 + GetInt(mST[7]) * 5 + GetInt(mST[8]) * 3;
            int mod = 10 - value % 11;
            if (Math.Abs(mod) == GetInt(mST[9]))
                istrue = true;
            return istrue;
        }
       
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static DateTime? ConvertStringToDateTime(string date, string format = "dd/MM/yyyy")
        {
            if (!string.IsNullOrEmpty(date))
            {
                DateTime dt = DateTime.ParseExact(date.Trim(), format, CultureInfo.InvariantCulture);
                return dt;
            }
            return null;
        }

        public static T CloneJson<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source));
        }
        public static T DeepClone<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
        public static string GetDisplayNameEnum(this Enum enumValue)
          
        {
            return enumValue.GetType()?
                      .GetMember(enumValue.ToString())?
                      .First()?
                      .GetCustomAttribute<DisplayAttribute>()?
                      .GetName();
        }
        public static String GetEnumMemberValue<T>(T value)
        where T : struct, IConvertible
        {
            return typeof(T)
                .GetTypeInfo()
                .DeclaredMembers
                .SingleOrDefault(x => x.Name == value.ToString())
                ?.GetCustomAttribute<EnumMemberAttribute>(false)
                ?.Value;
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
        public static string GetDateInWeek(int? addDate)
        {
            DateTime dateValue = DateTime.Now;
            if (addDate != null && addDate > 0)
            {
                dateValue = dateValue.AddDays(addDate.Value);
            }
            var date = (int)dateValue.DayOfWeek;

            switch (date)
            {
                case 0:
                    return "Chủ nhật";
                case 1:
                    return "Thứ hai";
                case 2:
                    return "Thứ ba";
                case 3:
                    return "Thứ tư";
                case 4:
                    return "Thứ năm";
                case 5:
                    return "Thứ sáu";
                case 6:
                    return "Thứ bảy";
                default:
                    break;
            }
            return date.ToString();
        }
        public static string DateInWeek(DateTime dateValue)
        {
            var date = (int)dateValue.DayOfWeek;
            switch (date)
            {
                case 0:
                    return "Chủ nhật";
                case 1:
                    return "Thứ hai";
                case 2:
                    return "Thứ ba";
                case 3:
                    return "Thứ tư";
                case 4:
                    return "Thứ năm";
                case 5:
                    return "Thứ sáu";
                case 6:
                    return "Thứ bảy";
                default:
                    break;
            }
            return date.ToString();
        }

        public static Guid NewGuid()
        {
            Guid g;
            g = Guid.NewGuid();
            return g;
        }
        public static string TimeAgo(DateTime dt)
        {
            TimeSpan span = DateTime.Now - dt;
            if (span.Days > 365)
            {
                int years = (span.Days / 365);
                if (span.Days % 365 != 0)
                    years += 1;
                return String.Format("{0} {1} trước",
                years, years == 1 ? "năm" : "năm");
            }
            if (span.Days > 30)
            {
                int months = (span.Days / 30);
                if (span.Days % 31 != 0)
                    months += 1;
                return String.Format("{0} {1} trước",
                months, months == 1 ? "tháng" : "tháng");
            }
            if (span.Days > 0)
                return String.Format(" {0} {1} trước",
                span.Days, span.Days == 1 ? "ngày" : "ngày");
            if (span.Hours > 0)
                return String.Format(" {0} {1} trước",
                span.Hours, span.Hours == 1 ? "giờ" : "giờ");
            if (span.Minutes > 0)
                return String.Format(" {0} {1} trước",
                span.Minutes, span.Minutes == 1 ? "phút" : "phút");
            if (span.Seconds > 5)
                return String.Format("{0} giây trước", span.Seconds);
            if (span.Seconds <= 5)
                return "Vừa đăng";
            return string.Empty;
        }
        public static string StripHTML(string input)
        {
            return HttpUtility.HtmlDecode(Regex.Replace(input, "<.*?>", String.Empty));
        }
        public static bool IsNumeric(object Expression)
        {
            double retNum;
            bool isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        public static string AmountToWord(double amount)
        {
            string amountword = string.Empty;
            if ((amount < 0) || (amount > 999999999999999))
            {
                return "Số quá lớn";
            }
            else
            {
                if ((amount >= 1000000000) && (amount <= 999999999999999))    //TỶ
                {
                    double ty = Math.Round((amount / 1000000000), 2);
                    amountword = $"{ty} tỷ";
                }
                if ((amount >= 1000000) && (amount < 1000000000)) //TRIỆU
                {
                    double trieu = Math.Round((amount / 1000000), 2);
                    amountword = $"{trieu} triệu";

                }
            }
            return amountword;
        }
        public static string So_chu(decimal gNum)
        {
            if (gNum == 0)
                return "Không đồng";

            string lso_chu = "";
            string tach_mod = "";
            string tach_conlai = "";
            decimal Num = Math.Round(gNum, 0);
            //double Num = clsCpn.Round(gNum, 3);

            string gN = Convert.ToString(Num);
            int m = Convert.ToInt32(gN.Length / 3);
            int mod = gN.Length - m * 3;
            string dau = "[+]";

            // Dau [+ , - ]
            if (gNum < 0)
                dau = "[-]";
            dau = "";

            // Tach hang lon nhat
            if (mod.Equals(1))
                tach_mod = "00" + Convert.ToString(Num.ToString().Trim().Substring(0, 1)).Trim();
            if (mod.Equals(2))
                tach_mod = "0" + Convert.ToString(Num.ToString().Trim().Substring(0, 2)).Trim();
            if (mod.Equals(0))
                tach_mod = "000";
            // Tach hang con lai sau mod :
            if (Num.ToString().Length > 2)
                tach_conlai = Convert.ToString(Num.ToString().Trim().Substring(mod, Num.ToString().Length - mod)).Trim();

            ///don vi hang mod
            int im = m + 1;
            if (mod > 0)
                lso_chu = Tach(tach_mod).ToString().Trim() + " " + Donvi(im.ToString().Trim());
            /// Tach 3 trong tach_conlai

            int i = m;
            int _m = m;
            int j = 1;
            string tach3 = "";
            string tach3_ = "";

            while (i > 0)
            {
                tach3 = tach_conlai.Trim().Substring(0, 3).Trim();
                tach3_ = tach3;
                lso_chu = lso_chu.Trim() + " " + Tach(tach3.Trim()).Trim();
                m = _m + 1 - j;
                if (!tach3_.Equals("000"))
                    lso_chu = lso_chu.Trim() + " " + Donvi(m.ToString().Trim()).Trim();
                tach_conlai = tach_conlai.Trim().Substring(3, tach_conlai.Trim().Length - 3);

                i = i - 1;
                j = j + 1;
            }
            if (lso_chu.Trim().Substring(0, 1).Equals("k"))
                lso_chu = lso_chu.Trim().Substring(10, lso_chu.Trim().Length - 10).Trim();
            if (lso_chu.Trim().Substring(0, 1).Equals("l"))
                lso_chu = lso_chu.Trim().Substring(2, lso_chu.Trim().Length - 2).Trim();
            if (lso_chu.Trim().Length > 0)
                lso_chu = dau.Trim() + " " + lso_chu.Trim().Substring(0, 1).Trim().ToUpper() + lso_chu.Trim().Substring(1, lso_chu.Trim().Length - 1).Trim() + " đồng chẵn.";

            return lso_chu.ToString().Trim();

        }

        private static string Tach(string tach3)
        {
            string Ktach = "";
            if (tach3.Equals("000"))
                return "";
            if (tach3.Length == 3)
            {
                string tr = tach3.Trim().Substring(0, 1).ToString().Trim();
                string ch = tach3.Trim().Substring(1, 1).ToString().Trim();
                string dv = tach3.Trim().Substring(2, 1).ToString().Trim();
                if (tr.Equals("0") && ch.Equals("0"))
                    Ktach = " không trăm lẻ " + Chu(dv.ToString().Trim()) + " ";
                if (!tr.Equals("0") && ch.Equals("0") && dv.Equals("0"))
                    Ktach = Chu(tr.ToString().Trim()).Trim() + " trăm ";
                if (!tr.Equals("0") && ch.Equals("0") && !dv.Equals("0"))
                    Ktach = Chu(tr.ToString().Trim()).Trim() + " trăm lẻ " + Chu(dv.Trim()).Trim() + " ";
                if (tr.Equals("0") && Convert.ToInt32(ch) > 1 && Convert.ToInt32(dv) > 0 && !dv.Equals("5"))
                    Ktach = " không trăm " + Chu(ch.Trim()).Trim() + " mươi " + Chu(dv.Trim()).Trim() + " ";
                if (tr.Equals("0") && Convert.ToInt32(ch) > 1 && dv.Equals("0"))
                    Ktach = " không trăm " + Chu(ch.Trim()).Trim() + " mươi ";
                if (tr.Equals("0") && Convert.ToInt32(ch) > 1 && dv.Equals("5"))
                    Ktach = " không trăm " + Chu(ch.Trim()).Trim() + " mươi lăm ";
                if (tr.Equals("0") && ch.Equals("1") && Convert.ToInt32(dv) > 0 && !dv.Equals("5"))
                    Ktach = " không trăm mười " + Chu(dv.Trim()).Trim() + " ";
                if (tr.Equals("0") && ch.Equals("1") && dv.Equals("0"))
                    Ktach = " không trăm mười ";
                if (tr.Equals("0") && ch.Equals("1") && dv.Equals("5"))
                    Ktach = " không trăm mười lăm ";
                if (Convert.ToInt32(tr) > 0 && Convert.ToInt32(ch) > 1 && Convert.ToInt32(dv) > 0 && !dv.Equals("5"))
                    Ktach = Chu(tr.Trim()).Trim() + " trăm " + Chu(ch.Trim()).Trim() + " mươi " + Chu(dv.Trim()).Trim() + " ";
                if (Convert.ToInt32(tr) > 0 && Convert.ToInt32(ch) > 1 && dv.Equals("0"))
                    Ktach = Chu(tr.Trim()).Trim() + " trăm " + Chu(ch.Trim()).Trim() + " mươi ";
                if (Convert.ToInt32(tr) > 0 && Convert.ToInt32(ch) > 1 && dv.Equals("5"))
                    Ktach = Chu(tr.Trim()).Trim() + " trăm " + Chu(ch.Trim()).Trim() + " mươi lăm ";
                if (Convert.ToInt32(tr) > 0 && ch.Equals("1") && Convert.ToInt32(dv) > 0 && !dv.Equals("5"))
                    Ktach = Chu(tr.Trim()).Trim() + " trăm mười " + Chu(dv.Trim()).Trim() + " ";

                if (Convert.ToInt32(tr) > 0 && ch.Equals("1") && dv.Equals("0"))
                    Ktach = Chu(tr.Trim()).Trim() + " trăm mười ";
                if (Convert.ToInt32(tr) > 0 && ch.Equals("1") && dv.Equals("5"))
                    Ktach = Chu(tr.Trim()).Trim() + " trăm mười lăm ";

            }


            return Ktach;

        }

        private static string Chu(string gNumber)
        {
            string result = "";
            switch (gNumber)
            {
                case "0":
                    result = "không";
                    break;
                case "1":
                    result = "một";
                    break;
                case "2":
                    result = "hai";
                    break;
                case "3":
                    result = "ba";
                    break;
                case "4":
                    result = "bốn";
                    break;
                case "5":
                    result = "năm";
                    break;
                case "6":
                    result = "sáu";
                    break;
                case "7":
                    result = "bảy";
                    break;
                case "8":
                    result = "tám";
                    break;
                case "9":
                    result = "chín";
                    break;
            }
            return result;
        }

        private static string Donvi(string so)
        {
            string Kdonvi = "";

            if (so.Equals("1"))
                Kdonvi = "";
            if (so.Equals("2"))
                Kdonvi = "nghìn";
            if (so.Equals("3"))
                Kdonvi = "triệu";
            if (so.Equals("4"))
                Kdonvi = "tỷ";
            if (so.Equals("5"))
                Kdonvi = "nghìn tỷ";
            if (so.Equals("6"))
                Kdonvi = "triệu tỷ";
            if (so.Equals("7"))
                Kdonvi = "tỷ tỷ";

            return Kdonvi;
        }


        public static string GetTemplate<T>(T model, string content, EnumTypeTemplate enumType)
        {
            try
            {
                string invoiceNo = string.Empty;

                string comemail = string.Empty;
                string comname = string.Empty;
                string comphone = string.Empty;
                string comaddress = string.Empty;

                string buyer = string.Empty;
                string cusPhone = string.Empty;
                string cuscode = string.Empty;
                string cusAddress = string.Empty;
                string casherName = string.Empty;
                string staffName = string.Empty;
                string ngaythangnamxuat = string.Empty;
                string giovao = string.Empty;

                string tableProduct = string.Empty;

                string tientruocthue = string.Empty;
                string tongtien = string.Empty;
                string giamgia = string.Empty;
                string khachcantra = string.Empty;
                string khachthanhtoan = string.Empty;
                string tienthuatrakhach = string.Empty;
                string thongtintracuuhoadon = string.Empty;
                string thuesuat = string.Empty;
                string tienthue = string.Empty;
                string kyhieuhoadon = string.Empty;
                string sohoadon = string.Empty;
                string tongsoluong = string.Empty;
                string tenbanphong = string.Empty;

                string linktracuu = string.Empty;
                string matracuu = string.Empty;
                string macoquanthue = string.Empty;

                string lienhehotline = string.Empty;
                if (enumType == EnumTypeTemplate.PRINT_BEP)
                {
                    foreach (PropertyInfo propertyInfo in model.GetType().GetProperties())
                    {
                        string value = model.GetType().GetProperty(propertyInfo.Name).GetValue(model)?.ToString();
                        if (value == null)
                        {
                            value = string.Empty;
                        }
                        switch (propertyInfo.Name)
                        {
                            case "comname":
                                comname = value;
                                break; 
                            case "staffName":
                                staffName = value;
                                break; 
                            case "ngaythangnamxuat":
                                ngaythangnamxuat = value;
                                break;
                            case "tongsoluong":
                                tongsoluong = value;
                                break; 
                            case "tenbanphong":
                                tenbanphong = value;
                                break;
                        }
                    }
                }
                else if (enumType == EnumTypeTemplate.INVOICEPOS)
                {
                    foreach (PropertyInfo propertyInfo in model.GetType().GetProperties())
                    {
                        string value = model.GetType().GetProperty(propertyInfo.Name).GetValue(model)?.ToString();
                        if (value == null)
                        {
                            value = string.Empty;
                        }
                        switch (propertyInfo.Name)
                        {
                            case "macoquanthue":
                                macoquanthue = value;
                                break; 
                            case "matracuu":
                                matracuu = value;
                                break; 
                            case "linktracuu":
                                linktracuu = value;
                                break; 
                            case "lienhehotline":
                                lienhehotline = value;
                                break; 
                            case "invoiceNo":
                                invoiceNo = value;
                                break;  
                            case "thongtintracuuhoadon":
                                thongtintracuuhoadon = value;
                                break;
                            case "comname":
                                comname = value;
                                break;
                            case "comphone":
                                comphone = value;
                                break;
                            case "comemail":
                                comemail = value;
                                break;
                            case "comaddress":
                                comaddress = value;
                                break;
                            case "tenbanphong":
                                tenbanphong = value;
                                break;
                            case "buyer":
                                buyer = value;
                                break;
                            case "cusPhone":
                                cusPhone = value;
                                break;
                            case "cusAddress":
                                cusAddress = value;
                                break;
                            case "cuscode":
                                cuscode = value;
                                break;
                            case "casherName":
                                casherName = value;
                                break;
                            case "staffName":
                                staffName = value;
                                break;
                            case "ngaythangnamxuat":
                                ngaythangnamxuat = value;
                                break; 
                            case "giovao":
                                giovao = value;
                                break;
                            case "tongtien":
                                tongtien = value;
                                break; 
                            case "tientruocthue":
                                tientruocthue = value;
                                break;
                            case "giamgia":
                                giamgia = value;
                                break;
                            case "khachcantra":
                                khachcantra = value;
                                break;
                            case "khachthanhtoan":
                                khachthanhtoan = value;
                                break;
                            case "tienthuatrakhach":
                                tienthuatrakhach = value;
                                break;
                            case "tableProduct":
                                tableProduct = value;
                                break;  
                            case "thuesuat":
                                thuesuat = value;
                                break;  
                            case "tienthue":
                                tienthue = value;
                                break;
                            case "kyhieuhoadon":
                                kyhieuhoadon = value;
                                break;
                            case "sohoadon":
                                sohoadon = value;
                                break;
                            default:
                                break;
                        }

                    }
                }


                var template = HttpUtility.HtmlDecode(content);
                var arrtoken = new Dictionary<string, string>()
                {
                    { "{matracuu}",matracuu},
                    { "{linktracuu}",linktracuu},
                    { "{macoquanthue}",macoquanthue},
                    { "{lienhehotline}",lienhehotline},
                    { "{invoiceNo}",invoiceNo},
                    { "{casherName}",casherName},
                    { "{comemail}",comemail},
                    { "{comphone}",comphone},
                    { "{comname}",comname},
                    { "{comaddress}",comaddress},
                    { "{buyer}",buyer},
                    { "{cusPhone}",cusPhone},
                    { "{cuscode}",cuscode},
                    { "{cusAddress}",cusAddress },
                    { "{tableProduct}",tableProduct },
                    { "{ngaythangnamxuat}",ngaythangnamxuat },
                    { "{giovao}",giovao },
                    { "{tientruocthue}",tientruocthue },
                    { "{tongtien}",tongtien },
                    { "{giamgia}",giamgia },
                    { "{khachcantra}",khachcantra },
                    { "{khachthanhtoan}",khachthanhtoan },
                    { "{tienthuatrakhach}",tienthuatrakhach },
                    { "{thongtintracuuhoadon}",thongtintracuuhoadon },
                    { "{thuesuat}",thuesuat },
                    { "{tienthue}",tienthue },
                    { "{sohoadon}",sohoadon },
                    { "{kyhieuhoadon}",kyhieuhoadon },
                    { "{tongsoluong}",tongsoluong },
                    { "{tenbanphong}",tenbanphong },
                    { "{staffName}",staffName }
                };
                // builder.HtmlBody = BuildTemplate(template, arrtoken);
                return BuildTemplate(template, arrtoken);
            }
            catch (Exception e)
            {
                throw;
            }
        }
        private static string BuildTemplate(string html, Dictionary<string, string> arrtoken)
        {
            if (string.IsNullOrEmpty(html)) return html;
            foreach (var token in arrtoken)
            {
                html = html.Replace(token.Key, token.Value);
            }
            return html;
        }
    }
}
