using System.Globalization;
using System.Text.RegularExpressions;

using static System.Math;

namespace System
{
    /// <summary>
    /// Extension methods for System.string.
    /// </summary>
    public static class StringExt
    {
        /// <summary>
        /// Perforum `Uri.UnescapeDataString(value)` on this string.
        /// </summary>
        /// <param name="value">The string to remove escape sequences from.</param>
        /// <returns>The unescaped string.</returns>
        public static string Unescape(this string value)
        {
            if (value is null)
            {
                return null;
            }
            else
            {
                return Uri.UnescapeDataString(value);
            }
        }

        /// <summary>
        /// Perform `Uri.EscapeDataString(value)` on this string.
        /// </summary>
        /// <param name="value">The string to add escape sequences from.</param>
        /// <returns>The escaped string.</returns>
        public static string Escape(this string value)
        {
            if (value is null)
            {
                return null;
            }
            else
            {
                return Uri.EscapeDataString(value);
            }
        }

        /// <summary>
        /// Replace forward-slash or backward-slash with whatever is the correct slash for a
        /// directory separator character on the current operating system.
        /// </summary>
        /// <returns>The path.</returns>
        /// <param name="value">Value.</param>
        public static string NormalizePath(this string value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return value.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Join a collection of string path components by the system's standard directory part
        /// separator character
        /// </summary>
        /// <param name="parts"></param>
        /// <returns></returns>
        public static string Join(this string[] parts)
        {
            return Join(parts, Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// An extension method to join an array of strings with a separator. Sort of a fluent
        /// interface for System.string.Join.
        /// </summary>
        /// <param name="parts">The strings to join.</param>
        /// <param name="separator">The separator to insert between each string value. Defaults to the empty string.</param>
        /// <returns>The joined string.</returns>
        public static string Join(this string[] parts, string separator)
        {
            if (parts is null)
            {
                throw new ArgumentNullException(nameof(parts));
            }

            if (parts.Length == 0)
            {
                return string.Empty;
            }

            if (separator is null)
            {
                separator = string.Empty;
            }

            var sb = new Text.StringBuilder(parts[0]);
            for (var i = 1; i < parts.Length; ++i)
            {
                sb.Append(separator)
                    .Append(parts[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// An extension method to join an array of strings with a separator. Sort of a fluent
        /// interface for System.String.Join.
        /// </summary>
        /// <param name="parts">The strings to join.</param>
        /// <param name="separator">The separator to insert between each string value. Defaults to the empty string.</param>
        /// <returns>The joined string.</returns>
        public static string Join(this string[] parts, char separator)
        {
            if (parts is null)
            {
                throw new ArgumentNullException(nameof(parts));
            }

            if (parts.Length == 0)
            {
                return string.Empty;
            }

            var sb = new Text.StringBuilder(parts.Length * 10);
            sb.Append(parts[0]);
            for (var i = 1; i < parts.Length; ++i)
            {
                sb.Append(separator);
                sb.Append(parts[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Performs a split operation with a single character delimiter, to avoid using the params version
        /// provided by NETFX, which instantiates an unnecessary array.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string[] SplitX(this string value, char token)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            else if (value.Length == 0)
            {
                return Array.Empty<string>();
            }
            else
            {
                var count = 0;
                foreach (var c in value)
                {
                    if (c == token)
                    {
                        ++count;
                    }
                }

                if (count == 0)
                {
                    return new string[] { value };
                }
                else
                {
                    var parts = new string[count + 1];
                    var start = 0;
                    count = 0;
                    for (var i = 0; i < value.Length; ++i)
                    {
                        if (value[i] == token)
                        {
                            parts[count] = value[start..i];
                            start = i + 1;
                            ++count;
                        }
                    }

                    parts[^1] = value[start..];

                    return parts;
                }
            }
        }

        public static int WagnerFischer_Damerau_Levenshtein_Distance(this string a, string b)
        {
            if (a is null)
            {
                throw new ArgumentNullException(nameof(a));
            }

            if (b is null)
            {
                throw new ArgumentNullException(nameof(b));
            }

            var m = a.Length + 1;
            var n = b.Length + 1;
            var matrix = new int[m, n];

            for (var i = 1; i < m; ++i)
            {
                matrix[i, 0] = i;
            }

            for (var j = 1; j < n; ++j)
            {
                matrix[0, j] = j;
            }

            for (var j = 1; j < n; ++j)
            {
                var y = b[j - 1];
                for (var i = 1; i < m; ++i)
                {
                    var x = a[i - 1];
                    var deleteCost = matrix[i - 1, j] + 1;
                    var insertCost = matrix[i, j - 1] + 1;
                    var subCost = (x == y ? 0 : 1);
                    var substitutionCost = matrix[i - 1, j - 1] + subCost;
                    matrix[i, j] = Min(Min(deleteCost, insertCost), substitutionCost);
                    if (j > 1 && i > 1 && a[i - 2] == y && b[j - 2] == x)
                    {
                        var transpositionCost = matrix[i - 2, j - 2] + subCost;
                        matrix[i, j] = Min(matrix[i, j], transpositionCost);
                    }
                }
            }

            return matrix[a.Length, b.Length];
        }

        public static string ToTitleCase(this string val)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(val);
        }

        public static float WagnerFischer_Damerau_Levenshtein_Similarity(this string a, string b)
        {
            if (a is null)
            {
                throw new ArgumentNullException(nameof(a));
            }

            if (b is null)
            {
                throw new ArgumentNullException(nameof(b));
            }

            if (a.Length < b.Length)
            {
                (b, a) = (a, b);
            }

            var distance = a.WagnerFischer_Damerau_Levenshtein_Distance(b);
            var prop = 1f;
            if (a.Length > 0)
            {
                prop = (float)distance / a.Length;
            }
            else if (b.Length == 0)
            {
                prop = 0;
            }

            return 1 - prop;
        }

        /// <summary>
        /// The Winkler modification will not be applied unless the
        /// percent match was at or above the mWeightThreshold percent
        /// without the modification. Winkler's paper used a default
        /// value of 0.7
        /// </summary>
        private const float WinklerThreshold = 0.7f;

        /// <summary>
        /// Size of the prefix to be considered by the Winkler
        /// modification. Winkler's paper used a default value of 4.
        /// </summary>
        private const int WinklerPrefixSize = 4;

        /// <summary>
        /// Returns the Jaro-Winkler distance between the specified
        /// strings. The distance is symmetric and will fall in the
        /// range 0 (no match) to 1 (perfect match).
        /// </summary>
        /// <param name="a">First String</param>
        /// <param name="b">Second String</param>
        /// <returns></returns>
        public static float JaroWinkler_Similarity(this string a, string b)
        {
            if (a is null)
            {
                throw new ArgumentNullException(nameof(a));
            }

            if (b is null)
            {
                throw new ArgumentNullException(nameof(b));
            }

            if (a.Length == 0)
            {
                return b.Length == 0 ? 1 : 0;
            }

            var searchRange = Max(0, (Max(a.Length, b.Length) / 2) - 1);

            var matchesA = new bool[a.Length];
            var matchesB = new bool[b.Length];

            var numCommon = 0;
            for (var i = 0; i < a.Length; ++i)
            {
                var start = Max(0, i - searchRange);
                var end = Min(i + searchRange + 1, b.Length);
                for (var j = start; j < end; ++j)
                {
                    if (!matchesB[j] && a[i] == b[j])
                    {
                        matchesA[i] = true;
                        matchesB[j] = true;
                        ++numCommon;
                        break;
                    }
                }
            }

            if (numCommon == 0)
            {
                return 0;
            }

            var numTransposed = 0;
            var k = 0;
            for (var i = 0; i < a.Length; ++i)
            {
                if (matchesA[i])
                {
                    while (!matchesB[k])
                    {
                        ++k;
                    }

                    if (a[i] != b[k])
                    {
                        ++numTransposed;
                    }

                    ++k;
                }
            }

            var halfNumTransposed = numTransposed / 2;

            float numCommonFloat = numCommon;
            var weight = ((numCommonFloat / a.Length)
                + (numCommonFloat / b.Length)
                + ((numCommon - halfNumTransposed) / numCommonFloat)) / 3f;

            if (weight <= WinklerThreshold)
            {
                return weight;
            }

            var prefixLength = Min(WinklerPrefixSize, Min(a.Length, b.Length));
            var position = 0;
            while (position < prefixLength
                && a[position] == b[position])
            {
                ++position;
            }

            if (position == 0)
            {
                return weight;
            }

            return weight + (0.1f * position * (1 - weight));
        }

        public static int DistanceTo(this string a, string b)
        {
            return WagnerFischer_Damerau_Levenshtein_Distance(a, b);
        }

        public static float Similarity(this string a, string b)
        {
            return a.JaroWinkler_Similarity(b);
        }

        private static IEnumerable<string> GraphemeClusters(this string s)
        {
            var enumerator = StringInfo.GetTextElementEnumerator(s);
            while (enumerator.MoveNext())
            {
                yield return (string)enumerator.Current;
            }
        }

        public static string Reverse(this string s)
        {
            return string.Concat(s.GraphemeClusters().Reverse());
        }

        public static bool ContainsIgnoreCase(this string value, string subStr)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

#if NETCOREAPP || NETSTANDARD2_1
            return value.Contains(subStr, StringComparison.OrdinalIgnoreCase);
#else
            return value.ToLower().Contains(subStr.ToLower());
#endif
        }

        // Examines the domain part of the email and normalizes it.
        private static string DomainMapper(Match match)
        {
            // Use IdnMapping class to convert Unicode domain names.
            var idn = new IdnMapping();

            // Pull out and process domain name (throws ArgumentException on invalid)
            var domainName = idn.GetAscii(match.Groups[2].Value);

            return match.Groups[1].Value + domainName;
        }

        private static readonly Regex normalizeDomain = new(
            "(@)(.+)$",
            RegexOptions.Compiled,
            TimeSpan.FromMilliseconds(200)
        );

        private static readonly Regex emailPattern = new(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase,
            TimeSpan.FromMilliseconds(250)
        );

        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                // Normalize the domain
                email = normalizeDomain.Replace(email, DomainMapper);
                return emailPattern.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }
    }
}