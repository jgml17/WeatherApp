
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Core.Models.WeatherAppModels
{
    /// <summary>
    /// DarkSky Model
    /// </summary>
    public class WeatherModel
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("timezone")]
        public string Timezone { get; set; }

        [JsonPropertyName("currently")]
        public Currently Currently { get; set; }

        [JsonPropertyName("daily")]
        public Daily Daily { get; set; }

        [JsonPropertyName("offset")]
        public long Offset { get; set; }
    }

    /// <summary>
    /// Currently
    /// </summary>
    public partial class Currently
    {
        [JsonPropertyName("time")]
        public long Time { get; set; }

        [JsonPropertyName("summary")]
        public string Summary { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }
        
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }

        [JsonPropertyName("humidity")]
        public double Humidity { get; set; }

        [JsonPropertyName("pressure")]
        public double Pressure { get; set; }

        [JsonPropertyName("windSpeed")]
        public double WindSpeed { get; set; }

        [JsonPropertyName("cloudCover")]
        public double CloudCover { get; set; }

        public string IconFormatted { get => IconFormat.FormatIcon(Icon); }
    }

    /// <summary>
    /// Diario
    /// </summary>
    public partial class Daily
    {
        [JsonPropertyName("summary")]
        public string Summary { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonPropertyName("data")]
        public List<DailyInfos> Data { get; set; }
    }

    /// <summary>
    /// Diario Infos
    /// </summary>
    public partial class DailyInfos
    {
        [JsonPropertyName("time")]
        public long Time { get; set; }

        [JsonPropertyName("summary")]
        public string Summary { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonPropertyName("temperatureHigh")]
        public double TemperatureHigh { get; set; }

        [JsonPropertyName("humidity")]
        public double Humidity { get; set; }

        [JsonPropertyName("pressure")]
        public double Pressure { get; set; }

        [JsonPropertyName("windSpeed")]
        public double WindSpeed { get; set; }

        [JsonPropertyName("cloudCover")]
        public double CloudCover { get; set; }

        public string IconFormatted { get => IconFormat.FormatIcon(Icon);  }
    }


    public static class IconFormat
    {
        public static string FormatIcon(string icon)
        {
            string ret = string.Empty;

            switch (icon)
            {
                case "partly-cloudy-day":
                    ret = "cloudy_1.png";
                    break;

                case "cloudy":
                    ret = "cloudy.png";
                    break;

                case "rain":
                    ret = "rain.png";
                    break;

                // How I have not all icons... sun will be default
                default:
                    ret = "sun.png";
                    break;
            }

            return ret;
        }
    }
}
