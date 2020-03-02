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
    public partial class WeatherView : ContentPage
    {
        private WeatherViewModel _viewmodel;

        /// <summary>
        /// Constructor with Dependency injection parameter
        /// </summary>
        public WeatherView(WeatherViewModel viewmodel)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            this.BindingContext = _viewmodel = viewmodel;

            // Animation Event fired from viewmodel
            _viewmodel.OnFireAnimation += async(info) =>
             {
                 await Animation(info);
             };
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

            // Call viewmodel  for infopanel refresh
            _viewmodel.SetInfoPanel(item);

            await weatherInfoPanel.RotateXTo(360, 350, Easing.SpringOut);
            if (Device.RuntimePlatform == Device.Android)
            {
                weatherInfoPanel.HasShadow = true;
            }
        }
    }
}
