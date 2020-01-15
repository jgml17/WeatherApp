using Core.Models.WeatherAppModels;
using Core.WeatherApp.Helpers;
using Core.Services.WeatherAppServices.Base;
using Core.Services.WeatherAppServices.Interfaces;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Extensions.Logging;
using WeatherApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Core.WeatherApp.Resources;
using Xamarin.Essentials;
using Core.WeatherApp.Model;
using System.Text.Json;

namespace Core.WeatherApp.ViewModels
{
    /// <summary>
    /// Here we have ViewModel Class where we have only business
    /// Respository is uncoupled by Depenndency Injection calls
    /// </summary>
    public class WeatherViewModel : BaseViewModel<WeatherViewModel>
    {
        #region Variables

        private IWeatherService _weatherService;
        public delegate void onAction(ViewActions action, object response = null);
        public event onAction OnViewActions;

        #endregion

        #region Properties

        private ObservableCollection<CustomDailyInfos> _dailyInfos;
        public ObservableCollection<CustomDailyInfos> DailyInfos
        {
            get { return _dailyInfos; }
            set { SetProperty(ref _dailyInfos, value); }
        }

        private CustomDailyInfos _originalDailyInfo;
        public CustomDailyInfos OriginalDailyInfo
        {
            get { return _originalDailyInfo; }
            set { SetProperty(ref _originalDailyInfo, value); }
        }

        private string _timeZone;
        public string TimeZone
        {
            get { return _timeZone; }
            set { SetProperty(ref _timeZone, value); }
        }

        private string _summary;
        public string Summary
        {
            get { return _summary; }
            set { SetProperty(ref _summary, value); }
        }

        private string _icon;
        public string Icon
        {
            get { return _icon; }
            set { SetProperty(ref _icon, value); }
        }

        private string _temperature;
        public string Temperature
        {
            get { return _temperature; }
            set { SetProperty(ref _temperature, value); }
        }

        private string _date;
        public string Date
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }

        private string _humidity;
        public string Humidity
        {
            get { return _humidity; }
            set { SetProperty(ref _humidity, value); }
        }

        private string _wind;
        public string Wind
        {
            get { return _wind; }
            set { SetProperty(ref _wind, value); }
        }

        private string _pressure;
        public string Pressure
        {
            get { return _pressure; }
            set { SetProperty(ref _pressure, value); }
        }

        private string _cloudiness;
        public string Cloudiness
        {
            get { return _cloudiness; }
            set { SetProperty(ref _cloudiness, value); }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Construtor 
        /// </summary>
        /// <param name="eventosService"></param>
        /// <param name="httpClientFactory"></param>
        /// <param name="logger"></param>
        public WeatherViewModel(IWeatherService weatherService, IHttpClientFactory httpClientFactory, ILogger<WeatherViewModel> logger) : base(httpClientFactory, logger)
        {
            _weatherService = weatherService;
            _httpClientFactory = httpClientFactory;
            _logger = logger;

            Title = "Selecionar Evento";
        }

        /// <summary>
        /// Creates a timer to call weather refresh
        /// </summary>
        public void Timer()
        {
            Device.StartTimer(TimeSpan.FromSeconds(TIMER_SECONDS), () => { _ = LoadDailyWeather(); return true; });
        }

        /// <summary>
        /// Load Daily Weather from API
        /// </summary>
        /// <returns></returns>
        public async Task LoadDailyWeather()
        {
            IsBusy = true;

            // Starts Loading
            OnViewActions.Invoke(ViewActions.Loading, AppResources.Loading);

            BaseServiceResponse<WeatherModel> model = new BaseServiceResponse<WeatherModel>();

            try
            {
                CancellationTokenSource tokenSource = new CancellationTokenSource();


                // Get GPS Location
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                }

                var task = _weatherService.GetLocalWeather(AppResources.ApiUri, AppResources.DarkSkyKey, location.Latitude.ToString().Replace(",","."), location.Longitude.ToString().Replace(",", "."), _httpClientFactory, tokenSource);

                if (task == await Task.WhenAny(task, Task.Delay(TimeOut, tokenSource.Token)))
                {
                    model = await task;

                    if (model != null)
                    {
                        switch (model.StatusCodeResponse)
                        {
                            case HttpStatusCode.OK:
                                // ok

                                // Fill Main Weather properties
                                TimeZone = model.Response.Timezone;
                                Icon = model.Response.Currently.IconFormatted;
                                Temperature = model.Response.Currently.Temperature.ToString("##");
                                Date = UnixDateTime.UnixTimeStampToDateTimeLong(model.Response.Currently.Time);
                                Summary = model.Response.Currently.Summary;

                                //Transform an object to another using json
                                var dailyTransform = JsonSerializer.Deserialize<List<CustomDailyInfos>>(JsonSerializer.Serialize<List<DailyInfos>>(model.Response.Daily.Data));

                                // Fill Info Panel with first item that is current day
                                OriginalDailyInfo = dailyTransform[0];
                                SetInfoPanel(OriginalDailyInfo);

                                // Fill Daily Infos without first element that is current day infos
                                dailyTransform.RemoveAt(0); // REmove first item = current

                                //DailyInfos
                                DailyInfos = new ObservableCollection<CustomDailyInfos>(dailyTransform);

                                _logger.LogInformation($"INFO ===> {model.StatusCodeResponse.ToString()} / {model.Messages.Message}");
                                break;

                            case HttpStatusCode.Unauthorized:
                                // Call Login


                                _logger.LogInformation($"INFO ===> {model.StatusCodeResponse.ToString()} / {model.Messages.Message}");
                                break;

                            case HttpStatusCode.InternalServerError:
                                // EXCEPTION
                                Crashes.TrackError(model.Messages.Exception, model.Messages.Properties);
                                _logger.LogCritical($"EXCEPTION ===> {model.Messages.Properties} / {model.Messages.Message}");
                                break;

                            default:
                                // ERRORS
                                Analytics.TrackEvent(model.StatusCodeResponse.ToString(), model.Messages.Properties);
                                _logger.LogError($"ERROR ===> {model.Messages.Properties} / {model.Messages.Message}");
                                break;

                        }
                    }

                    tokenSource.Cancel();

                    IsBusy = false;
                }
                else
                {
                    // TIMEOUT
                    var Properties = new Dictionary<string, string> { { "WeatherViewModel", "LoadDailyWeather" } };
                    Analytics.TrackEvent("TIMEOUT", Properties);
                    _logger.LogError($"ERROR ===> WeatherViewModel, LoadDailyWeather / TIMEOUT");

                    tokenSource.Cancel();
                    IsBusy = false;
                }
            }
            catch (Exception ex)
            {
                var properties = new Dictionary<string, string> { { "WeatherViewModel", "LoadDailyWeather" } };
                // Configure error response
                model.StatusCodeResponse = HttpStatusCode.InternalServerError;
                model.Messages.Properties = properties;
                // Call AppCenter
                Crashes.TrackError(ex, properties);
                _logger.LogError($"ERROR ===> {properties} / {ex.Message}");
            }
            finally
            {
                IsBusy = false;
                // Close Loading
                OnViewActions.Invoke(ViewActions.Loading);
                // Invoke Event 
                //OnStartViewActions?.Invoke(ViewActions.ConfirmarPresenca, model.Response);
            }

        }

        /// <summary>
        /// Set Info panel with data
        /// </summary>
        /// <param name="info"></param>
        public void SetInfoPanel(CustomDailyInfos daily)
        {
            Humidity = $"{daily.Humidity*100}%";
            Wind = $"{daily.WindSpeed} {AppResources.WindUnit}";
            Pressure = $"{daily.Pressure} {AppResources.PressureUnit}";
            Cloudiness = $"{daily.CloudCover*100}%";
        }

        #endregion

        #region Commands

        /// <summary>
        /// Active Command
        /// </summary>
        //public Command ActiveCommand => new Command(() =>
        //{
        //}, () => true);

        #endregion
    }
}
