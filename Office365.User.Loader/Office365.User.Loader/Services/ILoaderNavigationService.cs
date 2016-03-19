using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Views;

namespace Office365.User.Loader.Services
{
    public interface ILoaderNavigationService : INavigationService
    {
        object Parameter { get; }
    }
}
