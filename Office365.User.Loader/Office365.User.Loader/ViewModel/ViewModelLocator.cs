using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using Office365.User.Loader.Services;
using Office365.User.Loader.Views;

namespace Office365.User.Loader.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            if (!SimpleIoc.Default.IsRegistered<ILoaderNavigationService>())
            {
                var navigationService = CreateNavigationService();
                SimpleIoc.Default.Register(() => navigationService);
            }

            SimpleIoc.Default.Register<LoadPageVm>();

        }

        private ILoaderNavigationService CreateNavigationService()
        {
            var navigationService = new NavigationService();
            navigationService.Configure("Results", new Uri("/Views/ResultsView.xaml", UriKind.Relative));
            return navigationService;
        }

        public LoadPageVm LoadPageViewModel => ServiceLocator.Current.GetInstance<LoadPageVm>();
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}