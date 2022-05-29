using Juniper.Logging;
using Juniper.Processes;
using Juniper.Units;

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
        public bool IncludeEnvironment { get; set; }
        public bool IncludePDFJS { get; set; }
        public bool IncludeJQuery { get; set; }
        public bool IncludeFetcher { get; set; }
        public bool IncludePhysics { get; set; }
        public bool IncludeTeleconferencing { get; set; }
        public bool SourceBuildJuniperTS { get; set; }
    }

    public class BuildSystem : ILoggingSource, IDisposable
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

            using var build = new BuildSystem(
                projectName,
                options,
                false,
                opts.workingDir);

            opts.level = await build.GetBuildLevel(opts.level);

            do
            {
                try
                {
                    if (opts.interactive)
                    {
                        opts.REPL();
                    }

                    if (opts.DeleteNodeModuleDirs)
                    {
                        build.DeleteNodeModuleDirs();
                    }
                    else if (opts.DeletePackageLockJsons)
                    {
                        build.DeletePackageLockJsons();
                    }
                    else if (opts.NPMInstalls)
                    {
                        await build.NPMInstallsAsync();
                    }
                    else if (opts.TSChecks)
                    {
                        await build.TSChecksAsync();
                    }
                    else if (opts.NPMAudits)
                    {
                        await build.NPMAuditsAsync();
                    }
                    else if (opts.NPMAuditFixes)
                    {
                        await build.NPMAuditFixesAsync();
                    }
                    else if (opts.WriteVersion)
                    {
                        await build.WriteVersion();
                    }
                    else if (opts.OpenPackageJsons)
                    {
                        await build.OpenPackageJsonsAsync();
                    }
                    else if (opts.level > Level.None)
                    {
                        await build.CheckAsync(opts.level);
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

        private readonly DirectoryInfo projectDir;
        private readonly DirectoryInfo projectJsDir;
        private readonly DirectoryInfo projectNodeModules;
        private readonly DirectoryInfo juniperTsDir;
        private readonly FileInfo projectPackage;
        private readonly FileInfo projectBuildInfo;
        private readonly FileInfo projectAppSettings;
        private readonly Dictionary<FileInfo, (string, FileInfo)> dependencies = new();

        private readonly List<DirectoryInfo> TSProjects = new();
        private readonly List<DirectoryInfo> ESBuildProjects = new();
        private readonly List<DirectoryInfo> NPMProjects = new();

        private readonly bool isDev;
        private readonly bool sourceBuildTS;

        private Timer? timer;
        private bool disposedValue;

        private static DirectoryInfo TestDir(string message, DirectoryInfo? dir)
        {
            if (dir?.Exists != true)
            {
                throw new BuildSystemProjectRootNotFoundException(message);
            }

            return dir;
        }

        public BuildSystem(string projectName, BuildSystemOptions options, bool isDev, DirectoryInfo? workingDir)
        {
            this.isDev = isDev;
            this.sourceBuildTS = options.SourceBuildJuniperTS;

            workingDir ??= new DirectoryInfo(Environment.CurrentDirectory);
            var startDir = workingDir;

            while (startDir != null
                && !startDir.CD(projectName).Exists)
            {
                startDir = startDir.Parent;
            }

            startDir = TestDir($"Couldn't find project root from {workingDir.FullName}", startDir);

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
                AddDependency("Three.js", From("three", "build", "three.js"), To("three", "index.js"));
                AddDependency("Three.js min", From("three", "build", "three.min.js"), To("three", "index.min.js"));
            }

            if (options.IncludePDFJS)
            {
                AddDependency("PDFJS", From("pdfjs-dist", "build", "pdf.worker.js"), To("pdfjs", "index.js"));
                AddDependency("PDFJS min", From("pdfjs-dist", "build", "pdf.worker.min.js"), To("pdfjs", "index.min.js"));
            }

            if (options.IncludeJQuery)
            {
                AddDependency("JQuery", From("jquery", "dist", "jquery.js"), To("jquery", "index.js"));
                AddDependency("JQuery min", From("jquery", "dist", "jquery.min.js"), To("jquery", "index.min.js"));
            }

            if (options.IncludeEnvironment)
            {
                if (isDev && options.SourceBuildJuniperTS)
                {
                    AddDependency("Juniper Environment", From(juniperTsDir, "environment", "dist", "index.js"), To("environment", "index.js"));
                    AddDependency("Juniper Environment min", From(juniperTsDir, "environment", "dist", "index.min.js"), To("environment", "index.min.js"));
                }
                else
                {
                    AddDependency("Juniper Environment", From("@juniper-lib", "environment", "dist", "index.js"), To("environment", "index.js"));
                    AddDependency("Juniper Environment min", From("@juniper-lib", "environment", "dist", "index.min.js"), To("environment", "index.min.js"));
                }
            }

            if (options.IncludeFetcher)
            {
                if (isDev && options.SourceBuildJuniperTS)
                {
                    AddDependency("Juniper Fetcher", From(juniperTsDir, "fetcher-worker", "dist", "index.js"), To("fetcher-worker", "index.js"));
                    AddDependency("Juniper Fetcher min", From(juniperTsDir, "fetcher-worker", "dist", "index.min.js"), To("fetcher-worker", "index.min.js"));
                }
                else
                {
                    AddDependency("Juniper Fetcher", From("@juniper-lib", "fetcher-worker", "dist", "index.js"), To("fetcher-worker", "index.js"));
                    AddDependency("Juniper Fetcher min", From("@juniper-lib", "fetcher-worker", "dist", "index.min.js"), To("fetcher-worker", "index.min.js"));
                }
            }

            if (options.IncludePhysics)
            {
                if (isDev && options.SourceBuildJuniperTS)
                {
                    AddDependency("Juniper Physics", From(juniperTsDir, "physics-worker", "dist", "index.js"), To("physics-worker", "index.js"));
                    AddDependency("Juniper Physics min", From(juniperTsDir, "physics-worker", "dist", "index.min.js"), To("physics-worker", "index.min.js"));
                }
                else
                {
                    AddDependency("Juniper Physics", From("@juniper-lib", "physics-worker", "dist", "index.js"), To("physics-worker", "index.js"));
                    AddDependency("Juniper Physics min", From("@juniper-lib", "physics-worker", "dist", "index.min.js"), To("physics-worker", "index.min.js"));
                }
            }

            if (options.IncludeTeleconferencing)
            {
                if (isDev && options.SourceBuildJuniperTS)
                {
                    AddDependency("Juniper Teleconferencing", From(juniperTsDir, "tele", "dist", "index.js"), To("tele", "index.js"));
                    AddDependency("Juniper Teleconferencing min", From(juniperTsDir, "tele", "dist", "index.min.js"), To("tele", "index.min.js"));
                }
                else
                {
                    AddDependency("Juniper Teleconferencing", From("@juniper-lib", "tele", "dist", "index.js"), To("tele", "index.js"));
                    AddDependency("Juniper Teleconferencing min", From("@juniper-lib", "tele", "dist", "index.min.js"), To("tele", "index.min.js"));
                }
            }

            Info += (sender, e) => WriteInfo(e.Value);
            Warning += (sender, e) => WriteWarning(e.Value);
            Err += (sender, e) => WriteError(e.Value.Unroll());
        }

        private void CheckProject(DirectoryInfo project, BuildSystemOptions options)
        {
            var includeInBuild = project == projectDir
                    || (project.Name == "environment" && options.IncludeEnvironment)
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

        public BuildSystem AddDependency(string name, FileInfo from, FileInfo to)
        {
            dependencies.Add(from, (name, to));
            return this;
        }

        private static FileInfo R(DirectoryInfo dir, params string[] parts)
        {
            return dir.CD(parts[0..^1]).Touch(parts[^1]);
        }

        public static FileInfo From(DirectoryInfo root, params string[] parts)
        {
            return R(root, parts);
        }

        public FileInfo From(params string[] parts)
        {
            return From(projectNodeModules, parts);
        }

        public static FileInfo To(DirectoryInfo root, params string[] parts)
        {
            return R(root, parts);
        }

        public FileInfo To(params string[] parts)
        {
            return To(projectJsDir, parts);
        }

        private async Task<Level> GetBuildLevel(Level forceLevel)
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
                ? this.isDev ? Level.Medium : Level.High
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

        public void DeleteNodeModuleDirs()
        {
            foreach (var dir in NPMProjects
                .Select(dir => dir.CD("node_modules"))
                .Where(dir => dir.Exists))
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

        public void DeletePackageLockJsons()
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

        private T? TryMake<T>(Func<T> make) where T : class
        {
            try
            {
                return make();
            }
            catch (ShellCommandNotFoundException err)
            {
                OnError(err);
                return null;
            }
        }

        public IEnumerable<T> TryMake<V, T>(IEnumerable<V> collection, Func<V, T> make) where T : class
        {
            return collection
                .Select(dir => TryMake(() => make(dir)))
                .Where(t => t is not null)
                .Cast<T>();
        }

        private IEnumerable<NPMInstallCommand> GetInstallCommands(Level buildLevel)
        {
            return TryMake(
                NPMProjects, 
                dir => new NPMInstallCommand(dir, buildLevel == Level.High)
            );
        }

        public async Task NPMInstallsAsync()
        {
            await WithCommandTree(commands =>
                commands.AddCommands(GetInstallCommands(Level.High)));
        }

        public async Task TSChecksAsync()
        {
            await WithCommandTree(commands =>
                commands.AddCommands(TryMake(
                    TSProjects, 
                    dir => new TSBuildCommand(dir)
                )));
        }

        public async Task NPMAuditsAsync()
        {
            await WithCommandTree(commands =>
                commands.AddCommands(TryMake(
                    NPMProjects, 
                    dir => new ShellCommand(dir, "npm", "audit")
                )));
        }

        public async Task NPMAuditFixesAsync()
        {
            await WithCommandTree(commands =>
                commands.AddCommands(TryMake(
                    NPMProjects, 
                    dir => new ShellCommand(dir, "npm", "audit fix")
                )));
        }

        public async Task OpenPackageJsonsAsync()
        {
            await WithCommandTree(commands =>
                commands.AddCommands(TryMake(
                    NPMProjects.Select(dir => dir.Touch("package.json")).Where(f => f.Exists),
                    f => new ShellCommand(f.Directory, "explorer", f.Name)
                )));
        }


        private static readonly Dictionary<Level, string> buildLevelMessages = new()
        {
            { Level.High, "Running full install and build" },
            { Level.Medium, "Running update and build" },
            { Level.Low, "No build" },
            { Level.None, "No build" }
        };

        private CopyCommand[] GetDependecies() =>
            dependencies
                .Select(kv => new CopyCommand(kv.Value.Item1, kv.Key, kv.Value.Item2))
                .ToArray();

        public async Task CheckAsync(Level buildLevel)
        {
            var copyCommands = GetDependecies();

            await WithCommandTree(commands =>
            {
                commands
                    .AddCommands(new MessageCommand("Build level {0}: {1}", buildLevel, buildLevelMessages[buildLevel]))
                    .AddCommands(GetInstallCommands(buildLevel))
                    .AddCommands(copyCommands);

                if (buildLevel > Level.Low)
                {
                    commands
                        .AddCommands(TryMake(
                            ESBuildProjects, 
                            dir => new ShellCommand(dir, "npm", "run build")
                        ))
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

        public void Watch()
        {
            WatchAsync().Wait();
        }

        public async Task WatchAsync()
        {
            var proxy = new CommandProxier(projectDir);
            proxy.Info += Proxy_Info;
            proxy.Warning += Proxy_Warning;
            proxy.Err += Proxy_Err;
            await proxy.Start();

            var buildLevel = await GetBuildLevel(Level.None);
            var copyCommands = GetDependecies();
            var bundles = TryMake(
                ESBuildProjects.Where(dir => dir == projectDir || sourceBuildTS),
                dir => MakeWatchCommand(proxy, dir)
            ).ToArray();

            await WithCommandTree(commands =>
            {
                commands
                    .AddCommands(new MessageCommand("Build level {0}: {1}", buildLevel, buildLevelMessages[buildLevel]))
                    .AddCommands(GetInstallCommands(buildLevel))
                    .AddCommands(copyCommands);
            });

            foreach (var b in bundles)
            {
                _ = b.RunSafeAsync();
            }

            timer = new Timer(new TimerCallback((_) =>
            {
                foreach (var dep in copyCommands)
                {
                    if (dep.Recheck())
                    {
                        OnInfo(Colorize("watch", 36, "{0} copied", dep.CommandName));
                    };
                }
            }), null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3));
        }

        private AbstractShellCommand MakeWatchCommand(CommandProxier proxy, DirectoryInfo dir)
        {
            var parts = PathExt.Abs2Rel(dir.FullName, proxy.Root.FullName);
            var cmd = new ProxiedWatchCommand(proxy, parts);
            cmd.Info += Proxy_Info;
            cmd.Err += Proxy_Err;
            cmd.Warning += Proxy_Warning;
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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    timer?.Dispose();
                    timer = null;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
