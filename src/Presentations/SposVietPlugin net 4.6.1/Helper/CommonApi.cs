using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SposVietPlugin_net_4._6._1.Helper
{
    public class CommonApi
    {
        private static string urlApi = "http://api.sposviet.vn/api/";
        public CommonApi() { }
        public static async Task<string> PostApiAsync(string requestUri, string jsonData,Dictionary<string,string> additionalHeaders = null) {
            
            using (HttpClientHandler httpClientHandler = new HttpClientHandler())
            {
                //Uncomment below line to Skip cert validation check
                //httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                string result = "Error";
                using (HttpClient httpClient = new HttpClient(httpClientHandler))
                {
                    string uri = $"{urlApi}{requestUri}";
                    AddHeaders(httpClient, additionalHeaders);
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
        public static async Task<string> GetApiAsync(string Url,Dictionary<string, string> additionalHeaders = null)
        {
            try
            {
                using (HttpClientHandler httpClientHandler = new HttpClientHandler())
                {
                 
                    using (HttpClient httpClient = new HttpClient(httpClientHandler))
                    {
                        string uri = $"{urlApi}{Url}";
                        AddHeaders(httpClient, additionalHeaders);
                        using (var response = await httpClient.GetAsync(uri))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            return apiResponse;
                        }
                    }
                 
                }
                //using (var httpClient = new HttpClient())
                //{
                //    AddHeaders(httpClient, additionalHeaders);
                //    string uri = $"{urlApi}{Url}";
                //   // string uri = $"http://api.sposviet.vn/api/Company/search?Id=13";
                //    using (var response = await httpClient.GetAsync(uri))
                //    {
                //        string apiResponse = await response.Content.ReadAsStringAsync();
                //        return apiResponse;
                //    }
                //}
            }
            catch (Exception e)
            {
                LogControl.Write(e.ToString());
                throw;
            }
        }
        private static void AddHeaders(HttpClient httpClient, Dictionary<string, string> additionalHeaders)
        {
            //httpClient.DefaultRequestHeaders.Add("Accept", "*/*");

            //No additional headers to be added
            if (additionalHeaders == null)
                return;

            foreach (KeyValuePair<string, string> current in additionalHeaders)
            {
                httpClient.DefaultRequestHeaders.Add(current.Key, current.Value);
            }
        }
    }
    
}
