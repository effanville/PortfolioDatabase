using System;

namespace FinancialStructures.Mathematics
{
    public static class MathSupport
    {
        public static string Trunc(double value, int exp = 2)
        {
            double decimalPlaces = Math.Pow(10, exp);
            return (Math.Truncate(value * decimalPlaces) / decimalPlaces).ToString();
        }

        public static double Truncate(double value, int exp = 2)
        {
            double decimalPlaces = Math.Pow(10, exp);
            return (Math.Truncate(value * decimalPlaces) / decimalPlaces);
        }

    }
}
