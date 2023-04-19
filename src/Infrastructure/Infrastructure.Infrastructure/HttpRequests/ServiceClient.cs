using ApiHttpClient.Wrappers;
using Library;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.HttpRequests
{
    public class ServiceClient
    {
        private readonly JsonSerializerOptions _options;
        private string _baseUrl = "";
        private Uri baseAddress;
        HttpClient client;
        public ServiceClient(string uri=null)
        {

            //if (SystemVariableHelper.EnableProxyData)
            //{
            //    var proxy = new WebProxy
            //    {
            //        Address = new Uri($"{SystemVariableHelper.DomainProxy}:{SystemVariableHelper.PortProxy}"),
            //        BypassProxyOnLocal = false,
            //        UseDefaultCredentials = false,
            //        Credentials = new NetworkCredential(
            //        userName: SystemVariableHelper.UserNameProxy,
            //        password: SystemVariableHelper.PassWordProxy)
            //    };

            //    var httpClientHandler = new HttpClientHandler
            //    {
            //        Proxy = proxy,
            //    };

            //    client = new HttpClient(handler: httpClientHandler, disposeHandler: true);
            //}
            //else
            //{
            //    client = new HttpClient();
            //}


            client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            client.BaseAddress = baseAddress;
            _baseUrl = baseAddress.ToString();
        }
        public string BaseUrl
        {
            get { return _baseUrl; }
            set { _baseUrl = value; }
        }

        protected struct ObjectResponseResult<T>
        {
            public ObjectResponseResult(T responseObject, string responseText)
            {
                this.Object = responseObject;
                this.Text = responseText;
            }

            public T Object { get; }

            public string Text { get; }
        }
        protected virtual async Task<ObjectResponseResult<T>> ReadObjectResponseAsync<T>(HttpResponseMessage response)
        {

            if (response == null || response.Content == null)
            {
                return new ObjectResponseResult<T>(default, string.Empty);
            }

            var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            try
            {
               // var typedBody = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseText);
                var typedBody = ConvertSupport.ConverJsonToModel<T>(responseText);
                return new ObjectResponseResult<T>(typedBody, responseText);
            }
            catch (Newtonsoft.Json.JsonException exception)
            {
                var message = "Could not deserialize the response body string as " + typeof(T).FullName + ".";
                throw new ApiException(exception);
            }
        }
        public async Task ThrowApiExceptionAsync(HttpResponseMessage response, int status_)
        {
            string responseText_ = (response.Content == null) ? string.Empty : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            // throw new ApiException(response.m, (int)response.StatusCode, responseText_);

            if (status_ == (int)HttpStatusCode.BadRequest)
            {
                throw new ApiException($"Không tìm thấy yêu  cầu", status_);
             
            }
            else if (status_ == (int)HttpStatusCode.NotFound)
            {
                throw new ApiException("NotFound", (int)response.StatusCode, responseText_);
            }
            else if (status_ == (int)HttpStatusCode.Unauthorized)
            {
                throw new ApiException("Unauthorized", (int)response.StatusCode, responseText_);
            }
            else if (status_ == (int)HttpStatusCode.Forbidden)
            {
                throw new ApiException("Forbidden", (int)response.StatusCode, responseText_);
            }
            else if (status_ != (int)HttpStatusCode.OK && status_ != (int)HttpStatusCode.NoContent)
            {
                throw new ApiException("The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").", (int)response.StatusCode, responseText_);
            }
        }


        public async Task<ApiResponse> PostAsync(string apiurl, object data, bool model = true, string jwt ="")
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(baseAddress != null ? baseAddress.ToString().TrimEnd('/') : "").Append(apiurl);
            client.BaseAddress = new System.Uri(urlBuilder_.ToString(), System.UriKind.RelativeOrAbsolute);

            //var company = Common.ConverModelToJson(data); ;
            var request = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(data.ToString(), Encoding.UTF8);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            if (!string.IsNullOrEmpty(jwt))
            {
                client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwt);
            }

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            try
            {
                var status_ = ((int)response.StatusCode);
                if (response.IsSuccessStatusCode)
                {
                    var objectResponse_ = await ReadObjectResponseAsync<ApiResponse>(response).ConfigureAwait(false);
                    if (!model)
                    {
                        return new ApiResponse("", objectResponse_.Text);
                    }
                    return objectResponse_.Object;

                }
                else
                    await ThrowApiExceptionAsync(response, status_);

                return default;
            }
            finally
            {
                if (response != null)
                    response.Dispose();
            }
            //var content = await response.Content.ReadAsStringAsync();
            //var createdCompany = JsonSerializer.Deserialize<CompanyDto>(content);

        }
        public async Task<ApiResponse> PutAsync(string apiurl, object data, bool model = true, string jwt ="")
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(baseAddress != null ? baseAddress.ToString().TrimEnd('/') : "").Append(apiurl);
            client.BaseAddress = new System.Uri(urlBuilder_.ToString(), System.UriKind.RelativeOrAbsolute);

            //var company = Common.ConverModelToJson(data); ;
            var request = new HttpRequestMessage(HttpMethod.Put, client.BaseAddress);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(data.ToString(), Encoding.UTF8);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            if (!string.IsNullOrEmpty(jwt))
            {
                client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwt);
            }

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            try
            {
                var status_ = ((int)response.StatusCode);
                if (response.IsSuccessStatusCode)
                {
                    var objectResponse_ = await ReadObjectResponseAsync<ApiResponse>(response).ConfigureAwait(false);
                    if (!model)
                    {
                        return new ApiResponse("", objectResponse_.Text);
                    }
                    return objectResponse_.Object;

                }
                else
                    await ThrowApiExceptionAsync(response, status_);

                return default;
            }
            finally
            {
                if (response != null)
                    response.Dispose();
            }
            //var content = await response.Content.ReadAsStringAsync();
            //var createdCompany = JsonSerializer.Deserialize<CompanyDto>(content);

        }
        public async Task<ApiResponse> GetAsync(string apiUrl, object body = null, bool model = true, string jwt = "")
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(baseAddress != null ? baseAddress.ToString().TrimEnd('/') : "").Append(apiUrl);
            client.BaseAddress = new System.Uri(urlBuilder_.ToString(), System.UriKind.RelativeOrAbsolute);

            var request = new HttpRequestMessage(HttpMethod.Get, client.BaseAddress);
            //var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(body.ToString(), Encoding.UTF8, "application/json");
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            if (!string.IsNullOrEmpty(jwt))
            {
                client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwt);
            }
            var response_ = await client.SendAsync(request).ConfigureAwait(false);
        
            response_.EnsureSuccessStatusCode();
            try
            {
                var status_ = ((int)response_.StatusCode);
                if (status_ == (int)HttpStatusCode.OK || status_ == (int)HttpStatusCode.Created)
                {
                    var objectResponse_ = await ReadObjectResponseAsync<ApiResponse>(response_).ConfigureAwait(false);
                    if (!model)
                    {
                        return new ApiResponse("", objectResponse_.Text);
                    }
                    return objectResponse_.Object;
                }
                else
                    await ThrowApiExceptionAsync(response_, status_);
                return default;
            }
            finally
            {
                if (response_ != null)
                    response_.Dispose();
            }
        }
        public async Task<ApiResponse> DeleteAsync(string apiUrl, bool model = true, string jwt = "")
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(baseAddress != null ? baseAddress.ToString().TrimEnd('/') : "").Append(apiUrl);
            client.BaseAddress = new System.Uri(urlBuilder_.ToString(), System.UriKind.RelativeOrAbsolute);

            var request = new HttpRequestMessage(HttpMethod.Delete, client.BaseAddress);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
          

            if (!string.IsNullOrEmpty(jwt))
            {
                client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwt);
            }
            var response_ = await client.SendAsync(request).ConfigureAwait(false);
            response_.EnsureSuccessStatusCode();
            try
            {
                var status_ = ((int)response_.StatusCode);
                if (status_ == (int)HttpStatusCode.OK || status_ == (int)HttpStatusCode.Created)
                {
                    var objectResponse_ = await ReadObjectResponseAsync<ApiResponse>(response_).ConfigureAwait(false);
                    if (!model)
                    {
                        return new ApiResponse("", objectResponse_.Text);
                    }
                    return objectResponse_.Object;
                }
                else
                    await ThrowApiExceptionAsync(response_, status_);
                return default;
            }
            finally
            {
                if (response_ != null)
                    response_.Dispose();
            }
        }
    }
}
