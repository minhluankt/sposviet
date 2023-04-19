using System;
using System.Security.Cryptography;
using System.Text;
using SystemVariable;

namespace Application.Providers
{
    public static class CryptoEngine
    {
        public class Secrets
        {
            public string Key { get; set; } = SystemVariableHelper.publicKey;
        }

        public static string Encrypt(string source, string key)
        {
            var byteHash = MD5.HashData(Encoding.UTF8.GetBytes(key));
            var tripleDes = new TripleDESCryptoServiceProvider
            {
                Key = byteHash,
                Mode = CipherMode.ECB
            };
            //HttpUtility.UrlEncode()

            var byteBuff = Encoding.UTF8.GetBytes(source);
            return Convert.ToBase64String(tripleDes.CreateEncryptor()
                .TransformFinalBlock(byteBuff, 0, byteBuff.Length));
        }

        public static string Decrypt(string encodedText, string key)
        {
            try
            {
                if (encodedText == null)
                {
                    return "";
                }
                //encodedText = HttpUtility.UrlDecode(encodedText);
                var byteHash = MD5.HashData(Encoding.UTF8.GetBytes(key));
                var tripleDes = new TripleDESCryptoServiceProvider
                {
                    Key = byteHash,
                    Mode = CipherMode.ECB
                };
                var byteBuff = Convert.FromBase64String(encodedText);
                return Encoding.UTF8.GetString(
                    tripleDes
                        .CreateDecryptor()
                        .TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            }
            catch (Exception e)
            {

                return "";
            }
           
           
        }
    }
}
