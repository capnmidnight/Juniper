using System.Text.RegularExpressions;

namespace Juniper.Processes;

public class NPMInstallCommand : ShellCommand
{
    private readonly FileInfo packageJson;
    private readonly DirectoryInfo nodeModulesDir;
    public bool? NeededInstall { get; private set; } = null;


    public NPMInstallCommand(FileInfo packageJson, bool noPackageLock = true)
        : base(packageJson.Directory, "npm", $"install")
    {
        if (!packageJson.Exists)
        {
            throw new FileNotFoundException("package.json file does not exist!", packageJson.FullName);
        }

        this.packageJson = packageJson;

        nodeModulesDir = workingDir.CD("node_modules");
    }

    protected override void OnInfo(string message)
    {
        if (message != "found 0 vulnerabilities")
        {
            base.OnInfo(message);
        }
    }

    public override async Task RunAsync(CancellationToken cancellationToken)
    {
        NeededInstall = await NeedsInstall(cancellationToken);
        if (NeededInstall.Value)
        {
            await base.RunAsync(cancellationToken);
        }
    }

    private async Task<bool> NeedsInstall(CancellationToken? cancellationToken = null)
    {
        var needsInstall = !nodeModulesDir.Exists;
        if (!needsInstall)
        {
            var queue = new Queue<DirectoryInfo>() { workingDir };
            var checkedPackageJsons = new HashSet<string>();
            while (queue.Count > 0 && !needsInstall)
            {
                var here = queue.Dequeue();
                if (!checkedPackageJsons.Contains(packageJson.FullName))
                {
                    checkedPackageJsons.Add(packageJson.FullName);

                    var package = await NPMPackage.ReadAsync(packageJson, cancellationToken ?? CancellationToken.None);
                    if (package is not null)
                    {
                        var workspaces = package.workspaces ?? Array.Empty<string>();
                        queue.AddRange(workspaces
                            .Where(w => w is not null)
                            .SelectMany(workingDir.GetDirectories)
                            .Where(w => w.Touch("package.json").Exists));

                        var dependencies = package.dependencies.Merge(package?.devDependencies);
                        foreach (var (name, requiredVersionStr) in dependencies)
                        {
                            var installReason = await NeedsInstall(here, name, requiredVersionStr, cancellationToken ?? CancellationToken.None);
                            if (installReason is not null)
                            {
                                OnWarning(installReason);
                                needsInstall = true;
                                break;
                            }
                        }
                    }
                }
            }
        }

        return needsInstall;
    }

    private static readonly Regex versionPattern = new(@"(>|<|>=|<=|~|\^|=)?(\d+\.\d+\.\d+)", RegexOptions.Compiled);

    private static async Task<string?> NeedsInstall(DirectoryInfo fromDir, string name, string requiredVersionStr, CancellationToken cancellationToken)
    {
        if (requiredVersionStr.StartsWith("http")
            || requiredVersionStr.StartsWith("git"))
        {
            return $"Dependency {name} is a URL dependency";
        }

        if (requiredVersionStr.Length == 0
            || requiredVersionStr == "*")
        {
            return $"Dependency {name} is a glob dependency";
        }

        var packageDir = ResolveNPMPackage(fromDir, name);
        if (packageDir?.Exists != true)
        {
            return $"Dependency {name} couldn't be found in any resolution directories";
        }

        if(requiredVersionStr.StartsWith("file:"))
        {
            return null;
        }

        var depPackageJson = packageDir.Touch("package.json");
        if (!depPackageJson.Exists)
        {
            return $"Dependency {name} is missing package.json";
        }

        var package = await NPMPackage.ReadAsync(depPackageJson, cancellationToken);
        if (package is null)
        {
            return $"Dependency {name} couldn't parse package.json";
        }

        if (package.version is null
            || !Version.TryParse(package.version, out var actualVersion))
        {
            return $"Dependency {name} does not have a version value in package.json";
        }

        var match = versionPattern.Match(requiredVersionStr);
        if (!match.Success
            || !Version.TryParse(match.Groups[2].Value, out var requiredVersion))
        {
            return $"Required version of dependency {name} cannot be parsed";
        }

        var op = match.Groups[1].Value;
        var isGood = op switch
        {
            "<" => actualVersion < requiredVersion,
            "<=" => actualVersion <= requiredVersion,
            ">" => actualVersion > requiredVersion,
            ">=" => actualVersion >= requiredVersion,
            "=" or "" => actualVersion == requiredVersion,
            "~" => actualVersion.Major == requiredVersion.Major && actualVersion.Minor == requiredVersion.Minor,
            "^" => (requiredVersion.Major != 0 && actualVersion.Major == requiredVersion.Major)
               || (requiredVersion.Minor != 0 && actualVersion.Minor == requiredVersion.Minor)
               || (requiredVersion.Build != 0 && actualVersion.Build == requiredVersion.Build),
            _ => throw new InvalidOperationException(op)
        };
        if (!isGood)
        {
            return $"Dependency {name} versions don't match {package.name}: {package.version} {op} {requiredVersionStr[op.Length..]}";
        }

        return null;
    }

    private static DirectoryInfo? ResolveNPMPackage(DirectoryInfo? fromDir, string name)
    {
        while(fromDir is not null)
        {
            var depDir = fromDir.CD("node_modules", name);
            if (depDir.Exists)
            {
                return depDir;
            }

            fromDir = fromDir.Parent;
        }

        return null;
    }
}
