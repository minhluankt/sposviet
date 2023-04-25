using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SposVietPluginKySo.Helper
{
    public class CommonApi
    {
        private static string urlApi = "http://api.sposviet.vn/api/";
        public CommonApi() { }
        public static async Task<string> GetApiAsync(string Url,string jsonData) {
            using (HttpClientHandler httpClientHandler = new HttpClientHandler())
            {
                string result = "Error";
                using (HttpClient httpClient = new HttpClient(httpClientHandler))
                {
                    string uri = $"{urlApi}{Url}";
                    result = await httpClient.PostAsync(uri, new StringContent(jsonData)
                    {
                        Headers =
                        {
                            ContentType = new MediaTypeHeaderValue("application/json")
                        }
                    }).Result.Content.ReadAsStringAsync();
                }
                return result;
            }
        }
    }
    chặn không cho kéo form
}
