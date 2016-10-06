using System;

namespace UIOmatic.Extensions
{
    internal static class StringExtensions
    {
        public static string MakeSqlSafeName(this string input)
        {
            return "[" + input.Trim('[', ']') + "]";
        }
    }
}
