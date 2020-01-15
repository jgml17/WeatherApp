using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.WeatherApp.Helpers;
using Core.WeatherApp.Interfaces;
using Core.WeatherApp.Model;
using Core.WeatherApp.ViewModels;
using Core.WeatherApp.ViewModels.Popups;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

namespace Core.WeatherApp.Views
{
    /// <summary>
    /// View Class: Here we have the code only to belongs to view.
    /// </summary>
    public partial class WeatherView : ContentPage, IBaseView
    {
        private WeatherViewModel _viewmodel;
        private LoadingView _loadingView;

        /// <summary>
        /// Constructor with Dependency injection parameter
        /// </summary>
        public WeatherView(WeatherViewModel viewmodel)
        {
            InitializeComponent();

            this.BindingContext = _viewmodel = viewmodel;
        }

        /// <summary>
        /// Show Event
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Create Event on showing view
            _viewmodel.OnViewActions += _viewmodel_OnViewActions;

            // Load infos
            _ = _viewmodel.LoadDailyWeather();

            // Call Timer
            _viewmodel.Timer();
        }

        /// <summary>
        /// Events fired from ViewModel to View Processing
        /// </summary>
        /// <param name="action">Action Type</param>
        /// <param name="response">Object to come with</param>
        private void _viewmodel_OnViewActions(ViewActions action, object response = null)
        {
            switch (action)
            {
                case ViewActions.Loading:
                    // Loading
                    if (!_viewmodel.IsBusy)
                        HideLoading();
                    else
                        ShowLoading(response as string);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Close Event
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Remove Event from memory on closing view
            _viewmodel.OnViewActions -= _viewmodel_OnViewActions;
        }

        /// <summary>
        /// Get item info tapped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void WeatherForecastList_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            CustomDailyInfos item = e.Item as CustomDailyInfos;

            await Animation(item);

        }

        /// <summary>
        /// Starts animation
        /// </summary>
        /// <returns></returns>
        private async Task Animation(CustomDailyInfos item)
        {
            // call Animation
            if (Device.RuntimePlatform == Device.Android)
            {
                weatherInfoPanel.HasShadow = false;
            }
            weatherInfoPanel.RotationX = 0;
            await weatherInfoPanel.RotateXTo(180, 350, Easing.SpringIn);

            _viewmodel.SetInfoPanel(item);

            await weatherInfoPanel.RotateXTo(360, 350, Easing.SpringOut);
            if (Device.RuntimePlatform == Device.Android)
            {
                weatherInfoPanel.HasShadow = true;
            }
        }

        /// <summary>
        /// Show Loading view
        /// </summary>
        /// <param name="LoadMessage"></param>
        public void ShowLoading(string LoadMessage)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _loadingView = new LoadingView(LoadMessage);
                await Navigation.PushPopupAsync(_loadingView);
            });
        }

        /// <summary>
        /// Close Loading view
        /// </summary>
        public void HideLoading()
        {
            Device.BeginInvokeOnMainThread(async () => await Navigation.RemovePopupPageAsync(_loadingView));
        }

        /// <summary>
        /// Returns Info Panel to current date
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            await Animation(_viewmodel.OriginalDailyInfo);
        }
    }
}
