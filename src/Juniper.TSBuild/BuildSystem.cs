using Juniper.Processes;
using Juniper.TSWatcher;
using Juniper.Units;

using System.Text.RegularExpressions;

namespace Juniper.TSBuild
{
    public class BuildSystem
    {
        private static DirectoryInfo ResolveStartDir(ref string? startDir, string testDirName, params string[] addlTestDirNames)
        {
            var testDirNames = addlTestDirNames
                .Prepend(testDirName)
                .ToArray();

            startDir ??= Environment.CurrentDirectory;
            var dir = new DirectoryInfo(startDir);

            while (dir != null
                && dir.GetDirectories()
                    .Select(x => x.Name)
                    .Where(name => testDirNames.Contains(name))
                    .Count() != testDirNames.Length)
            {
                dir = dir.Parent;
            }

            if (dir is null)
            {
                throw new FileNotFoundException($"Couldn't find project root from {startDir}");
            }

            return dir;
        }

        protected enum Por
        {
            All,
            Basic,
            Minified
        }

        private static readonly string[] basicFileNames = {
            "index.js",
            "index.js.map"
        };

        private static readonly string[] minifiedFileNames = {
            "index.min.js",
            "index.min.js.map"
        };

        private static string[] AllFileNames => basicFileNames.Union(minifiedFileNames).ToArray();


        protected static IEnumerable<ICommand> Copy(Por por, DirectoryInfo outputDir, params DirectoryInfo[] inputDirs)
        {
            var fileNames = por == Por.Basic
                ? basicFileNames
                : por == Por.Minified
                    ? minifiedFileNames
                    : AllFileNames;

            return inputDirs.SelectMany(inputDir =>
            {
                var from = inputDir.MkDir("dist");
                var to = outputDir.MkDir(inputDir.Name);
                return fileNames.Select(file =>
                   new CopyCommand(
                       from.Touch(file),
                       to.Touch(file)));
            });
        }



        protected static IEnumerable<ShellCommand> NPM(string cmd, params DirectoryInfo[] dirs)
        {
            return dirs.Select(dir => new ShellCommand(dir, "npm", "run", cmd));
        }

        protected static ProxiedWatchCommand ProxiedNPM(CommandProxier proxy, params string[] pathParts)
        {
            return new ProxiedWatchCommand(proxy, pathParts);
        }

        private readonly DirectoryInfo projectDir;
        private readonly DirectoryInfo serverDir;
        private readonly DirectoryInfo serverJsDir;
        private readonly DirectoryInfo clientDir;
        private readonly DirectoryInfo clientNodeModules;
        private readonly DirectoryInfo juniperTsDir;
        private readonly FileInfo clientPackage;
        private readonly FileInfo serverBuildInfo;
        private readonly FileInfo serverAppSettings;
        private readonly DirectoryInfo[] juniperProjects;
        private readonly DirectoryInfo[] juniperBundles;
        private readonly Dictionary<FileInfo, FileInfo> dependencies = new();

        public BuildSystem(string clientName, string serverName, string? startDir = null)
        {
            projectDir = ResolveStartDir(ref startDir, clientName, serverName);
            var juniper = projectDir
                .EnumerateDirectories()
                .FirstOrDefault(n => n.Name == "Juniper")
                ?.CD("src", "Juniper.TypeScript");

            if (juniper?.Exists != true)
            {
                throw new FileNotFoundException("Couldn't find Juniper");
            }

            juniperTsDir = juniper;
            clientDir = projectDir.CD(clientName);
            clientNodeModules = clientDir.MkDir("node_modules");
            serverDir = projectDir.CD(serverName);
            serverJsDir = serverDir.MkDir("wwwroot", "js");

            clientPackage = clientDir.Touch("package.json");
            serverBuildInfo = serverDir.Touch("buildinfo.json");
            serverAppSettings = serverDir.Touch("appsettings.json");

            juniperProjects = juniperTsDir
                .EnumerateDirectories()
                .Where(d => d.EnumerateFiles()
                    .Any(f => f.Name == "package.json"))
                .ToArray();
            juniperBundles = juniperProjects
                .Where(d => d.EnumerateFiles()
                    .Any(f => f.Name == "esbuild.config.js"))
                .ToArray();
        }

        public BuildSystem AddDependency(FileInfo from, FileInfo to)
        {
            dependencies.Add(from, to);
            return this;
        }

        public FileInfo From(params string[] parts)
        {
            var file = parts.Last();
            var dirParts = parts.Reverse().Skip(1).Reverse();
            return clientNodeModules.MkDir(dirParts.ToArray()).Touch(file);
        }

        public FileInfo To(params string[] parts)
        {
            var file = parts.Last();
            var dirParts = parts.Reverse().Skip(1).Reverse();
            return serverJsDir.MkDir(dirParts.ToArray()).Touch(file);
        }

        public BuildSystem AddThreeJS()
        {
            AddDependency(From("three", "build", "three.js"), To("three", "index.js"));
            AddDependency(From("three", "build", "three.min.js"), To("three", "index.min.js"));
            return this;
        }

        public BuildSystem AddJQuery()
        {
            AddDependency(From("jquery", "dist", "jquery.js"), To("jquery", "index.js"));
            AddDependency(From("jquery", "dist", "jquery.min.js"), To("jquery", "index.min.js"));
            return this;
        }

        public BuildSystem AddPDFJS()
        {
            AddDependency(From("pdfjs-dist", "build", "pdf.worker.js"), To("pdfjs", "index.js"));
            AddDependency(From("pdfjs-dist", "build", "pdf.worker.min.js"), To("pdfjs", "index.min.js"));
            return this;
        }

        public BuildSystem AddDefaultDependencies()
        {
            AddThreeJS();
            AddJQuery();
            AddPDFJS();
            return this;
        }

        private async Task<Level> GetBuildLevel(bool isDev, Level? forceLevel)
        {
            if (forceLevel is not null)
            {
                return forceLevel.Value;
            }

            if (!serverBuildInfo.Exists)
            {
                return Level.High;
            }

            var v = await Task.WhenAll(
                CopyJsonValueCommand.ReadJsonValueAsync(clientPackage, "version"),
                CopyJsonValueCommand.ReadJsonValueAsync(serverBuildInfo, "Version"));

            return v[0] != v[1]
                ? isDev ? Level.Medium : Level.High
                : Level.Low;
        }

        public async Task CheckAsync(bool isDev, Level? forceLevel)
        {
            var buildLevel = await GetBuildLevel(isDev, forceLevel);
            switch (buildLevel)
            {
                case Level.High:
                Console.WriteLine("Running full install and build");
                await RunFullBuild(true);
                break;

                case Level.Medium:
                Console.WriteLine("Running build");
                await RunFullBuild(false);
                break;

                default:
                Console.WriteLine("Build unnecessary");
                break;
            }
        }

        public BuildSystem Check(bool isDev, Level? forceLevel = null)
        {
            CheckAsync(isDev, forceLevel).Wait();
            return this;
        }

        private async Task RunFullBuild(bool init, bool install = true)
        {
            var commands = new CommandTree();
            commands.Info += (sender, e) =>
            {
                if (sender is ICommand command)
                {
                    Console.WriteLine($"Info [{command.CommandName}]: {e.Value}");
                }
            };

            commands.Warning += (sender, e) =>
            {
                if (sender is ICommand command)
                {
                    Console.WriteLine($"Warning [{command.CommandName}]: {e.Value}");
                }
            };

            commands.Err += (sender, e) =>
            {
                if (sender is ICommand command)
                {
                    Console.Error.WriteLine($"Err [{command.CommandName}]: {e.Value.Unroll()}");
                }
            };

            if (install)
            {
                commands.AddCommands(NPM(init ? "inst" : "update", juniperProjects));
            }

            commands.AddCommands(NPM("build", init ? juniperProjects : juniperBundles))
                .AddCommands(Copy(Por.All, serverJsDir, juniperBundles));

            if (install)
            {
                commands.AddCommands(NPM(init ? "inst" : "update", clientDir));
            }

            commands
                .AddCommands(NPM("build", clientDir)
                    .Cast<ICommand>()
                    .Union(dependencies.Select(kv => new CopyCommand(kv.Key, kv.Value))))
                .AddCommands(
                    new CopyJsonValueCommand(
                        clientPackage, "version",
                        serverAppSettings, "Version"),
                    new CopyJsonValueCommand(
                        clientPackage, "version",
                        serverBuildInfo, "Version"));

            var start = DateTime.Now;
            await commands.ExecuteAsync();
            var end = DateTime.Now;
            var delta = end - start;

            Console.WriteLine($"Done in {delta.TotalSeconds:0.00}s");
        }

        public async Task WriteVersion()
        {
            var version = await CopyJsonValueCommand.ReadJsonValueAsync(clientPackage, "version");
            if (!string.IsNullOrEmpty(version))
            {
                await CopyJsonValueCommand.WriteJsonValueAsync(serverAppSettings, "Version", version);
                Console.WriteLine("Wrote v" + version);
            }
        }

        private static readonly Regex watchAllDonePattern = new("^done in \\d+(\\.\\d+)?s$", RegexOptions.Compiled);
        private static readonly Regex watchBasicDonePattern = new("^browser bundles rebuilt$", RegexOptions.Compiled);
        private static readonly Regex watchMinifiedDonePattern = new("^minified browser bundles rebuilt$", RegexOptions.Compiled);

        public IEnumerable<AbstractShellCommand> GetJuniperProxiedWatchCommands(CommandProxier proxy)
        {
            var relPath = PathExt.Abs2Rel(juniperTsDir.FullName, proxy.Root.FullName);
            var pathParts = relPath.SplitX(Path.DirectorySeparatorChar);
            return juniperBundles
                .Select(dir => ProxiedNPM(proxy, pathParts.Append(dir.Name).ToArray())
                    .OnStandardOutput(
                        watchAllDonePattern,
                        Copy(Por.All, serverJsDir, dir))
                    .OnStandardOutput(
                        watchBasicDonePattern,
                        Copy(Por.Basic, serverJsDir, dir))
                    .OnStandardOutput(
                        watchMinifiedDonePattern,
                        Copy(Por.Minified, serverJsDir, dir)))
                .Append(new ProxiedWatchCommand(proxy, clientDir.Name));
        }

        public Task Watch(out ICommand[] watchCommands)
        {
            var proxy = new CommandProxier(projectDir);

            var relPath = PathExt.Abs2Rel(juniperTsDir.FullName, proxy.Root.FullName);
            var pathParts = relPath.SplitX(Path.DirectorySeparatorChar);
            var cmds = juniperBundles
                .Select(dir => ProxiedNPM(proxy, pathParts.Append(dir.Name).ToArray())
                    .OnStandardOutput(
                        watchAllDonePattern,
                        Copy(Por.All, serverJsDir, dir))
                    .OnStandardOutput(
                        watchBasicDonePattern,
                        Copy(Por.Basic, serverJsDir, dir))
                    .OnStandardOutput(
                        watchMinifiedDonePattern,
                        Copy(Por.Minified, serverJsDir, dir)))
                .Append(new ProxiedWatchCommand(proxy, clientDir.Name))
                .ToArray();

            watchCommands = cmds;

            return Task.WhenAll(cmds.Select(cmd =>
                   cmd.ContinueAfter(watchAllDonePattern)));
        }
    }
}
