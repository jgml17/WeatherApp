using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.WeatherApp.Interfaces
{
    public interface IBaseView
    {
        void ShowLoading(string LoadMessage);
        void HideLoading();
    }
}
