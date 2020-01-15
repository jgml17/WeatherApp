using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Core.WeatherApp.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
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