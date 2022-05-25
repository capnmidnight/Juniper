using Juniper.Logging;
using Juniper.Processes;
using Juniper.Terminal;
using Juniper.Units;

using System.Text.RegularExpressions;

namespace Juniper.TSBuild
{
    public class BuildSystemProjectRootNotFoundException : DirectoryNotFoundException
    {
        public BuildSystemProjectRootNotFoundException(string message)
            : base(message)
        { }
    }

    public struct BuildSystemOptions
    {
        public bool IncludeThreeJS { get; set; }
        public bool IncludEnvironment { get; set; }
        public bool IncludePDFJS { get; set; }
        public bool IncludeJQuery { get; set; }
        public bool IncludeFetcher { get; set; }
        public bool IncludePhysics { get; set; }
        public bool IncludeTeleconferencing { get; set; }
    }

    public class BuildSystem : ILoggingSource
    {
        delegate void Writer(string format, params object[] args);

        static string Colorize(string tag, int color, string format, params object[] args)
        {
            return string.Format("\u001b[{0}m{1}:\u001b[0m {2}", color, tag, string.Format(format, args));
        }

        static void WriteError(string format, params object[] values)
        {
            Console.Error.WriteLine(Colorize("error", 31, format, values));
        }

        static void WriteInfo(string format, params object[] values)
        {
            Console.WriteLine(Colorize("info", 32, format, values));
        }

        static void WriteWarning(string format, params object[] values)
        {
            Console.WriteLine(Colorize("warn", 33, format, values));
        }

        public static async Task Run(string projectName, BuildSystemOptions options, string[] args)
        {
            var opts = new Options(args);

            var build = new BuildSystem(
                projectName,
                options);

            do
            {
                try
                {
                    if (opts.interactive)
                    {
                        opts.REPL();
                    }

                    if (opts.CleanOnly)
                    {
                        build.DeleteNodeModules();
                    }
                    else if (opts.DeletePackageLockOnly)
                    {
                        build.DeletePackageLocks();
                    }
                    else if (opts.InstallOnly)
                    {
                        await build.InstallAsync();
                    }
                    else if (opts.CheckOnly)
                    {
                        await build.TSBuildAsync();
                    }
                    else if (opts.AuditOnly)
                    {
                        await build.AuditAsync();
                    }
                    else if (opts.AuditFixOnly)
                    {
                        await build.AuditFixAsync();
                    }
                    else if (opts.VersionOnly)
                    {
                        await build.WriteVersion();
                    }
                    else if (opts.level > Level.None)
                    {
                        await build.CheckAsync(false, opts.level);
                    }
                }
                catch (Exception exp)
                {
                    WriteError("{0}\r\n{1}", exp.Message, exp.Unroll());
                }
            } while (!opts.complete);

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

        private static string[] AllFileNames =>
            basicFileNames.Union(minifiedFileNames).ToArray();

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

        private readonly DirectoryInfo projectDir;
        private readonly DirectoryInfo projectJsDir;
        private readonly DirectoryInfo projectNodeModules;
        private readonly DirectoryInfo juniperTsDir;
        private readonly FileInfo projectPackage;
        private readonly FileInfo projectBuildInfo;
        private readonly FileInfo projectAppSettings;
        private readonly Dictionary<FileInfo, FileInfo> dependencies = new();

        private readonly List<DirectoryInfo> TSProjects = new();
        private readonly List<DirectoryInfo> ESBuildProjects = new();
        private readonly List<DirectoryInfo> NPMProjects = new();

        private static DirectoryInfo TestDir(string message, DirectoryInfo? dir)
        {
            if (dir?.Exists != true)
            {
                throw new BuildSystemProjectRootNotFoundException(message);
            }

            return dir;
        }

        public BuildSystem(string projectName, BuildSystemOptions options)
        {
            var startDir = new DirectoryInfo(Environment.CurrentDirectory);

            while (startDir != null
                && !startDir.CD(projectName).Exists)
            {
                startDir = startDir.Parent;
            }

            startDir = TestDir($"Couldn't find project root from {Environment.CurrentDirectory}", startDir);

            var juniperDir = FindJuniperDir(startDir);

            juniperTsDir = TestDir("Couldn't find Juniper TypeScript", juniperDir.CD("src", "Juniper.TypeScript"));
            projectDir = TestDir($"Couldn't find project {projectName} from {startDir}", startDir.CD(projectName));
            projectJsDir = TestDir("Couldn't find project TypeScript", projectDir.MkDir("wwwroot", "js"));

            projectNodeModules = projectDir.MkDir("node_modules");
            projectPackage = projectDir.Touch("package.json");
            projectBuildInfo = projectDir.Touch("buildinfo.json");
            projectAppSettings = projectDir.Touch("appsettings.json");

            var projects = juniperTsDir.EnumerateDirectories()
                .Append(projectDir);

            foreach (var project in projects)
            {
                CheckProject(project, options);
            }

            if (options.IncludeThreeJS)
            {
                AddDependency(From("three", "build", "three.js"), To("three", "index.js"));
                AddDependency(From("three", "build", "three.min.js"), To("three", "index.min.js"));
            }

            if (options.IncludePDFJS)
            {
                AddDependency(From("pdfjs-dist", "build", "pdf.worker.js"), To("pdfjs", "index.js"));
                AddDependency(From("pdfjs-dist", "build", "pdf.worker.min.js"), To("pdfjs", "index.min.js"));
            }

            if (options.IncludeJQuery)
            {
                AddDependency(From("jquery", "dist", "jquery.js"), To("jquery", "index.js"));
                AddDependency(From("jquery", "dist", "jquery.min.js"), To("jquery", "index.min.js"));
            }

            Info += (sender, e) => WriteInfo(e.Value);
            Warning += (sender, e) => WriteWarning(e.Value);
            Err += (sender, e) => WriteError(e.Value.Unroll());
        }

        private void CheckProject(DirectoryInfo project, BuildSystemOptions options)
        {
            var includeInBuild = project == projectDir
                    || (project.Name == "environment" && options.IncludEnvironment)
                    || (project.Name == "fetcher-worker" && options.IncludeFetcher)
                    || (project.Name == "physics-worker" && options.IncludePhysics)
                    || (project.Name == "tele" && options.IncludeTeleconferencing);

            var pkgFile = project.Touch("package.json");
            if (pkgFile.Exists)
            {
                NPMProjects.Add(project);
            }

            var tsConfigFile = project.Touch("tsconfig.json");
            if (tsConfigFile.Exists)
            {
                TSProjects.Add(project);
            }

            if (includeInBuild)
            {
                var esbuildFile = project.Touch("esbuild.config.js");
                if (esbuildFile.Exists)
                {
                    ESBuildProjects.Add(project);
                }
            }
        }

        private static DirectoryInfo FindJuniperDir(DirectoryInfo? startDir)
        {
            while (startDir is not null)
            {
                var test = startDir.CD("Juniper");
                if (test.Exists)
                {
                    return test;
                }

                startDir = startDir.Parent;
            }

            throw new Exception("Couldn't find Juniper");
        }

        public BuildSystem AddDependency(FileInfo from, FileInfo to)
        {
            dependencies.Add(from, to);
            return this;
        }

        private static FileInfo R(DirectoryInfo dir, params string[] parts)
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

        private async Task WithCommandTree(Action<CommandTree> buildTree)
        {
            var commands = new CommandTree();
            commands.Info += (sender, e) =>
            {
                if (sender is ICommand command)
                {
                    OnInfo($"[{command.CommandName}]: {e.Value}");
                }
                else
                {
                    OnInfo(e.Value);
                }
            };

            commands.Warning += (sender, e) =>
            {
                if (sender is ICommand command)
                {
                    OnWarning($"[{command.CommandName}]: {e.Value}");
                }
                else
                {
                    OnWarning(e.Value);
                }
            };

            commands.Err += (sender, e) =>
            {
                if (sender is ICommand command)
                {
                    OnError(new Exception(command.CommandName, e.Value));
                }
                else
                {
                    OnError(e.Value);
                }
            };

            buildTree(commands);

            var start = DateTime.Now;
            await commands.ExecuteAsync();
            var end = DateTime.Now;
            var delta = end - start;

            OnInfo($"Build finished in {delta.TotalSeconds:0.00}s");
        }

        public void DeleteNodeModules()
        {
            foreach (var dir in NPMProjects)
            {
                for (int attempts = 2; attempts > 0; attempts--)
                {
                    try
                    {
                        dir.Delete(true);
                        OnInfo($"{dir.FullName} deleted");
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (attempts == 1)
                        {
                            OnWarning($"Could not delete {dir.FullName}. Reason: {ex.Message}.");
                        }
                    }
                }
            }
        }

        public void DeletePackageLocks()
        {
            foreach (var lockFile in NPMProjects
                .Select(dir => dir.Touch("package-lock.json"))
                .Where(f => f.Exists))
            {
                for (int attempts = 2; attempts > 0; attempts--)
                {
                    try
                    {
                        lockFile.Delete();
                        OnInfo($"{lockFile.FullName} deleted");
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (attempts == 1)
                        {
                            OnWarning($"Could not delete {lockFile.FullName}. Reason: {ex.Message}.");
                        }
                    }
                }
            }
        }

        private IEnumerable<NPMInstallCommand> GetInstallCommands(Level buildLevel)
        {
            return NPMProjects.Select(dir =>
                new NPMInstallCommand(dir, buildLevel == Level.High));
        }

        public async Task InstallAsync()
        {
            await WithCommandTree(commands =>
                commands.AddCommands(GetInstallCommands(Level.High)));
        }

        public async Task TSBuildAsync()
        {
            await WithCommandTree(commands =>
                commands.AddCommands(TSProjects.Select(dir =>
                    new TSBuildCommand(dir))));
        }

        public async Task AuditAsync()
        {
            await WithCommandTree(commands =>
                commands.AddCommands(NPMProjects.Select(dir =>
                    new ShellCommand(dir, "npm", "audit"))));
        }

        public async Task AuditFixAsync()
        {
            await WithCommandTree(commands =>
                commands.AddCommands(NPMProjects.Select(dir =>
                    new ShellCommand(dir, "npm", "audit fix"))));
        }


        private static readonly Dictionary<Level, string> buildLevelMessages = new()
        {
            { Level.High, "Running full install and build" },
            { Level.Medium, "Running update and build" },
            { Level.Low, "No build" },
            { Level.None, "No build" }
        };

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
                    var juniperBundles = ESBuildProjects
                            .Where(dir => dir.FullName != projectDir.FullName)
                            .ToList();
                    commands
                        .AddCommands(juniperBundles
                            .Select(dir => new ShellCommand(dir, "npm", "run build")))
                        .AddCommands(Copy(Por.All, projectJsDir, juniperBundles));

                    if (projectDir.Touch("package.json").Exists)
                    {
                        commands.AddCommands(new ShellCommand(projectDir, "npm", "run build"));
                    }

                    commands.AddCommands(
                        new CopyJsonValueCommand(
                            projectPackage, "version",
                            projectAppSettings, "Version"),
                        new CopyJsonValueCommand(
                            projectPackage, "version",
                            projectBuildInfo, "Version"));
                }
            });
        }

        public async Task Watch()
        {
            var proxy = new CommandProxier(projectDir);
            proxy.Info += Proxy_Info;
            proxy.Warning += Proxy_Warning;
            proxy.Err += Proxy_Err;
            await proxy.Start();

            await CheckAsync(true, Level.None);

            var bundles = ESBuildProjects
                .Select(dir => MakeWatchCommand(proxy, dir))
                .ToArray();

            var tasks = bundles
                .Select(bundle =>
                {
                    var taskCompleter = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                    bundle.OnStandardOutput(watchAllDonePattern, () =>
                    {
                        taskCompleter?.SetResult();
                        taskCompleter = null;
                    });
                    return taskCompleter.Task;
                })
                .ToArray();

            var runAll = Task.WhenAll(bundles
                .Select(b => b.RunSafeAsync())
                .ToArray());

            await Task.WhenAll(tasks);
        }

        private AbstractShellCommand MakeWatchCommand(CommandProxier proxy, DirectoryInfo dir)
        {
            var parts = PathExt.Abs2Rel(dir.FullName, proxy.Root.FullName);
            var cmd = new ProxiedWatchCommand(proxy, parts);
            cmd.Info += Proxy_Info;
            cmd.Err += Proxy_Err;
            cmd.Warning += Proxy_Warning;
            if (dir != projectDir)
            {
                cmd.OnStandardOutput(
                    watchAllDonePattern,
                    Copy(Por.All, projectJsDir, dir))
                .OnStandardOutput(
                    watchBasicDonePattern,
                    Copy(Por.Basic, projectJsDir, dir))
                .OnStandardOutput(
                    watchMinifiedDonePattern,
                    Copy(Por.Minified, projectJsDir, dir));
            }
            return cmd;
        }

        public async Task WriteVersion()
        {
            var version = await CopyJsonValueCommand.ReadJsonValueAsync(projectPackage, "version");
            if (!string.IsNullOrEmpty(version))
            {
                await CopyJsonValueCommand.WriteJsonValueAsync(projectAppSettings, "Version", version);
                OnInfo("Wrote v" + version);
            }
        }

        private static readonly Regex watchAllDonePattern = new("^done in \\d+(\\.\\d+)?s$", RegexOptions.Compiled);
        private static readonly Regex watchBasicDonePattern = new("^browser bundles rebuilt$", RegexOptions.Compiled);
        private static readonly Regex watchMinifiedDonePattern = new("^minified browser bundles rebuilt$", RegexOptions.Compiled);

        public event EventHandler<StringEventArgs>? Info;
        public event EventHandler<StringEventArgs>? Warning;
        public event EventHandler<ErrorEventArgs>? Err;

        private void OnInfo(string message) => Info?.Invoke(this, new StringEventArgs(message));
        private void Proxy_Info(object? sender, StringEventArgs e)
        {
            if (sender is ICommand command)
            {
                OnInfo($"[{command.CommandName}]: {e.Value}");
            }
            else
            {
                OnInfo(e.Value);
            }
        }

        private void OnWarning(string message) => Warning?.Invoke(this, new StringEventArgs(message));
        private void Proxy_Warning(object? sender, StringEventArgs e)
        {
            if (sender is ICommand command)
            {
                OnWarning($"[{command.CommandName}]: {e.Value}");
            }
            else
            {
                OnWarning(e.Value);
            }
        }

        private void OnError(Exception exp) => Err?.Invoke(this, new ErrorEventArgs(exp));
        private void Proxy_Err(object? sender, ErrorEventArgs e)
        {
            if (sender is ICommand command)
            {
                OnError(new Exception($"[{command.CommandName}]:", e.Value));
            }
            else
            {
                OnError(e.Value);
            }
        }
    }
}
