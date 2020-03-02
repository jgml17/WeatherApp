using Core.Models.WeatherAppModels;
using Core.WeatherApp.Helpers;
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
using Core.WeatherApp.Services.Weather;
using Core.WeatherApp.Services.Navigation;

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
        public WeatherViewModel(IWeatherService weatherService, ILogger<WeatherViewModel> logger, INavigationService<WeatherViewModel> navigationService) : base(logger, navigationService)
        {
            _weatherService = weatherService;

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
            ShowLoading(AppResources.Loading);

            try
            {
                // Get GPS Location
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    _logger.LogInformation($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                }

                // Call Api service
                var ret = await _weatherService.GetLocalWeather(location.Latitude.ToString().Replace(",", "."), location.Longitude.ToString().Replace(",", "."));

                // Fill Main Weather properties
                TimeZone = ret.Timezone;
                Icon = ret.Currently.IconFormatted;
                Temperature = ret.Currently.Temperature.ToString("##");
                Date = UnixDateTime.UnixTimeStampToDateTimeLong(ret.Currently.Time);
                Summary = ret.Currently.Summary;

                //Transform an object to another using json
                var dailyTransform = JsonSerializer.Deserialize<List<CustomDailyInfos>>(JsonSerializer.Serialize<List<DailyInfos>>(ret.Daily.Data));

                // Fill Info Panel with first item that is current day
                OriginalDailyInfo = dailyTransform[0];
                SetInfoPanel(OriginalDailyInfo);

                // Fill Daily Infos without first element that is current day infos
                dailyTransform.RemoveAt(0); // REmove first item = current

                //DailyInfos
                DailyInfos = new ObservableCollection<CustomDailyInfos>(dailyTransform);
            }
            catch (Exception ex)
            {
                ErrorHandle(ex, "WeatherViewModel", "LoadDailyWeather");
            }
            finally
            {
                IsBusy = false;
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
