﻿using Xamarin.Forms;
using Core.WeatherApp.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using Core.WeatherApp.Helpers;

namespace Core.WeatherApp
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; set; }
        // TODO
        private const string AppCenterAndroid = "TODO";
        private const string AppCenteriOS = "TODO";

        public App()
        {
            InitializeComponent();

            MainPage = ServiceProvider.GetService<WeatherView>();
        }

        protected override void OnStart()
        {
            // Handle when your app starts

            AppCenter.Start($"android={AppCenterAndroid};ios={AppCenteriOS}",
                typeof(Analytics), 
                typeof(Crashes), 
                typeof(Distribute));
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
