using System.Text.Json;
using System.Text.RegularExpressions;

namespace Juniper.Processes
{
    public class NPMInstallCommand : ShellCommand
    {
        private readonly FileInfo packageJson;
        private readonly DirectoryInfo nodeModulesDir;


        public NPMInstallCommand(DirectoryInfo? workingDir)
            : base(workingDir, "npm", "install")
        {
            packageJson = this.workingDir.Touch("package.json");
            if (!packageJson.Exists)
            {
                throw new FileNotFoundException("Given directory is not an NPM module!", packageJson.FullName);
            }

            nodeModulesDir = workingDir.CD("node_modules");
        }

        public override async Task RunAsync()
        {
            if (nodeModulesDir.Exists)
            {
                using var packageStream = packageJson.OpenRead();
                var package = await JsonSerializer.DeserializeAsync<NPMPackage>(packageStream);
                var dependencies = (package?.Dependencies ?? new Dictionary<string, string>()).Merge(package?.DevDependencies);
                var needsInstall = (await Task.WhenAll(dependencies.Select(NeedsInstall))).Any();
                if (!needsInstall)
                {
                    OnInfo("Install not required");
                    return;
                }
            }

            OnInfo("Install required");
            await base.RunAsync();
        }

        private static readonly Regex versionPattern = new(@"(>|<|>=|<=|~|\^|=)?(\d+\.\d+\.\d+)", RegexOptions.Compiled);
        private async Task<bool> NeedsInstall(KeyValuePair<string, string> kv) {
            var (name, requiredVersionStr) = kv;

            var depDir = nodeModulesDir.CD(name);
            if (!depDir.Exists)
            {
                OnInfo($"Dependency {name} does not exist");
                return true;
            }

            var packageJSON = depDir.Touch("package.json");
            if (!packageJSON.Exists)
            {
                OnInfo($"Dependency {name} is missing package.json");
                return true;
            }

            if (requiredVersionStr.StartsWith("http")
                || requiredVersionStr.StartsWith("git"))
            {
                OnInfo($"Dependency {name} is a URL dependency");
                return true;
            }

            if (requiredVersionStr.Length == 0
                || requiredVersionStr == "*"
                || requiredVersionStr.StartsWith("file:"))
            {
                OnInfo($"Dependency {name} is trivially satisfiable");
                return false;
            }

            using var packageStream = packageJson.OpenRead();
            var package = await JsonSerializer.DeserializeAsync<NPMPackage>(packageStream);
            if(package?.Version is null
                || !Version.TryParse(package.Version, out var actualVersion))
            {
                OnInfo($"Dependency {name} does not have a version value in package.json");
                return true;
            }

            var match = versionPattern.Match(requiredVersionStr);
            if (!match.Success
                || !Version.TryParse(match.Groups[2].Value, out var requiredVersion))
            {
                OnInfo($"Required version of dependency {name} cannot be parsed");
                return true;
            }
            
            var op = match.Groups[1].Value;
            return !(op == "<" && actualVersion < requiredVersion
                || op == "<=" && actualVersion <= requiredVersion
                || op == ">" && actualVersion > requiredVersion
                || op == ">=" && actualVersion >= requiredVersion
                || (op == "=" || op.Length == 0) && actualVersion == requiredVersion
                || op == "~" && actualVersion.Major == requiredVersion.Major && actualVersion.Minor == requiredVersion.Minor
                || op == "^" && actualVersion.Major == requiredVersion.Major);
        }

        private class NPMPackage
        {
            public string? Version { get; set; }
            public Dictionary<string, string>? Dependencies { get; set; }
            public Dictionary<string, string>? DevDependencies { get; set; }
        }
    }
}
