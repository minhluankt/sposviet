﻿using Application.Interfaces.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public sealed class RestClientHelper : IRestClientHelper
    {
        private readonly HttpClient httpClient;

        public RestClientHelper(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
        }

        public async Task<string> GetAsync(string requestUri, Dictionary<string, string> additionalHeaders = null)
        {
            //IDictionary<int, string> numberNames = new Dictionary<int, string>();
            //numberNames.Add(1, "One"); //adding a key/value using the Add() method
            //numberNames.Add(2, "Two");
            //numberNames.Add(3, "Three");
            string result = "Error";
            using (HttpClientHandler httpClientHandler = new HttpClientHandler())
            {
                //Uncomment below line to Skip cert validation check
                //httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }

                using (HttpClient httpClient = new HttpClient(httpClientHandler))
                {
                    AddHeaders(httpClient, additionalHeaders);
                    using (var response = await httpClient.GetAsync(requestUri))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        return apiResponse;
                    }
                   // result = await httpClient.GetStringAsync(requestUri);
                }
            }
            //return result;
        }

        public async Task<string> PostAsync<T>(string requestUri, T request, Dictionary<string, string> additionalHeaders = null) where T : class
        {
            string result = "Error";
            using (HttpClientHandler httpClientHandler = new HttpClientHandler())
            {
                //Uncomment below line to Skip cert validation check
                //httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }

                using (HttpClient httpClient = new HttpClient(httpClientHandler))
                {
                    AddHeaders(httpClient, additionalHeaders);
                    result = await httpClient.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(request))
                    {
                        Headers =
                        {
                            ContentType = new MediaTypeHeaderValue("application/json")
                        }
                    }).Result.Content.ReadAsStringAsync();
                }
            }
            return result;
        }

        public async Task<string> DeleteAsync(string requestUri, Dictionary<string, string> additionalHeaders = null)
        {
            string result = "Error";
            using (HttpClientHandler httpClientHandler = new HttpClientHandler())
            {
                //Uncomment below line to Skip cert validation check
                //httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }

                using (HttpClient httpClient = new HttpClient(httpClientHandler))
                {
                    AddHeaders(httpClient, additionalHeaders);
                    var httpResponseMessage = await httpClient.DeleteAsync(requestUri);
                    result = await httpResponseMessage.Content.ReadAsStringAsync();
                }
            }
            return result;
        }

        public async Task<string> PutAsync<T>(string requestUri, T request, Dictionary<string, string> additionalHeaders = null) where T : class
        {
            string result = "Error";
            using (HttpClientHandler httpClientHandler = new HttpClientHandler())
            {
                //Uncomment below line to Skip cert validation check
                //httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }

                using (HttpClient httpClient = new HttpClient(httpClientHandler))
                {
                    AddHeaders(httpClient, additionalHeaders);

                    var json = JsonConvert.SerializeObject(request, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        NullValueHandling = NullValueHandling.Ignore
                    });

                    var httpContent = new StringContent(json);
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var httpResponseMessage = await httpClient.PutAsync(requestUri, httpContent);
                    result = await httpResponseMessage.Content.ReadAsStringAsync();
                }
            }
            return result;
        }

        public async Task<string> PatchAsync<T>(string requestUri, T request, Dictionary<string, string> additionalHeaders = null) where T : class
        {
            string result = "Error";
            using (HttpClientHandler httpClientHandler = new HttpClientHandler())
            {
                //Uncomment below line to Skip cert validation check
                //httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }

                using (HttpClient httpClient = new HttpClient(httpClientHandler))
                {
                    AddHeaders(httpClient, additionalHeaders);

                    var json = JsonConvert.SerializeObject(request, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        NullValueHandling = NullValueHandling.Ignore
                    });

                    var httpContent = new StringContent(json);
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var httpResponseMessage = await httpClient.PatchAsync(requestUri, httpContent);
                    result = await httpResponseMessage.Content.ReadAsStringAsync();
                }
            }
            return result;
        }

        private void AddHeaders(HttpClient httpClient, Dictionary<string, string> additionalHeaders)
        {
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

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
