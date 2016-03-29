using System;
using System.Globalization;
using System.Windows.Data;
using Office365.User.Loader.Models;

namespace Office365.User.Loader.Converters
{
    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string response;
            switch ((OfficeUserStatus)value)
            {
                case OfficeUserStatus.NotLoaded:
                    response = "No Cargado";
                    break;
                case OfficeUserStatus.Loading:
                    response = "Cargando...";
                    break;
                case OfficeUserStatus.Loaded:
                    response = "Cargado";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
            return response;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}