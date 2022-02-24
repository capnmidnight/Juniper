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
            var needsInstall = false;
            if (nodeModulesDir.Exists)
            {
                using var packageStream = packageJson.OpenRead();
                var package = await JsonSerializer.DeserializeAsync<NPMPackage>(packageStream);
                var dependencies = (package?.dependencies ?? new Dictionary<string, string>()).Merge(package?.devDependencies);
                foreach (var (name, requiredVersionStr) in dependencies)
                {
                    if (await NeedsInstall(name, requiredVersionStr))
                    {
                        needsInstall = true;
                    }
                }
            }

            if (needsInstall)
            {
                OnInfo("Install required");
                await base.RunAsync();
            }
            else
            {
                OnInfo("Install not required");
            }
        }

        private static readonly Regex versionPattern = new(@"(>|<|>=|<=|~|\^|=)?(\d+\.\d+\.\d+)", RegexOptions.Compiled);
        private async Task<bool> NeedsInstall(string name, string requiredVersionStr)
        {
            var depDir = nodeModulesDir.CD(name);
            if (!depDir.Exists)
            {
                OnInfo($"Dependency {name} does not exist");
                return true;
            }

            var depPackageJson = depDir.Touch("package.json");
            if (!depPackageJson.Exists)
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
                return false;
            }

            var match = versionPattern.Match(requiredVersionStr);
            if (!match.Success
                || !Version.TryParse(match.Groups[2].Value, out var requiredVersion))
            {
                OnInfo($"Required version of dependency {name} cannot be parsed");
                return true;
            }

            using var packageStream = depPackageJson.OpenRead();
            var package = await JsonSerializer.DeserializeAsync<NPMPackage>(packageStream);
            if (package?.version is null
                || !Version.TryParse(package.version, out var actualVersion))
            {
                OnInfo($"Dependency {name} does not have a version value in package.json");
                return true;
            }

            var op = match.Groups[1].Value;
            if (!(op == "<" && actualVersion < requiredVersion
                || op == "<=" && actualVersion <= requiredVersion
                || op == ">" && actualVersion > requiredVersion
                || op == ">=" && actualVersion >= requiredVersion
                || (op == "=" || op.Length == 0) && actualVersion == requiredVersion
                || op == "~" && actualVersion.Major == requiredVersion.Major && actualVersion.Minor == requiredVersion.Minor
                || op == "^" && (requiredVersion.Major != 0 && actualVersion.Major == requiredVersion.Major
                    || requiredVersion.Minor != 0 && actualVersion.Minor == requiredVersion.Minor
                    || requiredVersion.Build != 0 && actualVersion.Build == requiredVersion.Build)))
            {
                OnInfo($"Versions don't match {package.version} {op} {requiredVersionStr[op.Length..]}");
                return true;
            }

            return false;
        }

        private class NPMPackage
        {
            public string? version { get; set; }
            public Dictionary<string, string>? dependencies { get; set; }
            public Dictionary<string, string>? devDependencies { get; set; }
        }
    }
}
