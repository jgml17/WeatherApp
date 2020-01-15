using Core.Models.WeatherAppModels;
using Core.Services.WeatherAppServices.Base;
using Core.Services.WeatherAppServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Services.WeatherAppServices
{
    public class WeatherService : IWeatherService
    {

        public async Task<BaseServiceResponse<WeatherModel>> GetLocalWeather(string APIURI, string token, string latitude, string longitude, IHttpClientFactory httpClientFactory, CancellationTokenSource cancelToken)
        {
            // Start basemodel with parameters of class and method
            BaseServiceResponse<WeatherModel> model = new BaseServiceResponse<WeatherModel>("WeatherService", "GetLocalWeather");

            using (var apiClient = new RestApiService(httpClientFactory, APIURI))
            {
                try
                {
                    var response = await apiClient.GetAsync<WeatherModel>("forecast", cancelToken.Token,
                        $"/{token}/{latitude},{longitude}?lang=pt&exclude=minutely,hourly,alerts,flags&units=ca");

                    if (response.StatusCodeResponse != HttpStatusCode.OK)
                    {
                        // Return Status Code Error...
                        model.StatusCodeResponse = response.StatusCodeResponse;
                        model.Messages.Message = response.Messages.Message;
                        model.Messages.Properties.Add(response.StatusCodeResponse.ToString(), response.Messages.Message);

                        //Analytics.TrackEvent("RegisterUserdEvent", properties);

                        Console.WriteLine($"==== ERROR ===== >>> Status Code -> {response.StatusCodeResponse}");
                    }
                    else
                    {
                        model.Response = response.Response;
                        model.StatusCodeResponse = HttpStatusCode.OK;
                    }
                }
                catch (Exception ex)
                {
                    model.StatusCodeResponse = HttpStatusCode.InternalServerError;
                    model.Messages.Exception = ex;
                    model.Messages.Message = ex.Message;

                    //Crashes.TrackError(ex, properties);

                    Console.WriteLine($"==== ERROR ===== >>> {ex.Message}");
                }

            }

            return model;
        }
    }
}
