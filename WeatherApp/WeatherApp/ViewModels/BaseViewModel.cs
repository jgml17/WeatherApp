using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Core.Models.WeatherAppModels;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Core.WeatherApp.Helpers;
using Core.WeatherApp.ViewModels.Popups;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Rg.Plugins.Popup.Pages;
using Core.WeatherApp;
using Microsoft.AppCenter.Crashes;
using Core.WeatherApp.Services.RequestProvider;
using Core.WeatherApp.Exceptions;
using Microsoft.AppCenter.Analytics;
using Core.WeatherApp.Services.Navigation;

namespace WeatherApp.ViewModels
{
    public class BaseViewModel<T> : INotifyPropertyChanged
    {
        private bool isLoading;

        protected const string LOGIN = "LOGIN";
        protected IHttpClientFactory _httpClientFactory;
        protected ILogger<T> _logger;
        private LoadingView _loadingView;
        protected object _navigationData;
        protected INavigationService<T> _navigationService;
        protected const int TIMER_SECONDS = 60;

        #region Properties

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

        #endregion

        public BaseViewModel()
        {
            isLoading = false;
        }

        public BaseViewModel(ILogger<T> logger, INavigationService<T> navigationService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            isLoading = false;
        }

        //public bool IsLogged()
        //{
        //    var ret = Settings<LoginModel>.GetValue(LOGIN);
        //    return ret != null ? true : false;
        //}

        public virtual void Initialize(object navigationData)
        {

        }

        public virtual Task InitializeAsync(object navigationData)
        {
            return Task.FromResult(false);
        }

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

        #region Error

        protected void ErrorHandle(Exception ex, string className, string methodName)
        {
            var Properties = new Dictionary<string, string> { { className, methodName } };

            // Access
            if (ex is ServiceAuthenticationException)
            {
                // Call Login view if it exists !!! ===========
                //PushPopupView<LoginView>();
                // ============================================

                // May or not log this ===================
                Properties.Add("Content", (ex as ServiceAuthenticationException).Content);
                Analytics.TrackEvent("ServiceAuthentication", Properties);
                _logger.LogInformation($"##### INFO ===> {(ex as ServiceAuthenticationException).Content} ######");
                // =================================

                return;
            }

            // Requests
            if (ex is HttpRequestExceptionEx)
            {
                Properties.Add("HttpCode", (ex as HttpRequestExceptionEx).HttpCode.ToString());
                Properties.Add("Message", ex.Message);
                Crashes.TrackError(ex, Properties);
                _logger.LogError($"##### ERROR {(ex as HttpRequestExceptionEx).HttpCode.ToString()} => {(ex as HttpRequestExceptionEx).Message} ######");
                return;
            }

            // Deafult
            Properties.Add("Message", ex.Message);
            Crashes.TrackError(ex, Properties);
            _logger.LogError($"##### ERROR => {ex.Message} ######");
        }

        #endregion

        #region Popup

        protected void PushPopupView<Tview>()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var view = App.ServiceProvider.GetService<Tview>() as PopupPage;
                await PopupNavigation.Instance.PushAsync(view);
            });
        }

        protected void PopupView()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                //var view = PopupNavigation.Instance.PopupStack.LastOrDefault();
                //await PopupNavigation.Instance.RemovePageAsync(view);
                await PopupNavigation.Instance.PopAllAsync();
            });
        }

        #endregion

        #region Loading

        protected void ShowLoading(string LoadMessage)
        {
            if (!isLoading)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    isLoading = true;
                    _loadingView = new LoadingView(LoadMessage);
                    await PopupNavigation.Instance.PushAsync(_loadingView);
                });
            }
        }

        protected void HideLoading()
        {
            Device.BeginInvokeOnMainThread(async () => await PopupNavigation.Instance.RemovePageAsync(_loadingView));
            isLoading = false;
        }

        #endregion


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
