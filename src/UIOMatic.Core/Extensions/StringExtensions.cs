using System;
using System.Text.RegularExpressions;

namespace UIOMatic.Extensions
{
    public static class StringExtensions
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

        public static string ToSentenceCase(this string str)
        {
            return Regex.Replace(str, "[a-z][A-Z]", m => $"{m.Value[0]} {char.ToLower(m.Value[1])}");
        }
    }
}
