using System;
using System.Globalization;
using System.Linq;

namespace InventoryManagement.Mobile.Helpers.Converters
{
    /// <summary>
    /// <c>Dates:</c> Methods for manipulating dates
    /// </summary>
    public class Dates
    {
        /// <summary>
        /// Gets the abbreviated name of a month based on InvariantCulture
        /// </summary>
        /// <param name="month">Integer representing the month</param>
        /// <returns>
        /// The abbreviated name for the month (e.g. Jan)
        /// <para>
        /// Returns an empty string for integers &lt;1 or  &gt;12
        /// </para>
        /// </returns>
        public static string GetMonthName(int month)
        {
            if (month >= 1 && month <= 12)
            {
                return CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(month);
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the integer of a month based on InvariantCulture
        /// </summary>
        /// <param name="month">String representing the month; can be full or abbreviated name (e.g. 'Jan'/'January')</param>
        /// <returns>
        /// An integer representing the month
        /// <para>
        /// Returns null for non-month names
        /// </para>
        /// </returns>
        public static int? GetMonthInt(string month)
        {
            try
            {
                return DateTime.ParseExact(month, "MMM", CultureInfo.InvariantCulture).Month;
            }
            catch
            {
                return null;
            }
        }
    }
}
