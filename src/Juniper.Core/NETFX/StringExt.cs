using System.IO;
using System.Text.RegularExpressions;

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
        public static string URIUnescape(this string value)
        {
            if (value == null)
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
        public static string URIEscape(this string value)
        {
            if (value == null)
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
            return value
                .Replace('/', Path.DirectorySeparatorChar)
                .Replace('\\', Path.DirectorySeparatorChar);
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
            if (parts.Length == 0)
            {
                return string.Empty;
            }

            if (separator == null)
            {
                separator = string.Empty;
            }

            var sb = new Text.StringBuilder(parts[0]);
            for (var i = 1; i < parts.Length; ++i)
            {
                sb.AppendFormat("{0}{1}", separator, parts[i]);
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
            var regex = new Regex(Regex.Escape(token.ToString()));
            var matches = regex.Matches(value);
            var parts = new string[matches.Count + 1];
            var start = 0;
            for (var i = 0; i < matches.Count; ++i)
            {
                var match = matches[i];
                parts[i] = value.Substring(start, match.Index - start);
                start = match.Index + 1;
            }

            parts[matches.Count] = value.Substring(start);
            return parts;
        }

        public static int DistanceTo(this string a, string b)
        {
            return WagnerFischer_LevenshteinDistance(a, b);
        }

        private static int WagnerFischer_LevenshteinDistance(string a, string b)
        {
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
                    var substitueCost = matrix[i - 1, j - 1] + (x == y ? 0 : 1);

                    matrix[i, j] = Math.Min(Math.Min(deleteCost, insertCost), substitueCost);
                }
            }

            return matrix[m - 1, n - 1];
        }

        public static float Similarity(this string a, string b)
        {
            if(a.Length < b.Length)
            {
                var c = a;
                a = b;
                b = c;
            }

            var distance = a.DistanceTo(b);
            var prop = 1f;
            if (a.Length > 0)
            {
                prop = (float)distance / a.Length;
            }
            else if(b.Length == 0)
            {
                prop = 0;
            }

            return 1 - prop;
        }
    }
}