using MovieList.Core.SharedObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MovieList.GUI.Converters
{
    public class MovieTypeImageConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                BitmapImage movieImage = new BitmapImage();
                movieImage.BeginInit();
                movieImage.CacheOption = BitmapCacheOption.OnLoad;
                movieImage.UriSource = new Uri(string.Format("pack://application:,,,/MovieList.GUI;component/Resources/{0}32x32.png", value), UriKind.RelativeOrAbsolute);
                movieImage.EndInit();
                if (movieImage.Width == 0)
                    return GetEmptyImage();
                return movieImage;
            }
            catch (Exception ex)
            {
                return GetEmptyImage();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private BitmapImage GetEmptyImage()
        {
            return new BitmapImage(new Uri("pack://application:,,,/MovieList.GUI;component/Resources/Other32x32.png", UriKind.RelativeOrAbsolute));
        }

        #endregion Methods
    }
}
