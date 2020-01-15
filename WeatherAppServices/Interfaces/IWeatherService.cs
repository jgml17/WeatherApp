using Core.Models.WeatherAppModels;
using Core.Services.WeatherAppServices.Base;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Services.WeatherAppServices.Interfaces
{
    public interface IWeatherService
    {
        /// <summary>
        /// Get Local Weather from DarkSky
        /// </summary>
        /// <param name="APIURI"></param>
        /// <param name="token"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="httpClientFactory"></param>
        /// <param name="cancelToken"></param>
        /// <returns></returns>
        Task<BaseServiceResponse<WeatherModel>> GetLocalWeather(string APIURI, string token, string latitude, string longitude, IHttpClientFactory httpClientFactory, CancellationTokenSource cancelToken);

    }
}
