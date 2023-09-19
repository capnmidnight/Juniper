#nullable enable
using System.Text.Json;

namespace Juniper.Processes
{
    public class NPMPackage
    {
#pragma warning disable IDE1006 // Naming Styles
        public string? version { get; set; }
        public string? name { get; set; }
        public string? types { get; set; }
        public Dictionary<string, string>? scripts { get; set; }
        public Dictionary<string, string>? dependencies { get; set; }
        public Dictionary<string, string>? devDependencies { get; set; }
        public string[]? workspaces { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public static NPMPackage? Read(FileInfo file) =>
            ReadAsync(file).Result;

        public static Task<NPMPackage?> ReadAsync(FileInfo file) =>
            ReadAsync(file, CancellationToken.None);

        public static async Task<NPMPackage?> ReadAsync(FileInfo file, CancellationToken cancellationToken)
        {
            using var packageStream = file.OpenRead();
            var package = await JsonSerializer.DeserializeAsync<NPMPackage>(packageStream, cancellationToken: cancellationToken);
            return package;
        }
    }
}
