using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Core.Services.WeatherAppServices.Interfaces;
using Core.Models.WeatherAppModels;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Core.WeatherApp.Helpers;

namespace WeatherApp.ViewModels
{
    public class BaseViewModel<T> : INotifyPropertyChanged
    {
        protected const string LOGIN = "LOGIN";

        protected IHttpClientFactory _httpClientFactory;
        protected ILogger<T> _logger;
        
        // 20 segundos
        protected const int TimeOut = 20000;
        protected const int TIMER_SECONDS = 60;

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        bool isBusy1 = false;
        public bool IsBusy1
        {
            get { return isBusy1; }
            set { SetProperty(ref isBusy1, value); }
        }

        bool isBusy2 = false;
        public bool IsBusy2
        {
            get { return isBusy2; }
            set { SetProperty(ref isBusy2, value); }
        }

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public BaseViewModel()
        {

        }

        public BaseViewModel(IHttpClientFactory httpClientFactory, ILogger<T> logger)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        //public bool IsLogged()
        //{
        //    var ret = Settings<LoginModel>.GetValue(LOGIN);
        //    return ret != null ? true : false;
        //}

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
