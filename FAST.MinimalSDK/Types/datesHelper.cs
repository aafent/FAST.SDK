using Microsoft.VisualBasic;
using System.ComponentModel;
using System.Globalization;

namespace FAST.Types
{

    /// <summary>
    /// Helper class for manipulating date and time values.
    /// </summary
    public static class datesHelper 
    {

        /// <summary>
        /// get the datetime of the start of the week
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="startOfWeek"></param>
        /// <returns></returns>
        /// <example>
        /// DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
        /// DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Sunday);
        /// </example>
        /// <remarks>http://stackoverflow.com/a/38064/428061</remarks>
        public static System.DateTime startOfWeek( System.DateTime dt, DayOfWeek startOfWeek)
        {
            var diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
                diff += 7;

            return dt.AddDays(-1 * diff).Date;
        }

        /// <summary>
        /// get the datetime of the start of the month
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        /// <remarks>http://stackoverflow.com/a/5002582/428061</remarks>
        public static System.DateTime startOfMonth( System.DateTime dt) =>
            new System.DateTime(dt.Year, dt.Month, 1);


        /// <summary>
        /// get datetime of the start of the year
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static System.DateTime startOfYear( System.DateTime dt) => 
             new System.DateTime(dt.Year, 1, 1);
       /// <summary>
        /// get datetime of the start of the year
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static System.DateTime startOfYear( int year) => 
             new System.DateTime(year, 1, 1);

        /// <summary>
        /// get datetime of the end of the year
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static System.DateTime endOfYear( System.DateTime dt) => 
             new System.DateTime(dt.Year, 12, 31);

        /// <summary>
        /// get datetime of the end of the year
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static System.DateTime endOfYear( int year) => 
             new System.DateTime(year, 12, 31);


        /// <summary>
        /// get datetime of the end of month 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static System.DateTime endOfMonth(System.DateTime dt)
        {
            DateTime EOM;
            EOM = startOfMonth(dt);
            EOM=EOM.AddMonths(1);
            EOM=EOM.AddDays(-1);
            return EOM;
        }
             

        /// <summary>
        /// One day timespan
        /// </summary>
        /// <returns></returns>
        public static System.TimeSpan oneDay()
        {
            return new TimeSpan(1,0,0,0);
        }

        /// <summary>
        /// Get Orthodox easter for requested year
        /// </summary>
        /// <param name="year">Year of easter</param>
        /// <returns>DateTime of Orthodox Easter</returns>
        public static DateTime getOrthodoxEaster(int year)
        {
            var a = year % 19;
            var b = year % 7;
            var c = year % 4;

            var d = (19 * a + 16) % 30;
            var e = (2 * c + 4 * b + 6 * d) % 7;
            var f = (19 * a + 16) % 30;

            var key = f + e + 3;
            var month = (key > 30) ? 5 : 4;
            var day = (key > 30) ? key - 30 : key;

            return new DateTime(year, month, day);
        }

        /// <summary>
        /// Get Catholic easter for requested year
        /// </summary>
        /// <param name="year">Year of easter</param>
        /// <returns>DateTime of Catholic Easter</returns>
        public static DateTime getCatholicEaster(int year)
        {
            var month = 3;

            var a = year % 19 + 1;
            var b = year / 100 + 1;
            var c = (3 * b) / 4 - 12;
            var d = (8 * b + 5) / 25 - 5;
            var e = (5 * year) / 4 - c - 10;

            var f = (11 * a + 20 + d - c) % 30;
            if (f == 24)
                f++;
            if ((f == 25) && (a > 11))
                f++;

            var g = 44 - f;
            if (g < 21)
                g = g + 30;

            var day = (g + 7) - ((e + g) % 7);
            if (day > 31)
            {
                day = day - 31;
                month = 4;
            }
            return new DateTime(year, month, day);
        }


        /// <summary>
        /// Adds the given number of business days to the <see cref="DateTime"/>.
        /// </summary>
        /// <param name="current">The date to be changed.</param>
        /// <param name="days">Number of business days to be added.</param>
        /// <param name="holidays">An optional list of holiday (non-business) days to consider.</param>
        /// <returns>A <see cref="DateTime"/> increased by a given number of business days.</returns>
        public static DateTime addBusinessDays(DateTime current, int days, IEnumerable<DateTime> holidays = null)
        {
            var sign = Math.Sign(days);
            var unsignedDays = Math.Abs(days);
            for (var i = 0; i < unsignedDays; i++)
            {
                do
                {
                    current = current.AddDays(sign);
                }
                while (current.DayOfWeek == DayOfWeek.Saturday
                    || current.DayOfWeek == DayOfWeek.Sunday
                    || (holidays != null && holidays.Contains(current.Date))
                    );
            }
            return current;
        }
 
        /// <summary>
        /// Subtracts the given number of business days to the <see cref="DateTime"/>.
        /// </summary>
        /// <param name="current">The date to be changed.</param>
        /// <param name="days">Number of business days to be subtracted.</param>
        /// <param name="holidays">An optional list of holiday (non-business) days to consider.</param>
        /// <returns>A <see cref="DateTime"/> increased by a given number of business days.</returns>
        public static DateTime subtractBusinessDays(DateTime current, int days, IEnumerable<DateTime> holidays)
        {
            return addBusinessDays(current, -days, holidays);
        }
 
        /// <summary>
        /// Retrieves the number of business days from two dates
        /// </summary>
        /// <param name="startDate">The inclusive start date</param>
        /// <param name="endDate">The inclusive end date</param>
        /// <param name="holidays">An optional list of holiday (non-business) days to consider.</param>
        /// <returns></returns>
        public static int getBusinessDays(DateTime startDate, DateTime endDate,IEnumerable<DateTime> holidays)
        {
            if (startDate > endDate)
                throw new NotSupportedException("ERROR: [startDate] cannot be greater than [endDate].");
 
            int cnt = 0;
            for (var current = startDate; current < endDate; current = current.AddDays(1))
            {
                if (current.DayOfWeek == DayOfWeek.Saturday 
                    || current.DayOfWeek == DayOfWeek.Sunday 
                    || (holidays != null && holidays.Contains(current.Date)) 
                    ) 
                { 
                    // skip holiday 
                }
                else cnt++;
            }
            return cnt;
        }
 

        /// <summary>
        /// Converts the UNIX time stamp value to C# DateTime object.
        /// </summary>
        /// <param name="unixTimeStamp">The UNIX time stamp.</param>
        /// <returns></returns>
        public static DateTime unixTimeStampToDateTime(double unixTimeStamp)
        {
            var unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var unixTimeStampInTicks = (long)(unixTimeStamp * TimeSpan.TicksPerSecond);
            return new DateTime(unixStart.Ticks + unixTimeStampInTicks);
        }

        /// <summary>
        /// Converts the UNIX time stamp value to C# DateTime object.
        /// </summary>
        /// <param name="unixTimeStamp">The UNIX time stamp.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Invalid UNIX time step.</exception>
        public static DateTime unixTimeStampToDateTime(string unixTimeStamp)
        {
            if(string.IsNullOrEmpty(unixTimeStamp))
                throw new ArgumentException("Invalid UNIX time stamp.");
            var timeStampinDouble = double.Parse(unixTimeStamp);
            return unixTimeStampToDateTime(timeStampinDouble);
        }

        /// <summary>
        /// Converts the C# DateTime object to UNIX time stamp.
        /// </summary>
        /// <param name="dateTime">The DateTime object.</param>
        /// <returns></returns>
        public static string DateTimeToUnixTimeStamp(DateTime dateTime)
        {
            var unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var unixTimeStampInTicks = (dateTime - unixStart).Ticks;
            var ticks = unixTimeStampInTicks / TimeSpan.TicksPerSecond;
            return ticks.ToString(CultureInfo.InvariantCulture);
        }


        /// <summary>
        /// Gets the difference between two dates.
        /// </summary>
        /// <param name="date1">The date1.</param>
        /// <param name="date2">The date2.</param>
        /// <returns></returns>
        public static string getEnglishDifferenceBetweenDates(DateTime date1, DateTime date2)
        {
            if (DateTime.Compare(date1, date2) >= 0)
            {
                TimeSpan ts = date1.Subtract(date2);
                if (ts.Days == 0)
                {
                    if (ts.Hours == 0)
                        return string.Format("{0}  minutes", ts.Minutes);
                    return string.Format("{0} hours, {1} minutes",
                        ts.Hours, ts.Minutes);
                }
                if (ts.Hours == 0)
                    return string.Format("{0}  minutes", ts.Minutes);
                return string.Format("{0} days", ts.Days);
            }
            return "Unknown";
        }


                /// <summary>
        /// Gets the formated date time string.
        /// </summary>
        /// <param name="dateTime">The DateTime object.</param>
        /// <returns></returns>
        public static string getEnglishFormatedDateTimeString(DateTime dateTime)
        {
            try
            {
                DateTime dt1 = DateTime.UtcNow;
                TimeSpan ts;
                try
                {
                    ts = dt1 - dateTime;
                }
                catch (Exception)
                {
                    ts = dateTime - dt1;
                }
                string days = "";
                string hour = "";
                if (ts.TotalDays <= 1)
                {
                    if (ts.TotalHours > 1 && ts.TotalHours < 2)
                    {
                        days = Convert.ToInt32(ts.Hours) + " hr ";
                    }
                    else if (ts.TotalHours > 1)
                    {
                        days = Convert.ToInt32(ts.Hours) + " hrs ";
                    }
                    if (ts.Minutes <= 1)
                    {
                        hour = Convert.ToInt32(ts.Minutes) + " min ";
                    }
                    else if (ts.TotalMinutes > 1 && ts.TotalMinutes < 2)
                    {
                        hour = Convert.ToInt32(ts.Minutes) + " min ";
                    }
                    else
                    {
                        hour = Convert.ToInt32(ts.Minutes) + " mins ";
                    }
                }
                if (ts.TotalDays > 1)
                {
                    if (ts.TotalDays > 7)
                    {
                        int value = (dt1.Year - dateTime.Year) * 12 + dt1.Month - dateTime.Month;
                        if (value < 12)
                        {
                            days = dateTime.ToString("MMMM dd") + " ";
                        }
                        else
                        {
                            days = dateTime.ToString("MMMM dd yyyy") + "";
                        }
                    }
                    else
                    {
                        if (ts.Days > 1)
                        {
                            days = Convert.ToInt32(ts.Days) + " Days ";
                        }
                        else if (ts.Days <= 1)
                        {
                            days = Convert.ToInt32(ts.Days) + " Day ";
                        }
                        if (ts.Hours > 1)
                        {
                            hour = Convert.ToInt32(ts.Hours) + " hrs ";
                        }
                        else if (ts.TotalMinutes <= 1)
                        {
                            hour = Convert.ToInt32(ts.Minutes) + " min ";
                        }
                        else
                        {
                            hour = Convert.ToInt32(ts.Minutes) + " mins ";
                        }
                    }
                }
                string response;
                if (days.Contains("Day") || days.Contains("hr") || hour.Contains("Day") || hour.Contains("hr"))
                    response = days + hour + "ago";
                else if (!string.IsNullOrEmpty(days))
                    response = days;
                else
                    response = hour + "ago";
                return response;
            }
            catch (Exception)
            {
                return "";
            }
        }



        /// <summary>
        /// Gets the formated date time string.
        /// </summary>
        /// <param name="unixTimestamp">The Unix time stamp.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Null or empty string not allowed for time stamp.</exception>
        public static string getEnglishFormatedDateTimeString(string unixTimestamp)
        {
            if(string.IsNullOrEmpty(unixTimestamp))
                throw new ArgumentException("Null or empty string not allowed for time stamp.");
            double timeStampInDouble = double.Parse(unixTimestamp);
            DateTime dateTime = unixTimeStampToDateTime(timeStampInDouble);
            return getEnglishFormatedDateTimeString(dateTime);
        }


        /// <summary>
        /// Converts the UTC date time in time zone date time.
        /// </summary>
        /// <param name="utcDateTime">The UTC date time.</param>
        /// <param name="timeZoneInfo">The time zone information.</param>
        /// <returns></returns>
        public static DateTime ConvertDateTimeInTimeZone(DateTime utcDateTime, TimeZoneInfo timeZoneInfo)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZoneInfo);
        }

        /// <summary>
        /// Converts the UTC date time in time zone date time.
        /// TimeZoneId can be get from ReadOnlyCollection - TimeZoneInfo.GetSystemTimeZones()
        /// </summary>
        /// <param name="utcDateTime">The UTC date time.</param>
        /// <param name="timeZoneId">The time zone identifier string.</param>
        /// <returns></returns>
        public static DateTime ConvertDateTimeInTimeZone(DateTime utcDateTime, string timeZoneId)
        {
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return ConvertDateTimeInTimeZone(utcDateTime, timeZoneInfo);
        }


        /// <summary>
        /// Gets the dates between date range.
        /// </summary>
        /// <param name="startDate">The startDate.</param>
        /// <param name="endDate">The endDate.</param>
        /// <returns></returns>
        public static IEnumerable<DateTime> eachDay(DateTime startDate, DateTime endDate)
        {
            for (var day = startDate.Date; day.Date <= endDate.Date; day = day.AddDays(1))
                yield return day;
        }



        /// <summary>
        /// test if a date is between a date range.
        /// </summary>
        /// <param name="dt">The under test date</param>
        /// <param name="startDate">The startDate.</param>
        /// <param name="endDate">The endDate.</param>
        /// <param name="endDate">Optional, to compare or not the time. Default is no</param>
        /// <returns></returns>
        public static Boolean isBetween(this DateTime dt, DateTime startDate, DateTime endDate, Boolean compareTime = false)
        {
           return compareTime ?
              dt >= startDate && dt <= endDate :
              dt.Date >= startDate.Date && dt.Date <= endDate.Date;
        }





        public enum relativeToEasterHolidays:int
        {
            [Description("Easter")]
            easter=0, 
            [Description("Ascension")]
            aascension=39, 
            [Description("Pentecost")]
            pentecost=10, 
            [Description("Trinity")]
            trinity=7, 
            [Description("Corpus")]
            corpus=4
        }

        public enum weekDaysMondayFist:int 
        {
            [Description("Monday")]
            monday = 1,
            [Description("Tuesday")]
            tuesday =2,
            [Description("Wednesday")]
            wednesday = 3,
            [Description("Thursday")]
            thursday = 4,
            [Description("Friday")]
            friday = 5,
            [Description("Saturday")]
            saturday =6,
            [Description("Sunday")]
            sunday = 7
        }

        public enum weekDaysSundayFist:int 
        {
            [Description("Sunday")]
            sunday = 1,
            [Description("Monday")]
            monday = 2,
            [Description("Tuesday")]
            tuesday =3,
            [Description("Wednesday")]
            wednesday = 4,
            [Description("Thursday")]
            thursday = 5,
            [Description("Friday")]
            friday = 6,
            [Description("Saturday")]
            saturday =7,
        }



        public static int toInteger(TimeSpan time)
        {
            return int.Parse(time.Hours.ToString() + time.Minutes.ToString().PadLeft(2, '0') + time.Seconds.ToString().PadLeft(2, '0'));
        }

        public static short toShort(TimeSpan time)
        {
            return short.Parse(time.Hours.ToString() + time.Minutes.ToString().PadLeft(2, '0'));
        }

        public static string toHHMM(TimeSpan time)
        {
            return time.ToString("hh\\:mm");
        }

        public static string toHHMMSS(TimeSpan time)
        {
            return time.ToString("hh\\:mm\\:ss");
        }


        // (v) added 11/9/2023

        /// <summary>
        /// Calculate Ascencion day
        /// </summary>
        /// <param name="selectDate">Current date (but not before 1583)</param>
        /// <returns>DateTime</returns>
        /// <remarks>Ascencion day is always 10 days before Whit Sunday.</remarks>
        public static DateTime ascensionDay(DateTime selectDate)
        {
            return easterSunday(selectDate).AddDays(39);
        }

        /// <summary>
        /// Calculate Ash Wednesday
        /// </summary>
        /// <param name="selectDate">Current date (but not before 1583)</param>
        /// <returns>DateTime</returns>
        /// <remarks>Ash Wednesday marks the start of Lent. This is the 40 day period between before Easter</remarks>
        public static DateTime ashWednesday(DateTime selectDate)
        {
            return easterSunday(selectDate).AddDays(-46);
        }

        /// <summary>
        /// Get the first day of christmas
        /// </summary>
        /// <param name="selectDate">Date</param>
        /// <returns>DateTime</returns>
        /// <remarks>Is always on December 25.</remarks>
        public static DateTime christmasDay(DateTime selectDate)
        {
            return new DateTime(selectDate.Year, 12, 25);
        }

        /// <summary>
        /// Calculate the first Sunday of Advent
        /// </summary>
        /// <param name="selectDate">Current date (but not before 1583)</param>
        /// <returns>DateTime</returns>
        /// <remarks>The first sunday of advent is the first sunday at least 4 weeks before christmas</remarks>
        public static DateTime firstSundayOfAdvent(DateTime selectDate)
        {
            int weeks = 4;
            int correction = 0;
            DateTime christmas = christmasDay(selectDate);

            if (christmas.DayOfWeek != DayOfWeek.Sunday)
            {
                weeks--;
                correction = ((int)christmas.DayOfWeek - (int)DayOfWeek.Sunday);
            }

            return christmas.AddDays(-1 * ((weeks * 7) + correction));
        }

        /// <summary>
        /// Generate random DateTime between range
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>DateTime.</returns>
        public static DateTime getRandomDateTime(DateTime? min, DateTime? max = null)
        {
            min = min ?? new DateTime(1753, 01, 01);
            max = max ?? new DateTime(9999, 12, 31);

            var rnd = new Random();
            var range = max.Value - min.Value;
            var randomUpperBound = (Int32)range.TotalSeconds;
            if (randomUpperBound <= 0)
                randomUpperBound = rnd.Next(1, Int32.MaxValue);

            var randTimeSpan = TimeSpan.FromSeconds((Int64)(range.TotalSeconds - rnd.Next(0, randomUpperBound)));
            return min.Value.Add(randTimeSpan);
        }

        /// <summary>
        /// Calculate Good Friday
        /// </summary>
        /// <param name="selectDate">Current date (but not before 1583)</param>
        /// <returns>DateTime</returns>
        /// <remarks>Good Friday is the Friday before easter.</remarks>
        public static DateTime goodFriday(DateTime selectDate)
        {
            return easterSunday(selectDate).AddDays(-2);
        }

        /// <summary>
        /// Calculate Easter Sunday day
        /// </summary>
        /// <param name="selectDate">Current date (but not before 1583)</param>
        /// <returns>DateTime.</returns>
        public static DateTime easterSunday(DateTime selectDate)
        {
            int day = 0;
            int month = 0;
            int year = selectDate.Year;

            int g = year % 19;
            int c = year / 100;
            int h = (c - (int)(c / 4) - (int)((8 * c + 13) / 25) + 19 * g + 15) % 30;
            int i = h - (int)(h / 28) * (1 - (int)(h / 28) * (int)(29 / (h + 1)) * (int)((21 - g) / 11));

            day = i - ((year + (int)(year / 4) + i + 2 - c + (int)(c / 4)) % 7) + 28;
            month = 3;

            if (day > 31)
            {
                month++;
                day -= 31;
            }

            return new DateTime(year, month, day);
        }

        /// <summary>
        /// Calculate Palm Sunday
        /// </summary>
        /// <param name="selectDate">Current date (but not before 1583)</param>
        /// <returns>DateTime</returns>
        /// <remarks>Palm Sunday is the sunday one week before easter.</remarks>
        public static DateTime palmSunday(DateTime selectDate)
        {
            return easterSunday(selectDate).AddDays(-7);
        }

        /// <summary>
        /// Calculate Whit Sunday
        /// </summary>
        /// <param name="selectDate">Current date (but not before 1583)</param>
        /// <returns>DateTime</returns>
        /// <remarks>Whit Sunday is always 7 weeks after Easter</remarks>
        public static DateTime whitSunday(DateTime selectDate)
        {
            return easterSunday(selectDate).AddDays(49);
        }

        /// <summary>
        /// Dates the difference.
        /// </summary>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>System.Int32.</returns>
        public static double dateDiff(DateTime dtStart, DateTime dtEnd, DateInterval interval)
        {
            double rtn = 0;
            TimeSpan timespan = (dtStart - dtEnd);

            switch (interval)
            {
                case DateInterval.Day:
                    rtn = timespan.TotalDays;
                    break;

                case DateInterval.Hour:
                    rtn = timespan.TotalHours;
                    break;

                case DateInterval.Minute:
                    rtn = timespan.TotalMinutes;
                    break;

                case DateInterval.Second:
                    rtn = timespan.TotalSeconds;
                    break;
            }

            return rtn;
        }

        // (v) added 3 Feb 2025

        /// <summary>
        /// Convert input datetime to UTC format string
        /// </summary>
        /// <param name="dt">Input Date time</param>
        /// <returns>string, utc format</returns>
        public static String toUTCFormatString(DateTime dt)
        {
            return dt.ToUniversalTime().ToString("yyyyMMddHHmmssffffff");
        }
        /// <summary>
        /// Convert the current date and time to UTC format string
        /// </summary>
        /// <returns>string, utc format</returns>
        public static String toUTCFormatString()
        {
            return DateTime.UtcNow.ToString("yyyyMMddHHmmssffffff");
        }

        /// <summary>
        /// Convert a string to a nullable datetime using the try parse
        /// </summary>
        /// <param name="sValue">The input string with date time format</param>
        /// <param name="provider">Optional a format provider</param>
        /// <returns>The nullable datetime</returns>
        public static DateTime? toNullableDateTime(string sValue, IFormatProvider? provider=null)
        {
            DateTime dt;
            if (DateTime.TryParse(sValue, provider, out dt)) return dt;
            return null;
        }


        /// <summary>
        /// Return null if the input is minimum 
        /// </summary>
        /// <param name="dtValue">The input value</param>
        /// <returns>Datetime</returns>
        public static DateTime? nullIfMinimum(DateTime dtValue)
        {
            return (dtValue == DateTime.MinValue) ? (DateTime?)null : dtValue;
        }

        /// <summary>
        /// Return null if the input is minimum 
        /// </summary>
        /// <param name="dtValue">The input value</param>
        /// <returns>Datetime</returns>
        public static DateTime? nullIfMinimum(DateTime? dtValue)
        {
            if ( dtValue == null ) return null;
            return (dtValue == DateTime.MinValue) ? (DateTime?)null : dtValue;
        }

    }

}
