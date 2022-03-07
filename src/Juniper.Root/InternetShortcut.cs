using System;
using System.Text.RegularExpressions;

namespace Juniper
{
    public class InternetShortcut
    {
        private static readonly Regex pattern = new(@"\[InternetShortcut\]\r?\nURL=(.+)\r?\n", RegexOptions.Compiled);

        public static InternetShortcut Parse(string input)
        {
            var match = pattern.Match(input);
            if (!match.Success)
            {
                throw new FormatException("Input string is not a valid InternetShortcut");
            }

            if (!Uri.TryCreate(match.Groups[1].Value, UriKind.Absolute, out var url))
            {
                throw new FormatException("Input string does not contain a valid URL.");
            }

            return new InternetShortcut(url);
        }


        public static bool TryParse(string input, out InternetShortcut value)
        {
            value = null;

            var match = pattern.Match(input);
            return match.Success
                && Uri.TryCreate(match.Groups[1].Value, UriKind.Absolute, out var url)
                && (value = new InternetShortcut(url)) != null;
        }

        public Uri Uri { get; private set; }

        public InternetShortcut(Uri uri)
        {
            if(uri is null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (!uri.IsAbsoluteUri)
            {
                throw new FormatException("Only absolute URIs may be used.");
            }

            Uri = uri;
        }

        public override string ToString()
        {
            return $@"[InternetShortcut]
URL={Uri}
";
        }
    }
}
