namespace System
{
    public static class DateTimeExt
    {
        public static DateOnly ToDateOnly(this DateTime date)
        {
            return DateOnly.FromDateTime(date);
        }

        /// <summary>
        /// Converts time representations.
        /// </summary>
        /// <param name="timeStamp">
        /// The number of seconds since the Unix Epoch (Midnight, January 1st, 1970)
        /// </param>
        /// <returns>A structured date representation.</returns>
        public static DateTime UnixTimestampToDateTime(this double timeStamp)
        {
            return DateTime.UnixEpoch.AddSeconds(timeStamp).ToLocalTime();
        }

        /// <summary>
        /// Converts time representations.
        /// </summary>
        /// <param name="timeStamp">
        /// The number of seconds since the Unix Epoch (Midnight, January 1st, 1970)
        /// </param>
        /// <returns>A structured date representation.</returns>
        public static DateTime UnixTimestampToDateTime(this long timeStamp)
        {
            return UnixTimestampToDateTime((double)timeStamp);
        }

        /// <summary>
        /// Converts time representations.
        /// </summary>
        /// <param name="time">A structured date represntation.</param>
        /// <returns>
        /// The number of days since the Celestial Julian Day Calculation Epoch (Day 2451545 on the
        /// Julian Calendar).
        /// </returns>
        public static double ToJulianDays(this DateTime time)
        {
            const double JDEpoch = 2451545;
            // Julian Day
            var jdDate = new DateTime(time.Year, time.Month, time.Day, 12, 0, 0);
            if (time.Hour < 12)
            {
                jdDate = jdDate.AddDays(-1);
            }

            var delta = time - jdDate;

            var Y = jdDate.Year;
            var M = jdDate.Month;
            var D = jdDate.Day;

            // use integer division explicitly
            var JD = (1461 * (Y + 4800 + ((M - 14) / 12)) / 4)
                + (367 * (M - 2 - (12 * ((M - 14) / 12))) / 12)
                - (3 * ((Y + 4900 + ((M - 14) / 12)) / 100) / 4)
                + D - 32075;

            return JD + delta.TotalDays - JDEpoch;
        }

        public static DateTime Min(IEnumerable<DateTime?> dates)
        {
            var min = DateTime.MaxValue;
            foreach(var value in dates)
            {
                if(value is not null && value < min)
                {
                    min = value.Value;
                }
            }
            return min;
        }

        public static DateTime Min(params DateTime?[] dates) => 
            Min(dates.AsEnumerable());

        public static DateTime Max(IEnumerable<DateTime?> dates)
        {
            var max = DateTime.MinValue;
            foreach (var value in dates)
            {
                if (value is not null && value > max)
                {
                    max = value.Value;
                }
            }
            return max;
        }

        public static DateTime Max(params DateTime?[] dates) =>
            Max(dates.AsEnumerable());
    }
}