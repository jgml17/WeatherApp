using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
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
    public partial class ToastMessageView : PopupPage
    {
        public ToastMessageView(string HexColor, string Message)
        {
            InitializeComponent();

            ToastField.BackgroundColor = Color.FromHex(HexColor);
            lblToastMessage.Text = Message;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            HidePopup();
        }

        private async void HidePopup()
        {
            await Task.Delay(4000);

            if (PopupNavigation.Instance.PopupStack.Contains(this))
                await PopupNavigation.Instance.RemovePageAsync(this);
        }
    }
}