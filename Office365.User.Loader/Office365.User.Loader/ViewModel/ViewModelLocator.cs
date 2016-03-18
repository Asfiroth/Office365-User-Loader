using System.Windows.Navigation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;

namespace Office365.User.Loader.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            //if (!SimpleIoc.Default.IsRegistered<INavigationService>())
            //{
            //    var navigationService = CreateNavigationService();
            //    SimpleIoc.Default.Register(() => navigationService);
            //}

            //SimpleIoc.Default.Register<INavigationService>();

        }
        

        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}