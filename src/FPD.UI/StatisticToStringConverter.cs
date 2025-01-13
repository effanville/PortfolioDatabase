using System;
using System.Globalization;
using System.Windows.Data;

using Effanville.FinancialStructures.Database.Statistics;

namespace Effanville.FPD.UI
{
    public sealed class StatisticToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Statistic enumValue = (Statistic)value;
            return enumValue.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Statistic value2 = Enum.Parse<Statistic>(value as string);
            return value2;
        }
    }
}
