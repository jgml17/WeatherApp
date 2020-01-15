using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Services.WeatherAppServices.Base
{
    /// <summary>
    /// Defautl calls to RestAPI services
    /// </summary>
    public class RestApiService : IDisposable
    {
        private bool _disposed;
        private HttpClient _httpClient;
        private FormUrlEncodedContent _formUrlEncodedContent;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="uri"></param>
        /// <param name="args"></param>
        public RestApiService(IHttpClientFactory httpClientFactory,
            string uri, 
            //TokenModel token = null, 
            KeyValuePair<string, string>[] args = null)
        {
            if (httpClientFactory != null)
            {
                _httpClient = httpClientFactory.CreateClient();
            }
            else
            {
                _httpClient = new HttpClient();
            }

            _httpClient.BaseAddress = new Uri(uri);
            //_httpClient.DefaultRequestHeaders.Add("Accept-Language", AppResources.Locale);

            //if (token?.access_token != null)
            //    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.access_token);

            if (args != null)
            {
                List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>();
                foreach (var item in args)
                {
                    values.Add(item);
                }
                _formUrlEncodedContent = new FormUrlEncodedContent(values);
            }

            _disposed = false;
        }

        /// <summary>
        /// Get Async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="cancelToken"></param>
        /// <param name="args"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        public async Task<BaseServiceResponse<T>> GetAsync<T>(string endpoint, CancellationToken cancelToken, string args = null, bool? claim = null)
        {
            BaseServiceResponse<T> response = new BaseServiceResponse<T>();

            {
                var ret = await _httpClient.GetAsync($"{endpoint}{args}", cancelToken);
                if (ret.IsSuccessStatusCode)
                {
                    var result = await ret.Content.ReadAsStringAsync();
                    response.StatusCodeResponse = ret.StatusCode;
                    response.Messages.Message = string.Empty;
                    response.Response = JsonSerializer.Deserialize<T>(result);
                }
                else
                {
                    var result = await ret.Content.ReadAsStringAsync();
                    response.StatusCodeResponse = ret.StatusCode;
                    response.Messages.Message = result;
                }
            }

            return response;

            //if (claim.HasValue)
            //{
            //    if (claim.Value)
            //    {
            //        var ret = JsonConvert.DeserializeObject<T>(result, new ClaimConverter());
            //        return ret;
            //    }
            //}
        }

        /// <summary>
        /// Post Async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="cancelToken"></param>
        /// <param name="data"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<BaseServiceResponse<T>> PostAsync<T>(string endpoint, CancellationToken cancelToken, object data = null, string args = null)
        {
            BaseServiceResponse<T> response = new BaseServiceResponse<T>();

            {
                var payload = GetPayload(data);
                string request = $"{endpoint}";

                if (args != null)
                    request = $"{endpoint}/{args}";

                HttpResponseMessage ret;
                if (data != null)
                    ret = await _httpClient.PostAsync(request, payload, cancelToken);
                else
                    ret = await _httpClient.PostAsync(request, _formUrlEncodedContent, cancelToken);

                if (ret.IsSuccessStatusCode)
                {
                    var result = await ret.Content.ReadAsStringAsync();
                    response.StatusCodeResponse = ret.StatusCode;
                    response.Messages.Message = string.Empty;
                    response.Response = JsonSerializer.Deserialize<T>(result);
                }
                else
                {
                    var result = await ret.Content.ReadAsStringAsync();
                    response.StatusCodeResponse = ret.StatusCode;
                    response.Messages.Message = result;
                }
            }
            

            return response;
        }

        private static StringContent GetPayload(object data)
        {
            var json = JsonSerializer.Serialize(data);

            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        #region Dispose

        public void Dispose()
        {
            //if (_disposed) throw new ObjectDisposedException("Resource was disposed.");

            Dispose(true);

            // Use SupressFinalize in case a subclass
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // If you need thread safety, use a lock around these 
            // operations, as well as in your methods that use the resource.
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_formUrlEncodedContent != null)
                        _formUrlEncodedContent.Dispose();

                    if (_httpClient != null)
                        _httpClient.Dispose();
#if DEBUG
                    Console.WriteLine("Objects disposed.");
#endif
                }

                // Indicate that the instance has been disposed.
                _httpClient = null;
                _formUrlEncodedContent = null;

                _disposed = true;
            }
        }

        #endregion
    }
}
