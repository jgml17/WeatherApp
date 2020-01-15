using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace Core.WeatherApp.ViewModels.Popups
{
    public partial class LoadingView : PopupPage
    {
        public LoadingView(string LoadMessage = null)
        {
            InitializeComponent();

            if (!string.IsNullOrEmpty(LoadMessage))
            {
                lblMessage.Text = LoadMessage;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
