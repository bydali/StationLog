using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
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
                s += item.Checkee + ",";
            }
            return s;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class TargetReceived : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var target = ((ObservableCollection<Cmd2Station>)value).
                Where(i => i.IsSelected == true &&
                i.Name == ConfigurationManager.ConnectionStrings["Station"].ConnectionString).First();
            if (target.IsChecked)
            {
                return Brushes.Black;
            }
            else
            {
                return Brushes.Red;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
