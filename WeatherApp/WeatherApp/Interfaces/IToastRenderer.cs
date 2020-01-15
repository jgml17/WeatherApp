using System;
using System.Collections.Generic;
using System.Text;

namespace Core.WeatherApp.Interfaces
{
    public interface IToastRenderer
    {
        void OpenToast(string text);
    }
}
