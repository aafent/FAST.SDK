using FAST.Html.ColorCode;

namespace FAST.Html
{
    /// <summary>
    /// HTML Helper
    /// </summary>
    public static class htmlColorHelper
    {

        #region (+) Html color names
        public const string Blue = "#FF0000FF";
        public const string White = "#FFFFFFFF";
        public const string Black = "#FF000000";
        public const string DullRed = "#FFA31515";
        public const string Yellow = "#FFFFFF00";
        public const string Green = "#FF008000";
        public const string PowderBlue = "#FFB0E0E6";
        public const string Teal = "#FF008080";
        public const string Gray = "#FF808080";
        public const string Navy = "#FF000080";
        public const string OrangeRed = "#FFFF4500";
        public const string Purple = "#FF800080";
        public const string Red = "#FFFF0000";
        public const string MediumTurqoise = "FF48D1CC";
        public const string Magenta = "FFFF00FF";
        public const string OliveDrab = "#FF6B8E23";
        public const string DarkOliveGreen = "#FF556B2F";
        public const string DarkCyan = "#FF008B8B";
        public const string DarkOrange = "#FFFF8700";
        public const string BrightGreen = "#FF00d700";
        public const string BrightPurple = "#FFaf87ff";
        #endregion (+) Html color names

        /// <summary>
        /// Convert a color code string into html-style color
        /// </summary>
        /// <param name="color">The color</param>
        /// <returns>The html color</returns>
        public static string toHtmlColor(string color)
        {
            if (color == null) return null;

            var length = 6;
            var start = color.Length - length;
            return "#" + color.Substring(start, length);
        }

        public static string colorFormatByByLanguage(string source)
        {
            HtmlClassFormatter html = new HtmlClassFormatter();
            return html.GetHtmlString(source, Languages.CSharp);
        }
        
    }
}