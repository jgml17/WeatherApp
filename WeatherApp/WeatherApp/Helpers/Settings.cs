using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Core.WeatherApp.Helpers
{
    public static class Settings<T>
    {
        private static ISettings AppSettings => CrossSettings.Current;

        public static T GetValue(string key)
        {
            string ret = AppSettings.GetValueOrDefault(key, string.Empty);
            return string.IsNullOrEmpty(ret) ? default : JsonSerializer.Deserialize<T>(ret);
        }

        public static void SetValue(string key, T value)
        {
            AppSettings.AddOrUpdateValue(key, JsonSerializer.Serialize(value));
        }
    }  

}