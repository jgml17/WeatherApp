using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.WeatherApp.Services.RequestProvider
{
    public interface IRequestProvider
    {
        Task<T> GetAsync<T>(string endpoint, CancellationToken cancelToken, string token = null, KeyValuePair<string, string>[] args = null, bool? claim = null);

        Task<T> PostAsync<T>(string endpoint, CancellationToken cancelToken, string token = null, object data = null, KeyValuePair<string, string>[] args = null);

        Task<T> PutAsync<T>(string endpoint, CancellationToken cancelToken, string token = null, object data = null, KeyValuePair<string, string>[] args = null);

        Task DeleteAsync(string endpoint, CancellationToken cancelToken, string token = null);
    }
}
