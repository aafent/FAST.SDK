namespace FAST.Strings
{
    public class rotXEncryption
    {
        public string alphabet = "_ αβγδεζηθικλμνξοπρστυφχψω,.<>?:abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ+-*/ΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩς!@#$%^&()[]{}";
        private byte encryptionLevel;

        public rotXEncryption(byte encryptionLevel)
        {
            if (encryptionLevel < 0 | encryptionLevel > 2)
            {
                throw new ArgumentException("Invalid encryption Level. Acceptable is between 0 and 2");
            }
            this.encryptionLevel = encryptionLevel;
        }

        public string encrypt(string originalString)
        {
            if (string.IsNullOrEmpty(originalString)) return originalString;
            switch (encryptionLevel)
            {
                case 0:
                    return encryptRotX(originalString);
                case 1:
                    return stringsHelper.reverse(encryptRotX(originalString));
                case 2:
                    return encryptRotX(stringsHelper.reverse(encryptRotX(originalString)));
                default:
                    throw new Exception("encryptionLevel out of range");
            }

        }
        public string decrypt(string encryptedString)
        {
            if (string.IsNullOrEmpty(encryptedString) ) return encryptedString;
            switch (encryptionLevel)
            {
                case 0:
                    return decryptRotX(encryptedString);
                case 1:
                    return decryptRotX(stringsHelper.reverse(encryptedString));
                case 2:
                    return decryptRotX(stringsHelper.reverse(decryptRotX(encryptedString)));
                default:
                    throw new Exception("encryptionLevel out of range");
            }
        }

        private string encryptRotX(string originalString)
        {
            string returnString = "";
            int shift = alphabet.Length / 2;

            foreach (char c in originalString)
            {
                int posInAlphabet = alphabet.IndexOf(c);
                if (posInAlphabet < 0)
                {
                    returnString += c;
                }
                else
                {
                    int nextIndex = posInAlphabet + shift;

                    if (nextIndex >= alphabet.Length)
                        nextIndex = nextIndex - alphabet.Length;

                    returnString += alphabet[nextIndex];
                    shift = alphabet.IndexOf(alphabet[nextIndex]);
                }
            }

            return returnString;
        }

        private string decryptRotX(string encryptedString)
        {
            string returnString = "";
            int shift = alphabet.Length / 2;

            foreach (char c in encryptedString)
            {
                int posInAlphabet = alphabet.IndexOf(c);
                if (posInAlphabet < 0)
                {
                    returnString += c;
                }
                else
                {
                    int nextIndex = posInAlphabet - shift;

                    if (nextIndex < 0)
                        nextIndex = alphabet.Length + nextIndex; // nextIndex is negative so we are decreasing regardless

                    returnString += alphabet[nextIndex];
                    shift = alphabet.IndexOf(c);
                }
            }
            return returnString;
        }
    }
}
