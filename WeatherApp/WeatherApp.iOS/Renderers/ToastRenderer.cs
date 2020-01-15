using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.WeatherApp.Interfaces;
using Foundation;
using UIKit;

namespace WeatherApp.iOS.Renderers
{
    public class ToastRenderer : IToastRenderer
    {
        public void OpenToast(string text)
        {
            var vc = UIApplication.SharedApplication.KeyWindow.RootViewController;
            var okAlert = UIAlertController.Create(string.Empty, text, UIAlertControllerStyle.Alert);
            okAlert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            vc.PresentViewController(okAlert, true, null);
        }
    }
}