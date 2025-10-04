using System.Globalization;

namespace FAST.Types
{

    /*
     * (i) the inital code is base on a project named PrettyFormat. The project has been altered and migrated 
    */

    /// <summary>
    /// It formats a <see cref="DateTime"/> in a pretty way, considering minutes to months.
    /// </summary>
    public class naturalTimeFormatProvider : IFormatProvider, ICustomFormatter
    {
        private readonly CultureInfo _cultureInfo;

        public const string FormatPretty = "P";

        /// <summary>
        /// Returns an object that provides formatting services for the specified type.
        /// </summary>
        /// <returns>
        /// An instance of the object specified by <paramref name="formatType"/>, if the <see cref="T:System.IFormatProvider"/> implementation can supply that type of object; otherwise, null.
        /// </returns>
        /// <param name="formatType">An object that specifies the type of format object to return. 
        ///                 </param><filterpriority>1</filterpriority>
        public object GetFormat(Type formatType)
        {
            if (typeof(ICustomFormatter) == formatType) return this;
            return null;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public naturalTimeFormatProvider() : this(null)
        {
        }

        /// <summary>
        /// Default constructor. Use this to override <see cref="CultureInfo.CurrentUICulture"/>
        /// </summary>
        /// <param name="cultureInfo">culture. If <value>null</value>, <see cref="CultureInfo.CurrentCulture"/> will be used</param>
        public naturalTimeFormatProvider(CultureInfo cultureInfo)
        {
            _cultureInfo = cultureInfo ?? CultureInfo.CurrentUICulture;
        }

        /// <summary>
        /// Converts the value of a specified object to an equivalent string representation using specified format and culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// The string representation of the value of <paramref name="arg"/>, formatted as specified by <paramref name="format"/> and <paramref name="formatProvider"/>.
        /// </returns>
        /// <param name="format">A format string containing formatting specifications. 
        ///                 </param><param name="arg">An object to format. 
        ///                 </param><param name="formatProvider">An <see cref="T:System.IFormatProvider"/> object that supplies format information about the current instance. 
        ///                 </param><filterpriority>2</filterpriority>
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            naturalFormats.Culture = _cultureInfo;
            if (arg == null)
                return string.Empty;

            if (format == null || !format.StartsWith(FormatPretty, StringComparison.OrdinalIgnoreCase))
                return DefaultFormat(format, arg);

            TimeSpan s;
            if ((arg is DateTime))
            {
                var date = (DateTime)arg;
                s = date.Subtract(DateTime.Now);
            }
            else if (arg is TimeSpan)
            {
                s = (TimeSpan)arg;
            }
            else
                return DefaultFormat(format, arg);

            var converter = s.TotalSeconds < 0
                              ? (Converter<LabelType, string>)GetLabelPast
                              : GetLabelFuture;

            return Format(s.Duration(), format, arg, converter);
        }

        private enum LabelType
        {
            Now,
            Minute1,
            Minutes,
            Hour1,
            Hours,
            Day1,
            Days,
            Weeks,
            Months,
        }

        private string Format(TimeSpan s, string format, object arg, Converter<LabelType, string> labels)
        {
            // past days
            var pastDays = (int)s.TotalDays;

            // past seconds
            var pastSeconds = (int)s.TotalSeconds;

            if (pastDays == 0)
            {
                if (pastSeconds < 60)
                {
                    return labels(LabelType.Now);
                }

                if (pastSeconds < 120)
                {
                    return labels(LabelType.Minute1);
                }

                if (pastSeconds < 3600)
                {
                    return string.Format(_cultureInfo,
                                         labels(LabelType.Minutes),
                                         Math.Floor((double)pastSeconds / 60));
                }
                // Less than 2 hours ago.
                if (pastSeconds < 7200)
                {
                    return labels(LabelType.Hour1);
                }
                // Less than one day ago.
                if (pastSeconds < 86400)
                {
                    return string.Format(_cultureInfo,
                                         labels(LabelType.Hours),
                                         Math.Floor((double)pastSeconds / 3600));
                }
            }

            if (pastDays == 1)
            {
                return labels(LabelType.Day1);
            }

            if (pastDays < 7)
            {
                return string.Format(_cultureInfo,
                                     labels(LabelType.Days),
                                     pastDays);
            }
            if (pastDays < 31)
            {
                return string.Format(_cultureInfo,
                                     labels(LabelType.Weeks),
                                     Math.Ceiling((double)pastDays / 7));
            }
            if (pastDays < 365)
            {
                return string.Format(_cultureInfo,
                                     labels(LabelType.Months),
                                     Math.Round(s.TotalDays / 30));
            }

            return DefaultFormat(format, arg);
        }

        private string GetLabelFuture(LabelType labelType)
        {
            switch (labelType)
            {
                case LabelType.Now:
                    return naturalFormats.FriendlyDateTime_just_now;
                case LabelType.Minute1:
                    return naturalFormats.FriendlyDateTime_next_minute;
                case LabelType.Minutes:
                    return naturalFormats.FriendlyDateTime_in_minutes;
                case LabelType.Hour1:
                    return naturalFormats.FriendlyDateTime_next_hour;
                case LabelType.Hours:
                    return naturalFormats.FriendlyDateTime_in_hours;
                case LabelType.Day1:
                    return naturalFormats.FriendlyDateTime_tomorrow;
                case LabelType.Days:
                    return naturalFormats.FriendlyDateTime_in_days;
                case LabelType.Weeks:
                    return naturalFormats.FriendlyDateTime_in_weeks;
                case LabelType.Months:
                    return naturalFormats.FriendlyDateTime_in_months;
                default:
                    throw new ArgumentOutOfRangeException("labelType");
            }
        }

        private string GetLabelPast(LabelType labelType)
        {
            switch (labelType)
            {
                case LabelType.Now:
                    return naturalFormats.FriendlyDateTime_just_now;
                case LabelType.Minute1:
                    return naturalFormats.FriendlyDateTime_1_minute_ago;
                case LabelType.Minutes:
                    return naturalFormats.FriendlyDateTime_minutes_ago;
                case LabelType.Hour1:
                    return naturalFormats.FriendlyDate_1_hour_ago;
                case LabelType.Hours:
                    return naturalFormats.FriendlyDateTime_hours_ago;
                case LabelType.Day1:
                    return naturalFormats.FriendlyDateTime_yesterday;
                case LabelType.Days:
                    return naturalFormats.FriendlyDateTime_days_ago;
                case LabelType.Weeks:
                    return naturalFormats.FriendlyDateTime_weeks_ago;
                case LabelType.Months:
                    return naturalFormats.FriendlyDateTime_months_ago;
                default:
                    throw new ArgumentOutOfRangeException("labelType");
            }
        }

        private string DefaultFormat(string format, object arg)
        {
            var formattableArg = arg as IFormattable;
            return formattableArg != null
              ? formattableArg.ToString(format, _cultureInfo)
              : arg.ToString();
        }
    }
}
