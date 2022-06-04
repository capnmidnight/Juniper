#nullable enable
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
#pragma warning restore IDE1006 // Naming Styles
    }
}
