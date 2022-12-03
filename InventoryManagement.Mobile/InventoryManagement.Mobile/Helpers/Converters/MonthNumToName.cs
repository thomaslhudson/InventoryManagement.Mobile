using System;
using System.Globalization;
using Xamarin.Forms;

namespace InventoryManagement.Mobile.Helpers.Converters
{
    /// <summary>
    /// <c>MonthNumToName</c> Binding Value Converter to convert a month number to month name; 
    /// available and used when binding occurs
    /// </summary>
    public class MonthNumToName : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            try
            {
                var intMonth = System.Convert.ToInt32(value);
                //if (Enumerable.Range(1, 12).Contains(intMonth))
                if (intMonth >= 1 && intMonth <= 12)
                {
                    return cultureInfo.DateTimeFormat.GetAbbreviatedMonthName(intMonth);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strMonth)
            {
                return DateTime.ParseExact(strMonth, "MMM", CultureInfo.InvariantCulture).Month;
            }

            return value;
        }
    }
}
