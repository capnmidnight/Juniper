using Juniper.Processes;
using Juniper.TSWatcher;
using Juniper.Units;

using System.Text.Json;
using System.Text.RegularExpressions;

namespace Juniper.TSBuild
{
    public class BuildSystemProjectRootNotFoundException : DirectoryNotFoundException
    {
        public BuildSystemProjectRootNotFoundException(string message)
            : base(message)
        { }
    }

    public class BuildSystem
    {
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
            return Copy(por, outputDir, inputDirs.AsEnumerable());
        }

        protected static IEnumerable<ICommand> Copy(Por por, DirectoryInfo outputDir, IEnumerable<DirectoryInfo> inputDirs)
        {
            var fileNames = por == Por.Basic
                ? basicFileNames
                : por == Por.Minified
                    ? minifiedFileNames
                    : AllFileNames;

            return from inputDir in inputDirs
                   let fromDir = inputDir.MkDir("dist")
                   let toDir = outputDir.MkDir(inputDir.Name)
                   from file in fileNames
                   select new CopyCommand(fromDir.Touch(file), toDir.Touch(file));
        }

        protected static IEnumerable<ICommand> Delete(params FileInfo[] files)
        {
            return Delete(files.AsEnumerable());
        }

        protected static IEnumerable<ICommand> Delete(IEnumerable<FileInfo> files)
        {
            return from file in files
                   where file.Exists
                   select new DeleteCommand(file);
        }

        protected static IEnumerable<ShellCommand> NPM(string cmd, params DirectoryInfo[] dirs)
        {
            return NPM(cmd, dirs.AsEnumerable());
        }

        protected static IEnumerable<ShellCommand> NPM(string cmd, IEnumerable<DirectoryInfo> dirs)
        {
            return from dir in dirs
                   select new ShellCommand(dir, "npm", cmd);
        }

        protected static ProxiedWatchCommand ProxiedNPM(CommandProxier proxy, params string[] pathParts)
        {
            return new ProxiedWatchCommand(proxy, pathParts);
        }

        protected static ProxiedWatchCommand ProxiedNPM(CommandProxier proxy, IEnumerable<string> pathParts)
        {
            return ProxiedNPM(proxy, pathParts.ToArray());
        }

        private readonly DirectoryInfo projectDir;
        private readonly DirectoryInfo projectJsDir;
        private readonly DirectoryInfo projectNodeModules;
        private readonly DirectoryInfo juniperTsDir;
        private readonly FileInfo projectPackage;
        private readonly FileInfo projectBuildInfo;
        private readonly FileInfo projectAppSettings;
        private readonly List<DirectoryInfo> juniperInstallables = new();
        private readonly List<DirectoryInfo> juniperBuildables = new();
        private readonly List<DirectoryInfo> juniperBundles = new();
        private readonly Dictionary<FileInfo, FileInfo> dependencies = new();

        private DirectoryInfo TestDir(string message, DirectoryInfo? dir)
        {
            if(dir?.Exists != true)
            {
                throw new BuildSystemProjectRootNotFoundException(message);
            }

            return dir;
        }

        public BuildSystem(string projectName, DirectoryInfo? startDir)
        {
            var originalStartDir = startDir;
            startDir ??= new DirectoryInfo(Environment.CurrentDirectory);

            while (startDir != null
                && !startDir.GetDirectories()
                    .Select(x => x.Name)
                    .Contains(projectName))
            {
                startDir = startDir.Parent;
            }

            startDir = TestDir($"Couldn't find project root from {originalStartDir}", startDir);

            var juniperDir = TestDir("Couldn't find Juniper", startDir
                .EnumerateDirectories()
                .FirstOrDefault(n => n.Name == "Juniper"));

            juniperTsDir = TestDir("Couldn't find Juniper TypeScript", juniperDir.CD("src", "Juniper.TypeScript"));
            projectDir = TestDir($"Couldn't find project {projectName} from {startDir}", startDir.CD(projectName));
            projectJsDir = TestDir("Couldn't find project TypeScript", projectDir.MkDir("wwwroot", "js"));

            projectNodeModules = projectDir.MkDir("node_modules");
            projectPackage = projectDir.Touch("package.json");
            projectBuildInfo = projectDir.Touch("buildinfo.json");
            projectAppSettings = projectDir.Touch("appsettings.json");

            var projects = juniperTsDir
                .EnumerateDirectories()
                .Where(d => d.Touch("package.json").Exists);

            foreach(var project in projects)
            {
                var pkgFile = project.Touch("package.json");
                using var pkgStream = pkgFile.OpenRead();
                var pkg = JsonSerializer.Deserialize<NPMPackage>(pkgStream);
                if(pkg?.types is null)
                {
                    juniperInstallables.Add(project);
                }

                if(pkg?.scripts?.ContainsKey("build") == true)
                {
                    juniperBuildables.Add(project);
                }

                if(project.EnumerateFiles().Any(f => f.Name == "esbuild.config.js"))
                {
                    juniperBundles.Add(project);
                }
            }
        }

        public BuildSystem AddDependency(FileInfo from, FileInfo to)
        {
            dependencies.Add(from, to);
            return this;
        }

        private FileInfo R(DirectoryInfo dir, params string[] parts)
        {
            return dir.MkDir(parts[0..^1]).Touch(parts[^1]);
        }

        public FileInfo From(params string[] parts)
        {
            return R(projectNodeModules, parts);
        }

        public FileInfo To(params string[] parts)
        {
            return R(projectJsDir, parts);
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

        private async Task<Level> GetBuildLevel(bool isDev, Level forceLevel)
        {
            if (forceLevel > Level.None)
            {
                return forceLevel;
            }

            if (!projectBuildInfo.Exists
                || !projectNodeModules.Exists)
            {
                return Level.High;
            }

            var v = await Task.WhenAll(
                CopyJsonValueCommand.ReadJsonValueAsync(projectPackage, "version"),
                CopyJsonValueCommand.ReadJsonValueAsync(projectBuildInfo, "Version"));

            return v[0] != v[1]
                ? isDev ? Level.Medium : Level.High
                : Level.Low;
        }

        private static async Task WithCommandTree(Action<CommandTree> buildTree)
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

            buildTree(commands);

            var start = DateTime.Now;
            await commands.ExecuteAsync();
            var end = DateTime.Now;
            var delta = end - start;

            Console.WriteLine($"Done in {delta.TotalSeconds:0.00}s");
        }

        private IEnumerable<NPMInstallCommand> GetInstallCommands(Level buildLevel)
        {
            var projects = buildLevel == Level.High
                ? juniperInstallables
                : juniperBuildables;

            return projects
                .Append(projectDir)
                .Select(dir =>
                    new NPMInstallCommand(dir, buildLevel == Level.High));
        }

        public async Task InstallAsync()
        {
            await WithCommandTree(commands =>
                commands.AddCommands(GetInstallCommands(Level.High)));
        }

        public async Task AuditAsync()
        {
            await WithCommandTree(commands =>
                commands.AddCommands(NPM("audit", juniperBuildables
                    .Append(projectDir))));
        }

        public async Task AuditFixAsync()
        {
            await WithCommandTree(commands =>
                commands.AddCommands(NPM("audit fix", juniperBuildables
                    .Append(projectDir))));
        }


        private static readonly Dictionary<Level, string> buildLevelMessages = new()
        {
            { Level.High, "Running full install and build" },
            { Level.Medium, "Running update and build" },
            { Level.Low, "No build" },
            { Level.None, "No build" }
        };

        public BuildSystem Check(bool isDev, Level forceLevel = Level.None)
        {
            CheckAsync(isDev, forceLevel).Wait();
            return this;
        }

        public async Task CheckAsync(bool isDev, Level forceLevel)
        {
            var buildLevel = await GetBuildLevel(isDev, forceLevel);

            await WithCommandTree(commands =>
            {
                commands
                    .AddCommands(new MessageCommand("Build level {0}: {1}", buildLevel, buildLevelMessages[buildLevel]))
                    .AddCommands(GetInstallCommands(buildLevel))
                    .AddCommands(dependencies.Select(kv => new CopyCommand(kv.Key, kv.Value)));

                if (buildLevel > Level.Low)
                {
                    commands
                        .AddCommands(Delete(juniperBuildables
                            .Append(projectDir)
                            .Select(d => d.Touch("tsconfig.tsbuildinfo"))))
                        .AddCommands(NPM("run build", juniperBuildables))
                        .AddCommands(Copy(Por.All, projectJsDir, juniperBundles))
                        .AddCommands(NPM("run build", projectDir))
                        .AddCommands(
                            new CopyJsonValueCommand(
                                projectPackage, "version",
                                projectAppSettings, "Version"),
                            new CopyJsonValueCommand(
                                projectPackage, "version",
                                projectBuildInfo, "Version"));
                }
            });
        }

        public async Task WriteVersion()
        {
            var version = await CopyJsonValueCommand.ReadJsonValueAsync(projectPackage, "version");
            if (!string.IsNullOrEmpty(version))
            {
                await CopyJsonValueCommand.WriteJsonValueAsync(projectAppSettings, "Version", version);
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
                        Copy(Por.All, projectJsDir, dir))
                    .OnStandardOutput(
                        watchBasicDonePattern,
                        Copy(Por.Basic, projectJsDir, dir))
                    .OnStandardOutput(
                        watchMinifiedDonePattern,
                        Copy(Por.Minified, projectJsDir, dir)))
                .Append(new ProxiedWatchCommand(proxy, projectDir.Name));
        }

        public Task Watch(out ICommand[] watchCommands)
        {
            var proxy = new CommandProxier(projectDir);

            var juniperRelPath = PathExt.Abs2Rel(juniperTsDir.FullName, proxy.Root.FullName);
            var juniperPathParts = juniperRelPath.SplitX(Path.DirectorySeparatorChar);
            var projectRelPath = PathExt.Abs2Rel(projectDir.FullName, proxy.Root.FullName);
            var projectPathParts = projectRelPath.SplitX(Path.DirectorySeparatorChar);
            var cmds = juniperBundles
                .Select(dir => ProxiedNPM(proxy, juniperPathParts.Append(dir.Name).ToArray())
                    .OnStandardOutput(
                        watchAllDonePattern,
                        Copy(Por.All, projectJsDir, dir))
                    .OnStandardOutput(
                        watchBasicDonePattern,
                        Copy(Por.Basic, projectJsDir, dir))
                    .OnStandardOutput(
                        watchMinifiedDonePattern,
                        Copy(Por.Minified, projectJsDir, dir)))
                .Append(new ProxiedWatchCommand(proxy, projectPathParts))
                .ToArray();

            watchCommands = cmds;

            return Task.WhenAll(cmds.Select(cmd =>
                   cmd.ContinueAfter(watchAllDonePattern)));
        }
    }
}
