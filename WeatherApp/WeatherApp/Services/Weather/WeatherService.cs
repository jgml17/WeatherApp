using Core.Models.WeatherAppModels;
using Core.WeatherApp.Resources;
using Core.WeatherApp.Services.RequestProvider;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.WeatherApp.Services.Weather
{
    public class WeatherService : IWeatherService
    {
        protected const int TimeOut = 10000;    // 10 segundos 
        private string APIURI = AppResources.ApiUri;

        private readonly IRequestProvider _requestProvider;
        private readonly ILogger<WeatherService> _logger;

        // Constructor for app
        public WeatherService(IRequestProvider requestProvider, ILogger<WeatherService> logger)
        {
            _requestProvider = requestProvider;
            _logger = logger;
        }

        /// <summary>
        /// Get Local weather
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public async Task<WeatherModel> GetLocalWeather(string latitude, string longitude)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            WeatherModel response;

            string endpoint = $"{APIURI}/forecast/{AppResources.DarkSkyKey}/{latitude},{longitude}?lang=pt&exclude=minutely,hourly,alerts,flags&units=ca";

            Task<WeatherModel> task = _requestProvider.GetAsync<WeatherModel>(endpoint, tokenSource.Token);

            if (task == await Task.WhenAny(task, Task.Delay(TimeOut, tokenSource.Token)))
            {
                response = await task;
            }
            else
            {
                // TIMEOUT
                tokenSource.Cancel(); // Send Cancellation to thread
                throw new HttpRequestExceptionEx(HttpStatusCode.RequestTimeout, "TIMEOUT");
            }

            return response;
        }
    }
}
