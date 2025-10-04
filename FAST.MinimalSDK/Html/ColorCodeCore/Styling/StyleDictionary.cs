// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.ObjectModel;

namespace FAST.Html.ColorCode.Styling
{
    /// <summary>
    /// A dictionary of <see cref="Style" /> instances, keyed by the styles' scope name.
    /// </summary>
    internal /*public*/ partial class StyleDictionary : KeyedCollection<string, Style>
    {
        /// <summary>
        /// When implemented in a derived class, extracts the key from the specified element.
        /// </summary>
        /// <param name="item">The element from which to extract the key.</param>
        /// <returns>The key for the specified element.</returns>
        protected override string GetKeyForItem(Style item)
        {
            return item.ScopeName;
        }

        private const string Blue = "#FF0000FF";
        private const string White = "#FFFFFFFF";
        private const string Black = "#FF000000";
        private const string DullRed = "#FFA31515";
        private const string Yellow = "#FFFFFF00";
        private const string Green = "#FF008000";
        private const string PowderBlue = "#FFB0E0E6";
        private const string Teal = "#FF008080";
        private const string Gray = "#FF808080";
        private const string Navy = "#FF000080";
        private const string OrangeRed = "#FFFF4500";
        private const string Purple = "#FF800080";
        private const string Red = "#FFFF0000";
        private const string MediumTurqoise = "FF48D1CC";
        private const string Magenta = "FFFF00FF";
        private const string OliveDrab = "#FF6B8E23";
        private const string DarkOliveGreen = "#FF556B2F";
        private const string DarkCyan = "#FF008B8B";
        private const string DarkOrange = "#FFFF8700";
        private const string BrightGreen = "#FF00d700";
        private const string BrightPurple = "#FFaf87ff";

    }
}