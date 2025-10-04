namespace FAST.Arithmetic
{
    public class arithmeticHelper
    {
        public static decimal divide(decimal num1, decimal num2, int decimalPoints)
        {
            if (num2 == 0)
            {
                return 0M;
            }
            else
            {
                return Math.Round(num1 / num2, decimalPoints);
            }
        }

        // (v) add 2/12/2019
        public static bool isOdd(int value)
        {
            return value % 2 != 0;
        }

        public static bool isEven(int value)
        {
            return value % 2 == 0;
        }

        public static double rad(double deg)
        {
	    return deg * Math.PI / 180.0f;
	}



    }
}
