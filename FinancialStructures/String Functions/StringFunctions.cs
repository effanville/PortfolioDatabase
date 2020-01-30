using System;

namespace StringFunctions
{
    public static class StringExtensions
    {
        public static string WithMaxLength(this string value, int maxLength)
        {
            if (value == null)
            {
                return null;
            }

            return value.Substring(0, Math.Min(value.Length, maxLength));
        }
    }
}
