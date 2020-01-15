using System;
namespace Core.WeatherApp.Helpers
{
    public static class UnixDateTime
    {
        /// <summary>
        /// Unix TimeStamp Transformation Long 
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public static string UnixTimeStampToDateTimeLong(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return $"{dtDateTime.ToLongDateString()}, {dtDateTime.ToLongTimeString()}";
        }

        /// <summary>
        /// Unix TimeStamp Transformation Short 
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public static string UnixTimeStampToDateTimeShort(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return $"{dtDateTime.ToString("dddd")} {dtDateTime.Day}";
        }
    }
}
