using System;

namespace UIOmatic.Extensions
{
    internal static class StringExtensions
    {
        public static string MakeSqlSafeName(this string input)
        {
            return "[" + input.Trim('[', ']') + "]";
        }

        public static bool DetectIsJson(this string input)
        {
            input = input.Trim();
            if (input.StartsWith("{") && input.EndsWith("}"))
                return true;
            if (input.StartsWith("["))
                return input.EndsWith("]");
            return false;
        }
    }
}
