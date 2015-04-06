using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace FilePost.Converters
{
    public class ItemCountConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ObservableCollection<FPFile> list = (ObservableCollection<FPFile>)value;
            return string.Format("Files: {0}", list.Count);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
