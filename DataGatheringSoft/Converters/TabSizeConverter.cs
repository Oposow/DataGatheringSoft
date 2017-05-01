using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace DataGatheringSoft.Converters
{
    public class TabSizeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            MetroAnimatedTabControl tabControl = values[0] as MetroAnimatedTabControl;
            double width = tabControl.ActualWidth / tabControl.Items.Count;
            //Subtract 1, otherwise we could overflow to two rows.
            return (width <= 1) ? 0 : (width - 1);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
