namespace FAST.Data
{
    /// <summary>
    /// Helper class with methods and AddOns to the Data Structures 
    /// </summary>
    public static class dataMappingHelper
    {
        /// <summary>
        /// Cast a string value to given type
        /// </summary>
        /// <param name="toType">The type to return</param>
        /// <param name="value">The string value to be casted</param>
        /// <returns>The value casted to the requested type</returns>
        public static object castValue(Type toType, string value)
        {
            if (value == null) { return null; }
            else if (toType == typeof(string)) { return value; }

            else if (toType == typeof(int)) { return int.Parse(value); }
            else if (toType == typeof(Int16)) { return Int16.Parse(value); }
            else if (toType == typeof(Int32)) { return Int32.Parse(value); }
            else if (toType == typeof(Int64)) { return Int64.Parse(value); }
            else if (toType == typeof(sbyte)) { return sbyte.Parse(value); }
            else if (toType == typeof(Byte)) { return Byte.Parse(value); }
            else if (toType == typeof(long)) { return long.Parse(value); }
            else if (toType == typeof(double)) { return double.Parse(value); }
            else if (toType == typeof(Decimal)) { return Decimal.Parse(value); }

            else if (toType == typeof(Boolean)) { return Boolean.Parse(value); }
            else if (toType == typeof(Enum)) { return Enum.Parse(toType, value); }

            else if (toType == typeof(DateTime)) { return DateTime.Parse(value); }

            return value;
        }

        /// <summary>
        /// Add all AddOns to the collection
        /// </summary>
        /// <param name="addOns">The collection
        /// </param>
        public static void allAddOns(IdataMappingAddOnsCollection addOns)
        {
            if (!addOns.addOns.ContainsKey("letters")) addOns.add("letters",lettersOnly);
            if (!addOns.addOns.ContainsKey("numbers")) addOns.add("numbers", numbersOnly);
            if (!addOns.addOns.ContainsKey("first")) addOns.add("first", firstCharacters);
            if (!addOns.addOns.ContainsKey("reverse")) addOns.add("reverse", reverse);

            return;
        }

        /// <summary>
        /// Add-Ons Rule: Keep only letters
        /// </summary>
        /// <param name="args">N/A</param>
        /// <param name="input">The input value</param>
        /// <returns>The output value</returns>
        public static object lettersOnly(string args, object input)
        {
            var tmps = Strings.stringParsingHelper.keepLetters(input.ToString(), "");
            return castValue(input.GetType(), tmps);
        }

        /// <summary>
        /// Add-Ons Rule: Keep only numbers
        /// </summary>
        /// <param name="args">N/A</param>
        /// <param name="input">The input value</param>
        /// <returns>The output value</returns>
        public static object numbersOnly(string args, object input)
        {
            var tmps = Strings.stringParsingHelper.keepNumbers(input.ToString(), "");
            return castValue(input.GetType(), tmps);
        }


        /// <summary>
        /// Add-Ons Rule: Keep the N first characters
        /// </summary>
        /// <param name="args">Number, the number of characters to keep</param>
        /// <param name="input">The input value</param>
        /// <returns>The output value</returns>
        public static object firstCharacters(string args, object input)
        { 
                // Keep the first 10 characters
                // syntax: first.10   
                var len = int.Parse(input.ToString());
                if (len<1 ) return input;
                return input.ToString().Substring(len - 1);
        }

        /// <summary>
        /// Add-Ons Rule: Reverse the string (LTR)
        /// </summary>
        /// <param name="args">N/A</param>
        /// <param name="input">The input value</param>
        /// <returns>The output value</returns>
        public static object reverse(string args, object input)
        {
            return Strings.stringsHelper.reverse(input.ToString());
        }

    }
}
