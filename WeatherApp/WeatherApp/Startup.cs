using Core.WeatherApp.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Essentials;
using Core.Services.WeatherAppServices.Interfaces;
using Core.Services.WeatherAppServices;
using Core.Models.WeatherAppModels;
using WeatherApp.ViewModels;
using Core.WeatherApp.Views.Popups;
using Core.WeatherApp.Views;
using Core.WeatherApp.ViewModels;

namespace Core.WeatherApp
{
    public static class Startup
    {
        public static App Init(Action<HostBuilderContext, IServiceCollection> nativeConfigureServices)
        {
            var systemDir = FileSystem.CacheDirectory;
            Utils.ExtractSaveResource("Core.WeatherApp.appsettings.json", systemDir);
            var fullConfig = Path.Combine(systemDir, "Core.WeatherApp.appsettings.json");

            var host = new HostBuilder()
                            .ConfigureHostConfiguration(c =>
                            {
                                c.AddCommandLine(new string[] { $"ContentRoot={FileSystem.AppDataDirectory}" });
                                c.AddJsonFile(fullConfig);
                            })
                            .ConfigureServices((c, x) =>
                            {
                                nativeConfigureServices(c, x);
                                ConfigureServices(c, x);
                            })
                            .ConfigureLogging(l => l.AddConsole(o =>
                            {
                                o.DisableColors = true;
                            }))
                            .Build();

            App.ServiceProvider = host.Services;

            return App.ServiceProvider.GetService<App>();
        }


        static void ConfigureServices(HostBuilderContext ctx, IServiceCollection services)
        {
            if (ctx.HostingEnvironment.IsDevelopment())
            {
                //var world = ctx.Configuration["Hello"];
            }

            // ViewModels 
            services.AddHttpClient();
            services.AddTransient<WeatherViewModel>();


            // Services
            services.AddTransient<IWeatherService, WeatherService>();

            // Pages
            services.AddTransient<WeatherView>();


            // Singletons
            services.AddSingleton<App>();
        }

    }

}
