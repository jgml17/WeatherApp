using Core.WeatherApp.Services.RequestProvider;
using Core.WeatherApp.Services.Weather;
using Core.WeatherApp.ViewModels;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NUnitTestWeatherApp
{
    public class WeatherServiceTests
    {
        private WeatherService service;
        //private WeatherViewModel viewModel;

        [SetUp]
        public void Setup()
        {
            service = new WeatherService(new RequestProvider(), null);
            //viewModel = new WeatherViewModel(service, null, null);
        }

        /// <summary>
        /// GetLocalWeather Service OK Test
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetLocalWeatherServiceOK()
        {
            try
            {
                var ret = await service.GetLocalWeather("-23.5489", "-46.6388"); // Sao Paulo - Brazil
                if (ret.Daily.Data.Count > 0)
                {
                    Assert.Pass("COUNT: {0}", ret.Daily.Data.Count);
                }
                else
                {
                    Assert.Fail("NOTHING");
                }
            }
            catch (Exception ex)
            {
                if (ex != ex as SuccessException)
                {
                    // ERROR
                    Assert.Fail("ERROR {0}", ex.Message);
                }
            }
        }
    }
}
