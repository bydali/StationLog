using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using YDMSG;

namespace StationLog
{
    class TargetsStation : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var target = ((ObservableCollection<Cmd2Station>)value).Where(i => i.IsSelected == true);
            var s = "";
            foreach (var item in target)
            {
                s += item.Name + ",";
            }
            return s;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class TargetSignee : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var target = ((ObservableCollection<Cmd2Station>)value).
                Where(i => i.IsSelected == true &&
                i.Name == ConfigurationManager.ConnectionStrings["Station"].ConnectionString);
            var s = "";
            foreach (var item in target)
            {
                s += item.Signee + ",";
            }
            return s;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
