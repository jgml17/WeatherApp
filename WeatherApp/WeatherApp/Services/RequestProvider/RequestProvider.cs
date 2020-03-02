using Core.WeatherApp.Exceptions;
using Core.WeatherApp.Resources;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Core.WeatherApp.Services.RequestProvider
{
    /// <summary>
    /// Defautl calls to RestAPI services
    /// </summary>

    public class RequestProvider : IRequestProvider
    {
        private FormUrlEncodedContent _formUrlEncodedContent;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Constructor for UnitTests
        /// </summary>
        public RequestProvider()
        {
            // Client Factory
            _httpClientFactory = null;
        }

        /// <summary>
        /// Constructor for App
        /// </summary>
        /// <param name="httpClientFactory"></param>
        public RequestProvider(IHttpClientFactory httpClientFactory)
        {
            // Client Factory
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Get request
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="cancelToken"></param>
        /// <param name="args"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string endpoint, CancellationToken cancelToken, string token = null, KeyValuePair<string, string>[] args = null, bool? claim = null)
        {
            HttpClient httpClient = CreateHttpClient();
            AddHeaderParams(httpClient, token, args);

            HttpResponseMessage response = await httpClient.GetAsync(endpoint, cancelToken);

            await HandleResponse(response);

            string serialized = await response.Content.ReadAsStringAsync();
            
            T result = JsonSerializer.Deserialize<T>(serialized);

            //if (claim.HasValue)
            //{
            //    if (claim.Value)
            //    {
            //        var ret = JsonConvert.DeserializeObject<T>(result, new ClaimConverter());
            //        return ret;
            //    }
            //}

            return result;
        }

        /// <summary>
        /// Post request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="cancelToken"></param>
        /// <param name="token"></param>
        /// <param name="data"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<T> PostAsync<T>(string endpoint, CancellationToken cancelToken, string token = null, object data = null, KeyValuePair<string, string>[] args = null)
        {
            var payload = GetPayload(data);
            string request = $"{endpoint}";

            HttpClient httpClient = CreateHttpClient();

            AddHeaderParams(httpClient, token, args);
            HttpResponseMessage response;

            if (data != null)
                response = await httpClient.PostAsync(request, payload, cancelToken);
            else
                response = await httpClient.PostAsync(request, _formUrlEncodedContent, cancelToken);

            await HandleResponse(response);

            string serialized = await response.Content.ReadAsStringAsync();

            T result = JsonSerializer.Deserialize<T>(serialized);

            return result;
        }

        /// <summary>
        /// Put request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="cancelToken"></param>
        /// <param name="token"></param>
        /// <param name="data"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<T> PutAsync<T>(string endpoint, CancellationToken cancelToken, string token = null, object data = null, KeyValuePair<string, string>[] args = null)
        {
            var payload = GetPayload(data);
            string request = $"{endpoint}";

            HttpClient httpClient = CreateHttpClient();
            AddHeaderParams(httpClient, token, args);

            HttpResponseMessage response;
            if (data != null)
                response = await httpClient.PutAsync(request, payload, cancelToken);
            else
                response = await httpClient.PutAsync(request, _formUrlEncodedContent, cancelToken);

            await HandleResponse(response);

            string serialized = await response.Content.ReadAsStringAsync();

            T result = JsonSerializer.Deserialize<T>(serialized);

            return result;
        }



        /// <summary>
        /// Delete request
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="cancelToken"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task DeleteAsync(string endpoint, CancellationToken cancelToken, string token = null)
        {
            HttpClient httpClient = CreateHttpClient();
            await httpClient.DeleteAsync(endpoint, cancelToken);
        }

        #region Helpers



        /// <summary>
        /// Payload
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private StringContent GetPayload(object data)
        {
            var json = JsonSerializer.Serialize(data);

            return new StringContent(json, Encoding.UTF8);
        }

        /// <summary>
        /// Create HTTP CLIENT
        /// </summary>
        /// <returns></returns>
        private HttpClient CreateHttpClient()
        {
            HttpClient httpClient;

            if (_httpClientFactory != null)
            {
                httpClient = _httpClientFactory.CreateClient();
            }
            else
            {
                httpClient = new HttpClient();
            }

            return httpClient;
        }

        /// <summary>
        /// Add Headers Parameters
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="token"></param>
        /// <param name="args"></param>
        private void AddHeaderParams(HttpClient httpClient, string token, KeyValuePair<string, string>[] args = null)
        {
            httpClient.DefaultRequestHeaders.Add("Accept-Language", AppResources.Locale);
            //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrEmpty(token))
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            if (args != null)
            {
                List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>();
                foreach (var item in args)
                {
                    values.Add(item);
                }

                _formUrlEncodedContent = new FormUrlEncodedContent(values);
            }
        }

        /// <summary>
        /// Http Requests handle 
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private async Task HandleResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                // Access errors
                if (response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new ServiceAuthenticationException(content);
                }

                // Errors
                throw new HttpRequestExceptionEx(response.StatusCode, content);
            }
        }

        #endregion
    }
}
