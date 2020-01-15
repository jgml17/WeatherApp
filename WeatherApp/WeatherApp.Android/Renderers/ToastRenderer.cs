using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Core.WeatherApp.Interfaces;

namespace WeatherApp.Droid.Renderers
{
    public class ToastRenderer : IToastRenderer
    {
        public void OpenToast(string text)
        {
            Toast.MakeText(Application.Context, text, ToastLength.Long).Show();
        }
    }
}