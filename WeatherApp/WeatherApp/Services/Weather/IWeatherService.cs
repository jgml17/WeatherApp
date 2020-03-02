using Core.Models.WeatherAppModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.WeatherApp.Services.Weather
{
    public interface IWeatherService
    {
        Task<WeatherModel> GetLocalWeather(string latitude, string longitude);
    }
}
