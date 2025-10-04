using FAST.Types;
using System.Globalization;

namespace FAST.Files
{
    /*
     * (i) the inital code is base on a project named PrettyFormat. The project has been altered and migrated 
    */


    /// <summary>
    /// Format provider for file size values.
    /// <remarks>
    /// Specify the format "fs" in the format string, optionally followed by the file number format (default is F2)
    /// </remarks>
    /// </summary>
    public class fileSizeFormatProvider : IFormatProvider, ICustomFormatter
    {
        /// <summary>
        /// File size format specifier
        /// </summary>
        public static readonly string FileSizeFormat = "fs";

        /// <summary>
        /// Unit name used to build the string
        /// </summary>
        private readonly string[] _units;

        private readonly CultureInfo _cultureInfo;

        // <summary>
        /// Default ctor. Builds the format provider with the default unit names.
        /// <remarks>
        /// Default unit names in English are: <code>{"bytes", "KB", "MB", "GB", "TB"}</code>
        /// </remarks>
        public fileSizeFormatProvider()
          : this(null)
        {
        }

        /// <summary>
        /// Builds the format provider with the default unit names and a given inner format
        /// <remarks>
        /// Default unit names in English are: <code>{"bytes", "KB", "MB", "GB", "TB"}</code>
        /// </remarks>
        /// <param name="cultureInfo">a <see cref="CultureInfo"/></param>
        /// </summary>
        public fileSizeFormatProvider(CultureInfo cultureInfo)
        {
            _cultureInfo = cultureInfo ?? CultureInfo.CurrentUICulture;

            naturalFormats.Culture = _cultureInfo;
            _units = new[]
                       {
                   naturalFormats.FileSizeFormatProvider_bytes,
                   naturalFormats.FileSizeFormatProvider_KB,
                   naturalFormats.FileSizeFormatProvider_MB,
                   naturalFormats.FileSizeFormatProvider_GB,
                   naturalFormats.FileSizeFormatProvider_TB
                 };
        }

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
            if (formatType == typeof(ICustomFormatter))
                return this;
            return null;
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
            if (format == null || !format.StartsWith(FileSizeFormat, StringComparison.OrdinalIgnoreCase) || arg is string)
            {
                return DefaultFormat(format, arg);
            }

            // Convert the size in decimal
            decimal size;
            try
            {
                size = Convert.ToDecimal(arg, formatProvider);
            }
            catch (InvalidCastException)
            {
                return DefaultFormat(format, arg);
            }

            string suffix = null;
            foreach (var unit in _units)
            {
                if (size < 1024)
                {
                    suffix = unit;
                    break;
                }
                size /= 1024;
            }

            if (suffix == null)
            {
                suffix = _units[_units.Length - 1];
                size *= 1024;
            }

            var numberFormat = format.Substring(FileSizeFormat.Length);
            if (string.IsNullOrEmpty(numberFormat))
                numberFormat = "F2";

            var resultFormat = "{0:" + numberFormat + "} {1}";
            return string.Format(_cultureInfo, resultFormat, size, suffix);
        }

        private string DefaultFormat(string format, object arg)
        {
            var formattableArg = arg as IFormattable;
            if (formattableArg != null)
                return formattableArg.ToString(format, _cultureInfo);
            if (arg != null)
                return arg.ToString();
            return string.Empty;
        }
    }
}
