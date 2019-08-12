using System.IO;

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
        public static string Join(this string[] parts, string separator = null)
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
            var sb = new Text.StringBuilder(parts[0]);
            for (var i = 1; i < parts.Length; ++i)
            {
                sb.AppendFormat("{0}{1}", separator, parts[i]);
            }
            return sb.ToString();
        }
    }
}