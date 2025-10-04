namespace FAST.Strings
{

    public class stringsList
    {
        public string defaultDelimiter = null; 
        public stringsList()
        {
        }
        public stringsList(string stringList) 
        {
            setStringList(stringList);
        }
        public stringsList(string stringList, string delimiter)
        {
            defaultDelimiter=delimiter;
            setStringList(stringList);
        }

        private string _getListBuffer = "";
        private string _setListBuffer = "";

        public bool eol
        {
            get
            {
                if (_getListBuffer == "") { return true; }
                else { return false; }
            }
        }

        public string nextToken()
        {
            return nextToken(defaultDelimiter);
        }
        public string nextToken(string delimiter)
        {
            return nextToken(ref _getListBuffer, delimiter);
        }
        public void setStringList(string stringList)
        {
            _getListBuffer = stringList;
            return;
        }
        public void reset()
        {
            _setListBuffer = "";
            return;
        }
        public string add(string stringToAddToList, string delimiter)
        {
            if (_setListBuffer != "") { _setListBuffer += delimiter; }
            _setListBuffer += stringToAddToList;

            return _setListBuffer;
        }
        // (v) todo: to rename or unify both methods
        public string getStringList()
        {
            return _setListBuffer;
        }
        public string getRemainingStringList()
        {
            return _getListBuffer;
        }
        // (^) 

        // (v) statics

        public static string join(string[] tokenToJoin, string delimeter)
        {
            string jointTokens = "";
            for (int item = 0; item <= tokenToJoin.GetUpperBound(0); item++)
            {
                if (jointTokens != "") { jointTokens += delimeter; }
                jointTokens += tokenToJoin[item];
            }
            return jointTokens;
        }
        public static string nextToken(ref string stringList, string delimiter)
        {
            string token = "";
            int p1 = 0;
            p1 = stringList.IndexOf(delimiter);
            if (p1 >= 0)
            {
                token = stringList.Substring(0, p1);
                stringList = stringList.Substring(p1 + delimiter.Length);
            }
            else
            {
                token = stringList;
                stringList = "";
            }

            return token;
        }
        public static string token(string stringList, string delimiter, int numberOfToken)
        {
            string tokenToReturn = "";
            for (int interation = 1; interation <= numberOfToken; interation++)
            {
                tokenToReturn = nextToken(ref stringList, delimiter);
            }
            return tokenToReturn;
        }
        public static bool containsInList(string stringList, string delimiter, string valueToCheck)
        {
            stringList = delimiter + stringList.ToLower().Trim() + delimiter;
            valueToCheck = delimiter + valueToCheck.ToLower().Trim() + delimiter;
            return stringList.Contains(valueToCheck);
        }
        public static bool containsInList(string stringList, string delimiter, string allValuesSymbol, string valueToCheck)
        {
            if (stringList == allValuesSymbol) { return true; }
            if (valueToCheck == allValuesSymbol) { return true; }
            return containsInList(stringList, delimiter, valueToCheck);
        }

    }
}
