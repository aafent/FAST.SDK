using System.Text;
using System.Net;
using System.Globalization;

namespace FAST.Strings
{
    public class converters
    {
        public static string uriToString(string url, string fileNameToStore = "")
        {
            return uriToString(new Uri(url), fileNameToStore);
        }
        public static string uriToString(Uri uri, string fileNameToStore = "")
        {
            if (string.IsNullOrEmpty(fileNameToStore))
            {
                fileNameToStore = System.IO.Path.GetTempFileName();
            }
            new WebClient().DownloadFile(uri, fileNameToStore);
            return fileToString(fileNameToStore, false);
        }

        public static string fileToString(string fileName, bool useAppendLine, string encodingName = "")
        {
            Encoding fileEncoding;
            if (String.IsNullOrEmpty(encodingName))
            {
                fileEncoding = Encoding.Default;
            }
            else
            {
                fileEncoding = Encoding.GetEncoding(encodingName);
            }
            return fileToString(fileName, useAppendLine, fileEncoding);
        }
        public static string fileToString(string fileName, bool useAppendLine, Encoding fileEncoding)
        {
            StringBuilder toReturn = new StringBuilder();
            string line = string.Empty;

            FileStreamOptions fso = new FileStreamOptions()
            {
                Mode = FileMode.Open,
                Access = FileAccess.Read,
                Share = FileShare.None
            };
            using (StreamReader sr = new StreamReader(fileName, fileEncoding, false, fso))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    if (useAppendLine)
                    {
                        toReturn.AppendLine(line);
                    }
                    else
                    {
                        toReturn.Append(line);
                    }
                }
            }
            return toReturn.ToString();
        }

        public static void stringToFile(string fileName, string content)
        {
            FileStreamOptions fso = new FileStreamOptions()
            {
                Mode = FileMode.Create,
                Access = FileAccess.Write,
                Share = FileShare.ReadWrite
            };
            using (System.IO.TextWriter writer = new System.IO.StreamWriter(fileName, fso))
            {
                writer.Write(content.ToString());
                writer.Flush();
                writer.Close();
            }
            return;
        }
        public static void stringToFile(string fileName, string content, Encoding encoding)
        {
            stringToFile(fileName, content, false, encoding);
            return;
        }
        public static void stringToFile(string fileName, string content, bool append, Encoding encoding)
        {
            FileStreamOptions fso = new FileStreamOptions()
            {
                Mode = FileMode.Append,
                Access = FileAccess.Write,
                Share = FileShare.ReadWrite
            };
            if ( !append ) fso.Mode = FileMode.Create;
            using (System.IO.TextWriter writer = new System.IO.StreamWriter(fileName, encoding, fso))
            {
                writer.Write(content.ToString());
                writer.Flush();
                writer.Close();
            }
            return;
        }

        public static Stream generateStreamFromString(string input)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(input);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static void stringToFileAsByteArray(string fileName, string content)
        {
            Encoding encoding = Encoding.GetEncoding("windows-1253");
            FileStreamOptions fso = new FileStreamOptions()
            {
                Mode = FileMode.Create,
                Access = FileAccess.Write,
                Share = FileShare.ReadWrite
            };
            using (System.IO.TextWriter writer = new System.IO.StreamWriter(fileName, encoding, fso ))
            {
                writer.Write(content.ToString().ToArray() );
                writer.Flush();
                writer.Close();
            }
            return;
        }
        private Stream stringToStream(string value)
        {
            using (MemoryStream stream = new MemoryStream() )
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(value);
                writer.Flush();
                stream.Position = 0;
                return stream;
            }
        }


        [ObsoleteAttribute("This method has been deprecated. Use validationHelper.isNumeric instead.", true)]
        public static bool isNumeric(System.Object Expression, System.Globalization.NumberStyles style, IFormatProvider formatProvider)
        {   // (!) 29/11/2019 moved to validationHelper.cs
            return validationHelper.isNumeric(Expression, style, formatProvider);
        }

        public static string toInvariantCultureString(object value)
        {
            switch (value.GetType().UnderlyingSystemType.ToString())
            {
                case "System.Decimal":
                    return ((Decimal)value).ToString(System.Globalization.CultureInfo.InvariantCulture);
                case "System.Double":
                    return ((Double)value).ToString(System.Globalization.CultureInfo.InvariantCulture);
                case "System.Single":
                    return ((Single)value).ToString(System.Globalization.CultureInfo.InvariantCulture);
                default:
                    return value.ToString();
            }
        }

        public static Decimal toDecimalIrrelevant(string value, bool useEN_US=false)
        {
            if ( useEN_US ) return Convert.ToDecimal(value, new CultureInfo("en-US"));
            return Convert.ToDecimal(value, System.Globalization.CultureInfo.InvariantCulture);
        }


        public static stringsPair toStringsPair(string input, string delimeter)
        {
            stringsPair pair = new stringsPair();
            var p1 = input.IndexOf(delimeter);
            if (p1 > 0)
            {
                pair.left = input.Substring(0, p1);
                pair.right = input.Substring(p1 + delimeter.Length);
            }
            else
            {
                pair.left = input;
            }

            return pair;
        }

        public static byte[] hexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
        public static bool toBoolean(string userText, bool nonParsingDefault)
        {
            if (string.IsNullOrEmpty(userText)) { return nonParsingDefault; }
            userText = stringsHelper.toGreeklish(userText).ToUpper();
            switch (userText)
            {
                case "OXI":
                    return false;
                case "O":
                    return false;
                case "NO":
                    return false;
                case "0":
                    return false;
                case "FALSE":
                    return false;
                case "DISABLE":
                    return false;

                case "NAI":
                    return true;
                case "N":
                    return true;
                case "YES":
                    return true;
                case "TRUE":
                    return true;
                case "1":
                    return true;
                case "ENABLE":
                    return true;

                default:
                    return nonParsingDefault;
            }
        }
        public static string toRedable(Exception ex)
        {
            string msg = null;
            if (ex != null)
            {
                if ( !string.IsNullOrEmpty(ex.Message) )
                {
                    msg = ex.Message;
                }
                if (ex.InnerException != null)
                {
                    if ( !string.IsNullOrEmpty(ex.InnerException.Message))
                    {
                        if (!string.IsNullOrEmpty(msg)) { msg += "\n"; }
                        msg += ex.InnerException.Message;
                    }
                }
            }

            if ( msg == null ) { msg="General error (exception)";}

            return msg;
        }

        public static string base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string sensitiveValueToConfigValue(string readersPassword, string sensitiveValue)
        {
            return converters.base64Encode(stringCipher.encrypt(sensitiveValue, readersPassword));
        }

        public static string configValueToSensitiveValue(string readersPassword, string sensitiveValue)
        {
            string toDecode = converters.base64Decode(sensitiveValue);
            return stringCipher.decrypt(toDecode, readersPassword);
        }


        // (v) 27 nov 2019
        public static string objectToCompactString<T>(T inputObject)
        {
            StringBuilder modelAsString = new StringBuilder();
            using (StringWriter writerToModel = new StringWriter(modelAsString))
            {
                new FAST.Core.baseSerializer().serialize(inputObject, typeof(T), writerToModel, "objectToCompactString()");
            }
            string temp = modelAsString.ToString();
            temp = stringsHelper.zip(temp);
            temp =converters.base64Encode(temp);

            return temp;
        }

        public static T compactStringToObject<T>(string inputCompactString)
        {
            T objectToReturn = default(T);
            inputCompactString = compactStringToString(inputCompactString);
            using (StringReader reader = new StringReader(inputCompactString))
            {
                objectToReturn = (T)new FAST.Core.baseSerializer().deserialize(typeof(T), reader, "compactStringToObject()");
            }
            return objectToReturn;
        }
        
        public static string compactStringToString(string inputCompactString)
        {
            inputCompactString = converters.base64Decode(inputCompactString);
            inputCompactString = stringsHelper.unZip(inputCompactString);

            return inputCompactString;
        }

        // (v) 29/11/2019
				
        /// <summary>
        ///     Example:
        ///     Console.WriteLine(FormatCurrency("USD", 1230.56M));
	///     Console.WriteLine(FormatCurrency("USD", 1230.00M));
	///	Console.WriteLine(FormatCurrency("VND", 1200000M));
        /// </summary>
        /// <param name="currencyCode"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        /// example of return: USD 1,230.56   USD 1,230    1.200.000 VND
        /// 
	    public static string formatCurrency(string currencyCode, decimal amount)
	{
            CultureInfo culture = (from c in CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                                    let r = new RegionInfo(c.LCID)
                                    where r != null
                                    && r.ISOCurrencySymbol.ToUpper() == currencyCode.ToUpper()
                                    select c).FirstOrDefault();
            if(culture == null)
            {
                // fall back to current culture if none is found
                // you could throw an exception here if that's not supposed to happen
                culture = CultureInfo.CurrentCulture; 
		   
            }
            culture = (CultureInfo)culture.Clone();
            culture.NumberFormat.CurrencySymbol = currencyCode;
            culture.NumberFormat.CurrencyPositivePattern = culture.NumberFormat.CurrencyPositivePattern == 0 ? 2 : 3;
            var cnp = culture.NumberFormat.CurrencyNegativePattern;
            switch(cnp)
            {
	        case 0: cnp = 14; break;
	        case 1: cnp = 9; break;
	        case 2: cnp = 12; break;
	        case 3: cnp = 11; break;
	        case 4: cnp = 15; break;
	        case 5: cnp = 8; break;
	        case 6: cnp = 13; break;
	        case 7: cnp = 10; break;
            }
	    culture.NumberFormat.CurrencyNegativePattern = cnp;
	  
            return amount.ToString("C" + ((amount % 1) == 0?"0":"2"), culture);		
	}

        // (v) 11/9/2023 

        /// <summary>
        /// Hexadecimals to bytes.
        /// </summary>
        /// <param name="hex">The hexadecimal.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] hexToBytes(string hex)
        {
            byte[] bytes = new byte[hex.Length / 2];

            for (int i = 0; i < hex.Length / 2; i++)
            {
                string code = hex.Substring(i * 2, 2);
                bytes[i] = byte.Parse(code, System.Globalization.NumberStyles.HexNumber);
            }
            return bytes;
        }

        /// <summary>
        /// Byteses to hexadecimal.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>System.String.</returns>
        public static string bytesToHex(byte[] bytes)
        {
            var hex = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
                hex.AppendFormat("{0:X2}", bytes[i]);

            return hex.ToString();
        }


    }
}
