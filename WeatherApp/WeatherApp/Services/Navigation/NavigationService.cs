using Core.WeatherApp.ViewModels;
using Core.WeatherApp.Views;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.ViewModels;
using Xamarin.Forms;

namespace Core.WeatherApp.Services.Navigation
{
    public class NavigationService<TViewModel> : INavigationService<TViewModel>
    {
        public BaseViewModel<TViewModel> PreviousPageViewModel
        {
            get
            {
                var mainPage = Application.Current.MainPage as NavigationView;
                var viewModel = mainPage.Navigation.NavigationStack[mainPage.Navigation.NavigationStack.Count - 2].BindingContext;
                return viewModel as BaseViewModel<TViewModel>;
            }
        }

        public Task InitializeAsync()
        {
            //if (string.IsNullOrEmpty(_settingsService.AuthAccessToken))
            //    return NavigateToAsync<LoginViewModel>();
            //else

            return NavigateToAsync<WeatherViewModel>();
        }

        public Task NavigateToAsync<T>() where T : BaseViewModel<T>
        {
            return InternalNavigateToAsync<T>(typeof(T), null);
        }

        public Task NavigateToAsync<T>(object parameter) where T : BaseViewModel<T>
        {
            return InternalNavigateToAsync<T>(typeof(T), parameter);
        }

        public Task RemoveLastFromBackStackAsync()
        {
            var mainPage = Application.Current.MainPage as NavigationView;

            if (mainPage != null)
            {
                mainPage.Navigation.RemovePage(mainPage.Navigation.NavigationStack[mainPage.Navigation.NavigationStack.Count - 2]);
            }

            return Task.FromResult(true);
        }

        public Task RemoveBackStackAsync()
        {
            var mainPage = Application.Current.MainPage as NavigationView;

            if (mainPage != null)
            {
                for (int i = 0; i < mainPage.Navigation.NavigationStack.Count - 1; i++)
                {
                    var page = mainPage.Navigation.NavigationStack[i];
                    mainPage.Navigation.RemovePage(page);
                }
            }

            return Task.FromResult(true);
        }

        private async Task InternalNavigateToAsync<T>(Type viewModelType, object parameter)
        {
            Page page = CreatePage(viewModelType, parameter);

            //if (page is LoginView)
            //{
            //    Application.Current.MainPage = new NavigationView(page);
            //}
            //else
            {
                // If not master detail page -> invert comments !!! ===================================================
                var navigationPage = Application.Current.MainPage as NavigationView;
                //var navigationPage = (Application.Current.MainPage as MainPageView).Detail as NavigationView;
                // =============================================================================================

                if (navigationPage != null)
                {
                    await navigationPage.PushAsync(page);
                }
                else
                {
                    Application.Current.MainPage = new NavigationView(page);
                }
            }

            // Initializing
            (page.BindingContext as BaseViewModel<T>).Initialize(parameter);
            await (page.BindingContext as BaseViewModel<T>).InitializeAsync(parameter);
        }

        private Type GetPageTypeForViewModel(Type viewModelType)
        {
            var viewName = viewModelType.FullName.Replace("Model", string.Empty);
            var viewModelAssemblyName = viewModelType.GetTypeInfo().Assembly.FullName;
            var viewAssemblyName = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", viewName, viewModelAssemblyName);
            var viewType = Type.GetType(viewAssemblyName);
            return viewType;
        }

        private Page CreatePage(Type viewModelType, object parameter)
        {
            Type pageType = GetPageTypeForViewModel(viewModelType);
            if (pageType == null)
            {
                throw new Exception($"Cannot locate page type for {viewModelType}");
            }

            //old - Page page = Activator.CreateInstance(pageType) as Page;
            Page page = App.ServiceProvider.GetService(pageType) as Page;

            return page;
        }
    }

}
