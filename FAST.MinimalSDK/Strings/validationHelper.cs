namespace FAST.Strings
{
    public static class validationHelper
    {

        // (v) moved here on 29/11/2019 from converters.cs


        public static bool isNumeric(System.Object Expression, System.Globalization.NumberStyles style, IFormatProvider formatProvider)
        {
            if (Expression == null || Expression is DateTime)
            {
                return false;
            }

            if (Expression is Int16 || Expression is Int32 || Expression is Int64 || Expression is Decimal || Expression is Single || Expression is Double || Expression is Boolean)
            {
                return true;
            }

            double result = 0;
            return Double.TryParse(Expression as string, style, formatProvider, out result);
        }

        /// <summary>
        ///     IsNumeric checks if a string is a valid floating value
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Boolean True if isNumeric else False</returns>
        /// <remarks></remarks>
        public static bool isNumeric(string value) => int.TryParse(value, out int result);

        // (v) 29 nov 2019


        public static bool verifyRomanianCNP(string cnp, bool throwException=false)
        {
            string errorText = "";
            bool result=verifyRomanianCNP(cnp, out errorText);
            if (throwException)
            {
                throw new Exception(errorText);
            }
            return result;
        }
        public static bool verifyRomanianCNP(string cnp, out string errorText)
        {            
            const string cnpDigits = "279146358279";
            long month = 0;
            long day = 0;

            // (v) check the length
            if (cnp.Length != 13)
            {
                errorText = string.Format("Wrong length of {1} digits. CNP was {0}", cnp, cnp.Length);
                return false; // wrong length
            }

            // (v) check if it is convertable to a long number
            long cnpNumber = 0;
            if (!long.TryParse(cnp, out cnpNumber))
            {
                errorText = string.Format("Invalid character found. CNP was {0}", cnp);
                return false; // not a numeric string
            }



            // (v) calculate the check digit using the modulo 11 algorithm
            long total = 0;
            for (int i = 0;i < 12;i++)
            {
                long digit = long.Parse(cnp.Substring(i, 1));
                total += digit * long.Parse(cnpDigits.Substring(i, 1));

                switch (1 + i) // 1 is the first digit
                {
                    case 4:  // month 1st digit eg: var month = values[3] * 10 + values[4];
                        month = digit * (10);
                        break;
                    case 5: // month 2st digit
                        month += digit;
                        break;

                    case 6: // day of month, 1st digit eg: var day = values[5] * 10 + values[6];
                        day = digit * (10);
                        break;
                    case 7: // day of month, 2nd digit
                        day += digit;
                        break;
                }
            }
            long rest = total - 11 * (int)(total / 11);
            rest = rest == 10 ? 1 : rest;

            if (month > 12 | month < 1)
            {
                errorText = string.Format("Wrong month ({1}) found. CNP was {0}", cnp, month);
                return false; // wrong month
            }

            if (
                 (day < 1) 
                 | //OR
                 (
                    (month == 1 | month == 3 | month == 5 | month == 7 | month == 8 | month == 10 | month == 12) 
                    & //and
                    (day > 31)
                 ) 
                 | //OR
                 (
                    (month == 4 | month == 6 | month == 9 | month == 11 ) 
                    & //and
                    (day > 30)
                 ) 
                 | //OR
                 (
                    (month == 2 ) 
                    & //and
                    (day > 29) //(!) does not work for yeap years
                 ) 
               ) //if
            {
                errorText = string.Format("Wrong day of month ({1}) found. CNP was {0}", cnp, day);
                return false; // wrong day of the month
            }

            // (v) compare the check digit
            if (long.Parse(cnp.Substring(12, 1)) != rest)
            {
                errorText = string.Format("Incorrect checkdigit. CNP was {0}", cnp);
                return false; // wrong check digit
            }


            errorText = string.Empty;
            return true; // seems to have the correct form
        }

        // (v) 22/6/2021 : moved from FAST.Core.validators
        public static bool isGreekVATNumberValid(string VAT, bool permitLeadingLetters = true)
        {
            if (string.IsNullOrEmpty(VAT)) { return false; }
            if (permitLeadingLetters)
            {
                if (VAT.Length == 11)
                {
                    if (VAT.Substring(0, 2) != "EL")
                    {
                        return false;
                    }
                    else
                    {
                        VAT = VAT.Substring(2);
                    }
                }
            }
            if (VAT.Length == 8) { VAT = "0" + VAT; }
            if (VAT.Length != 9) return false;
            var digits = VAT.ToCharArray();
            int checkDigit = digits[8] - 48;
            long sum = ((digits[7] - 48) << 1) +
                ((digits[6] - 48) << 2) +
                ((digits[5] - 48) << 3) +
                ((digits[4] - 48) << 4) +
                ((digits[3] - 48) << 5) +
                ((digits[2] - 48) << 6) +
                ((digits[1] - 48) << 7) +
                ((digits[0] - 48) << 8);
            long mod = sum % 11;
            if (mod == 10)
                mod = 0;
            return (mod == checkDigit);
        }



        /// <summary>
        ///     Checks if the String contains only Unicode letters.
        ///     null will return false. An empty String ("") will return false.
        /// </summary>
        /// <param name="value">string to check if is Alpha</param>
        /// <returns>true if only contains letters, and is non-null</returns>
        public static bool isAlpha(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            return value.Trim().Replace(" ", "").All(Char.IsLetter);
        }

        /// <summary>
        ///     Checks if the String contains only Unicode letters, digits.
        ///     null will return false. An empty String ("") will return false.
        /// </summary>
        /// <param name="value">string to check if is Alpha or Numeric</param>
        /// <returns></returns>
        public static bool isAlphaNumeric(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            return value.Trim().Replace(" ", "").All(Char.IsLetterOrDigit);
        }



    }
}
