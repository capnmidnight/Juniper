using System.Text.RegularExpressions;

namespace Juniper.Configuration
{
    public static class IWebHostEnvironmentExt
    {
        private static readonly Regex versionPattern = new("\"version\": \"([^\"]+)\"", RegexOptions.Compiled);

        public static Version GetVersion(this IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                var rand = new Random();
                return new Version(rand.Next(0, 1000), rand.Next(0, 1000), rand.Next(0, 1000), rand.Next(0, 1000));
            }
            else
            {
                var file = System.IO.File.ReadAllText("package.json");
                var match = versionPattern.Match(file);
                return Version.Parse(match.Groups[1].Value);
            }
        }
    }
}
