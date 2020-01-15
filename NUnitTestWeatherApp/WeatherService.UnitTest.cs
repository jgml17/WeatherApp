using Core.Models.WeatherAppModels;
using Core.Services.WeatherAppServices;
using Core.Services.WeatherAppServices.Base;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NUnitTestWeatherApp
{
    public class WeatherServiceTests
    {
        private WeatherService service;
        private int TimeOut;
        private const string APIURI = "https://api.darksky.net/";
        private const string TOKEN = "10fbddb2009ce521f23f18dd962946d7";

        [SetUp]
        public void Setup()
        {
            TimeOut = 10000; // 10 segundos
            service = new WeatherService();
        }

        [Test]
        public async Task GetLocalWeatherOK()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            var task = service.GetLocalWeather(APIURI, TOKEN, "-23.5489", "-46.6388", null, tokenSource);

            if (task == await Task.WhenAny(task, Task.Delay(TimeOut, tokenSource.Token)))
            {
                var ret = await task;

                if (ret != null)
                {
                    switch (ret.StatusCodeResponse)
                    {
                        case HttpStatusCode.OK:
                            if (ret.Response.Daily.Data.Count > 0)
                            {
                                Assert.Pass("COUNT: ", ret.Response.Daily.Data.Count);
                            }
                            else
                            {
                                Assert.Fail("SEM PREVISAO");
                            }
                            break;

                        case HttpStatusCode.Unauthorized:
                            Assert.Fail("Unauthorized");
                            break;

                        default:
                            // ERROR
                            Assert.Fail("ERROR {0}", ret.Messages.Message);
                            break;

                    }
                }

                tokenSource.Cancel();
            }
            else
            {
                tokenSource.Cancel();
                Assert.Fail("TIMEOUT");
            }
        }
    }
}
