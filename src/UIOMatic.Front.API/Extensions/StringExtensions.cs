using System.Text;

namespace UIOMatic.Front.API.Extensions
{
    internal static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string? value) => string.IsNullOrWhiteSpace(value);

        public static string ToPascalCase(this string source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return SymbolsPipe(
                source,
                '\0',
                (s, i) => new char[] { char.ToUpperInvariant(s) });
        }

        public static string ToCamelCase(this string source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return SymbolsPipe(
                source,
                '\0',
                (s, disableFrontDelimeter) =>
                {
                    if (disableFrontDelimeter)
                    {
                        return new char[] { char.ToLowerInvariant(s) };
                    }

                    return new char[] { char.ToUpperInvariant(s) };
                });
        }

        private static readonly char[] Delimeters = { ' ', '-', '_' };

        private static string SymbolsPipe(
            string source,
            char mainDelimeter,
            Func<char, bool, char[]> newWordSymbolHandler)
        {
            var builder = new StringBuilder();

            bool nextSymbolStartsNewWord = true;
            bool disableFrontDelimeter = true;
            for (var i = 0; i < source.Length; i++)
            {
                var symbol = source[i];
                if (Delimeters.Contains(symbol))
                {
                    if (symbol == mainDelimeter)
                    {
                        builder.Append(symbol);
                        disableFrontDelimeter = true;
                    }

                    nextSymbolStartsNewWord = true;
                }
                else if (!char.IsLetterOrDigit(symbol))
                {
                    builder.Append(symbol);
                    disableFrontDelimeter = true;
                    nextSymbolStartsNewWord = true;
                }
                else
                {
                    if (nextSymbolStartsNewWord || char.IsUpper(symbol))
                    {
                        builder.Append(newWordSymbolHandler(symbol, disableFrontDelimeter));
                        disableFrontDelimeter = false;
                        nextSymbolStartsNewWord = false;
                    }
                    else
                    {
                        builder.Append(symbol);
                    }
                }
            }

            return builder.ToString();
        }
    }
}
