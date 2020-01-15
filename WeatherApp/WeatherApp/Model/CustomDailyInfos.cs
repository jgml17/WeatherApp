using System;
using Core.Models.WeatherAppModels;
using Core.WeatherApp.Helpers;

namespace Core.WeatherApp.Model
{
    /// <summary>
    /// Add Date and Temperature format to original model
    /// </summary>
    public class CustomDailyInfos : DailyInfos
    {
        public string Date { get => UnixDateTime.UnixTimeStampToDateTimeShort(Time); }
        public string Temp { get => TemperatureHigh.ToString("##"); }
    }
}
