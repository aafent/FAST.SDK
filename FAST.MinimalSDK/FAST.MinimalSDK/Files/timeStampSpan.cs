using System.Globalization;

namespace FAST.Files
{
    public class timeStampSpan
    {
        // (v) private variables
        private string _timeStamp = "";

        // (v) properties
        public string stamp
        {
            get
            {
                return _timeStamp;
            }
            set
            {
                set(value);
            }
        }
        public bool isValid
        {
            get
            {
                if (string.IsNullOrEmpty(_timeStamp)) { return false; }
                if (_timeStamp.Substring(8, 1) != "-") { return false; }
                if (_timeStamp.Substring(15, 1) != "-") { return false; }
                if (!(_timeStamp.Length == 23 & _timeStamp.Length == 19)) { return false; }
                if (_timeStamp.Length > 19)
                {
                    if (_timeStamp.Substring(19, 1) != "-") { return false; }
                }
                return true;

            }
        }
        public bool isInvalid
        {
            get
            {
                return !isValid;
            }
        }
        public bool hasSequencePart
        {
            get
            {
                if (!isValid) { return false; }
                if (_timeStamp.Length == 23) { return true; }
                return false;
            }
        }
        public string sequencePart
        {
            get
            {
                return _timeStamp.Substring(20, 3);
            }
        }
        public string dateTimeStamp
        {
            get
            {
                return _timeStamp.Substring(0, 19);
            }
        }
        public string datePart
        {
            get
            {
                return _timeStamp.Substring(0, 8);
            }
        }
        public string timePart
        {
            get
            {
                return _timeStamp.Substring(9, 10);
            }
        }

        // (v) constructors
        public timeStampSpan(string timeStamp)
        {
            set(timeStamp);
        }

        // (v) equal operator
        public static bool operator ==(timeStampSpan leftStamp, timeStampSpan rightStamp)
        {
            if (leftStamp.isInvalid && rightStamp.isInvalid) return false;
            return leftStamp.Equals(rightStamp);
        }
        public static bool operator !=(timeStampSpan leftStamp, timeStampSpan rightStamp)
        {
            return !(leftStamp == rightStamp);
        }
        public override bool Equals(object obj)
        {
            return this.stamp == ((timeStampSpan)obj).stamp;
        }
        public override int GetHashCode() // add: 30/6/2021
        {
            return this.stamp.GetHashCode();
        }

        // (v) privates
        private void set(string timeStamp)
        {
            _timeStamp = timeStamp;
        }

        // (v) publics
        public DateTime toDateTime()
        {
            DateTime stampDateTime = DateTime.ParseExact(dateTimeStamp, "yyyyMMdd-HHmmss-fff", CultureInfo.InvariantCulture);
            return DateTime.SpecifyKind(stampDateTime, DateTimeKind.Utc);
        }
        public override string  ToString()
        {
 	         return stamp;
        }


        // (v) statics
        public static string timeStampFromFilePath( string fullPath) 
        {
            return Path.GetFileNameWithoutExtension(fullPath);
        }
        public static timeStampSpan timeStampSpanFromFilePath(string fullPath)
        {
            return new timeStampSpan(timeStampFromFilePath(fullPath));
        }
        public static timeStampSpan newTimeStampSpan()
        {
            var fileTimeStamp = new fileTimeStamp();
            string newDateStamp;
            string newDatetimeStamp;
            fileTimeStamp.timestamps( out newDateStamp, out newDatetimeStamp );
            return new timeStampSpan(newDatetimeStamp); 
        }
        public static timeStampSpan timeStampSpanFromYMD(string YMD)
        {
            if (YMD.Length != 10)
            {
                throw new ArgumentException("Invalid YMD size, always must be 10.");
            }
            return new timeStampSpan(string.Format("{0}-000000-000-000", YMD));
        }

    }
}
