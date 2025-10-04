namespace FAST.Strings
{
    public class infoStringHelper
    {
        public static string inf(string infoName, string infoBuffer)
        {
            int p1 = (-1);
            int p2 = (-1);
            string returnInf = "";

            p1 = infoBuffer.IndexOf(infoName + "{");
            if (p1 >= 0)
            {
                p2 = infoBuffer.IndexOf("}", p1 + infoName.Length);
            }
            if ((p1 >= 0) && (p2 >= 0))
            {
                returnInf = infoBuffer.Substring(p1 + infoName.Length + 1, p2 - p1 - infoName.Length - 1);
            }
            return returnInf;
        }
        public static string infUrl(string infoName, string infoBuffer)
        {
            int p1 = (-1);
            int p2 = (-1);
            string returnInf = "";

            p1 = infoBuffer.IndexOf(infoName);
            if (p1 >= 0)
            {
                p2 = infoBuffer.IndexOf("&", p1 + infoName.Length);
            }
            if ((p1 >= 0) && (p2 >= 0))
            {
                returnInf = infoBuffer.Substring(p1 + infoName.Length + 1, p2 - p1 - infoName.Length - 1);
            }
            return returnInf;
        }
        public static string setInf(string infoBuffer, string infoName, object infoValue)
        {
            int p1 = (-1);
            int p2 = (-1);

            string value = infoValue.ToString();

            p1 = infoBuffer.IndexOf(infoName + "{");
            if (p1 >= 0)
            {
                p2 = infoBuffer.IndexOf("}", p1 + infoName.Length);
            }
            if ((p1 >= 0) && (p2 >= 0))
            {
                infoBuffer = infoBuffer.Substring(0, p1) + infoName + "{" + value + "}" + infoBuffer.Substring(p2 + 1);
            }
            else
            {
                infoBuffer = infAdd(infoBuffer, infoName, value);
            }
            return infoBuffer;
        }
        public static string infAdd(string infoBuffer, string infoName, object infoValue)
        {
            string value = infoValue.ToString().Trim();
            infoBuffer = infoBuffer.Trim();
            if (!String.IsNullOrEmpty(infoBuffer)) { infoBuffer += ","; }
            infoBuffer += infoName.Trim().ToUpper() + "{" + value + "}";
            return infoBuffer;
        }
    }

}
