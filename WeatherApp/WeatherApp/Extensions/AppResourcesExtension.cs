using System;
using System.Reflection;
using System.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Core.WeatherApp.Extensions
{
    /// <summary>
    /// Classe responsavel por Resource
    /// </summary>
    [ContentProperty("Text")]
    public class AppResourcesExtension : IMarkupExtension
    {
        const string ResourceId = "Core.WeatherApp.Resources.AppResources";
        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
                return "";

            ResourceManager resmgr = new ResourceManager(ResourceId, typeof(AppResourcesExtension).GetTypeInfo().Assembly);

            var translation = resmgr.GetString(Text);

            if (translation == null)
            {
                translation = Text; // HACK: returns the key, which GETS DISPLAYED TO THE USER
            }
            return translation;
        }
    }
}
