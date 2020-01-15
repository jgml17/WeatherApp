using Android.OS;
using Android.App;

namespace WeatherApp.Droid
{
    //[Activity(Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = false)]
    [Activity(Theme = "@style/Theme.Splash", MainLauncher = true,  NoHistory = true, HardwareAccelerated =true,ClearTaskOnLaunch =true)]
    public class Splashscreen : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            StartActivity(typeof(MainActivity));
        }
    }
}