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
        public bool IncludeTeleconferencing { get; set; }
        public bool SourceBuildJuniperTS { get; set; }
    }

    public class BuildSystem : ILoggingSource, IDisposable
    {
        delegate void Writer(string format, params object[] args);

        static string Colorize(string tag, int color, string format, params object[] args)
        {
            if (args.Length > 0)
            {
                format = string.Format(format, args);
            }

            return string.Format("\u001b[{0}m{1}:\u001b[0m {2}", color, tag, format);
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

            do
            {
                try
                {
                    if (opts.Interactive)
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
                    else if (opts.DeleteTSBuildInfos)
                    {
                        build.DeleteTSBuildInfos();
                    }
                    else if (opts.NPMInstalls)
                    {
                        await build.NPMInstallsAsync();
                    }
                    else if (opts.NPMAudits)
                    {
                        await build.NPMAuditsAsync();
                    }
                    else if (opts.NPMAuditFixes)
                    {
                        await build.NPMAuditFixesAsync();
                    }
                    else if (opts.OpenPackageJsons)
                    {
                        await build.OpenPackageJsonsAsync();
                    }
                    else if (opts.TypeCheck)
                    {
                        await build.TypeCheckAsync();
                    }
                    else if (!opts.Finished || opts.Build)
                    {
                        await build.BuildAsync();
                    }
                }
                catch (Exception exp)
                {
                    WriteError("{0}\r\n{1}", exp.Message, exp.Unroll());
                }
            } while (!opts.Finished);
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
        private readonly FileInfo projectAppSettings;
        private readonly Dictionary<FileInfo, (string, FileInfo)> dependencies = new();

        private readonly List<DirectoryInfo> TSProjects = new();
        private readonly List<DirectoryInfo> ESBuildProjects = new();
        private readonly List<DirectoryInfo> NPMProjects = new();

        private readonly bool sourceBuildTS;
        private readonly bool hasNPM;

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
            sourceBuildTS = options.SourceBuildJuniperTS;

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

            projectJsDir = projectDir.MkDir("wwwroot", "js");
            projectNodeModules = projectDir.CD("node_modules");
            projectPackage = projectDir.Touch("package.json");
            projectAppSettings = projectDir.Touch("appsettings.json");

            hasNPM = ShellCommand.IsAvailable("npm");

            if (hasNPM)
            {
                var npmProjects = new[]
                {
                    juniperTsDir,
                    projectDir
                };

                foreach (var project in npmProjects)
                {
                    CheckNPMProject(project, options);
                }

                var tsProjects = JuniperSubNPMProjects.Append(projectDir);

                foreach (var project in tsProjects)
                {
                    CheckTSProject(project, options);
                }

                if (options.IncludeThreeJS)
                {
                    AddDependency("Three.js", From("three", "build", "three.js"), To("three", "index.js"));
                    AddDependency("Three.js min", From("three", "build", "three.min.js"), To("three", "index.min.js"));
                }

                if (options.IncludePDFJS)
                {
                    AddDependency("PDFJS", From("pdfjs-dist", "build", "pdf.worker.js"), To("pdfjs", "index.js"));
                    AddDependency("PDFJS map", From("pdfjs-dist", "build", "pdf.worker.js.map"), To("pdfjs", "index.js.map"));
                    AddDependency("PDFJS min", From("pdfjs-dist", "build", "pdf.worker.min.js"), To("pdfjs", "index.min.js"));
                }

                if (options.IncludeJQuery)
                {
                    AddDependency("JQuery", From("jquery", "dist", "jquery.js"), To("jquery", "index.js"));
                    AddDependency("JQuery min", From("jquery", "dist", "jquery.min.js"), To("jquery", "index.min.js"));
                }

                var sourceDir = ((isDev && options.SourceBuildJuniperTS)
                    ? juniperTsDir
                    : projectNodeModules).CD("@juniper-lib");

                if (sourceDir.Exists)
                {
                    if (options.IncludeEnvironment)
                    {
                        AddJuniperDependency(sourceDir, "environment");
                    }

                    if (options.IncludeFetcher)
                    {
                        AddJuniperDependency(sourceDir, "fetcher-worker");
                    }

                    if (options.IncludeTeleconferencing)
                    {
                        AddJuniperDependency(sourceDir, "tele");
                    }
                }
            }

            Info += (sender, e) => WriteInfo(e.Value);
            Warning += (sender, e) => WriteWarning(e.Value);
            Err += (sender, e) => WriteError(e.Value.Unroll());
        }

        private void CheckNPMProject(DirectoryInfo project, BuildSystemOptions options)
        {
            var includeInBuild = project == projectDir || options.SourceBuildJuniperTS;
            if (includeInBuild)
            {
                var pkgFile = project.Touch("package.json");
                if (pkgFile.Exists)
                {
                    NPMProjects.Add(project);
                }
            }
        }

        private void CheckTSProject(DirectoryInfo project, BuildSystemOptions options)
        {
            var includeInBuild = project == projectDir || options.SourceBuildJuniperTS;
            var includeInESBuild = project == projectDir
                    || (options.SourceBuildJuniperTS
                        && ((project.Name == "environment" && options.IncludeEnvironment)
                            || (project.Name == "fetcher-worker" && options.IncludeFetcher)
                            || (project.Name == "tele" && options.IncludeTeleconferencing)));

            if (includeInBuild)
            {
                var tsConfigFile = project.Touch("tsconfig.json");
                if (tsConfigFile.Exists)
                {
                    TSProjects.Add(project);
                }

                if (includeInESBuild)
                {
                    var esbuildFile = project.Touch("esbuild.config.js");
                    if (esbuildFile.Exists)
                    {
                        ESBuildProjects.Add(project);
                    }
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

        private BuildSystem AddJuniperDependency(DirectoryInfo sourceDir, string name)
        {
            var from = sourceDir.CD(name, "dist");
            var to = projectJsDir.CD(name);
            var msg = "Juniper " + name;
            AddDependency(msg, from.Touch("index.js"), to.Touch("index.js"));
            AddDependency(msg + " map", from.Touch("index.js.map"), to.Touch("index.js.map"));
            AddDependency(msg + " min", from.Touch("index.min.js"), to.Touch("index.min.js"));
            AddDependency(msg + " min map", from.Touch("index.min.js.map"), to.Touch("index.min.js.map"));
            return this;
        }

        private IEnumerable<DirectoryInfo> JuniperSubNPMProjects => juniperTsDir
            .EnumerateDirectories()
            .Where(dir => dir.Name.StartsWith("@juniper"))
            .SelectMany(dir => dir.EnumerateDirectories())
            .Where(dir => dir.Touch("package.json").Exists);

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

        private void DeleteDir(DirectoryInfo dir)
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

        private void DeleteFile(FileInfo lockFile)
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

        private void DeleteFiles(IEnumerable<FileInfo> files)
        {
            foreach (var file in files.Where(f => f.Exists))
            {
                DeleteFile(file);
            }
        }

        private void DeleteNodeModuleDirs()
        {
            foreach (var dir in NPMProjects
                .Select(dir => dir.CD("node_modules"))
                .Where(dir => dir.Exists))
            {
                DeleteDir(dir);
            }
        }

        private void DeletePackageLockJsons()
        {
            DeleteFiles(NPMProjects.Select(dir => dir.Touch("package-lock.json")));
        }

        private void DeleteTSBuildInfos()
        {
            DeleteFiles(TSProjects.Select(dir => dir.Touch("tsconfig.tsbuildinfo")));
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

        private IEnumerable<T> TryMake<V, T>(IEnumerable<V> collection, Func<V, T> make) where T : class
        {
            return collection
                .Select(dir => TryMake(() => make(dir)))
                .Where(t => t is not null)
                .Cast<T>();
        }

        private IEnumerable<NPMInstallCommand> GetInstallCommands()
        {
            return TryMake(
                NPMProjects,
                dir => new NPMInstallCommand(dir, true)
            );
        }

        private async Task NPMInstallsAsync()
        {
            await WithCommandTree(commands =>
                commands.AddCommands(GetInstallCommands()));
        }

        private async Task NPMAuditsAsync()
        {
            await WithCommandTree(commands =>
                commands.AddCommands(TryMake(
                    NPMProjects,
                    dir => new ShellCommand(dir, "npm", "audit")
                )));
        }

        private async Task NPMAuditFixesAsync()
        {
            await WithCommandTree(commands =>
                commands.AddCommands(TryMake(
                    NPMProjects,
                    dir => new ShellCommand(dir, "npm", "audit fix")
                )));
        }

        private async Task OpenPackageJsonsAsync()
        {
            await WithCommandTree(commands =>
                commands.AddCommands(TryMake(
                    NPMProjects
                        .Union(JuniperSubNPMProjects)
                        .Select(dir => dir.Touch("package.json"))
                        .Where(f => f.Exists),
                    f => new ShellCommand(f.Directory, "explorer", f.Name)
                )));
        }

        private async Task TypeCheckAsync()
        {
            await WithCommandTree(commands =>
                commands.AddCommands(TryMake(
                    NPMProjects,
                    dir =>
                    {
                        if (dir == projectDir)
                        {
                            return new ShellCommand(dir, "npm", "run", "check");
                        }
                        else
                        {
                            return new ShellCommand(dir, "npm", "run", "check", "--workspaces", "--if-present");
                        }
                    }
                )));
        }

        private CopyCommand[] GetDependecies() =>
            dependencies
                .Select(kv => new CopyCommand(kv.Value.Item1, kv.Key, kv.Value.Item2))
                .ToArray();

        private async Task BuildAsync()
        {
            var copyCommands = GetDependecies();

            await WithCommandTree(commands =>
            {
                var projES = ESBuildProjects.Where(dir => dir == projectDir).ToArray();

                commands
                    .AddCommands(new MessageCommand("Starting build"))
                    .AddCommands(GetInstallCommands());

                if (sourceBuildTS)
                {
                    commands.AddCommands(new ShellCommand(juniperTsDir, "npm", "run", "build", "--workspaces", "--if-present"));
                }

                commands.AddCommands(copyCommands);

                if (projES.Length > 0)
                {
                    commands.AddCommands(TryMake(
                        projES,
                        dir => new ShellCommand(dir, "npm", "run", "build")
                    ));
                }

                commands.AddCommands(
                    new CopyJsonValueCommand(
                        projectPackage, "version",
                        projectAppSettings, "Version"));
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

            var copyCommands = GetDependecies();
            var bundles = TryMake(
                ESBuildProjects.Where(dir => dir == projectDir || sourceBuildTS),
                dir => MakeWatchCommand(proxy, dir)
            ).ToArray();

            await WithCommandTree(commands =>
            {
                commands
                    .AddCommands(new MessageCommand("Starting watch"))
                    .AddCommands(GetInstallCommands())
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
            var args = new List<string>
            {
                "run",
                "watch"
            };

            if (dir != projectDir)
            {
                args.Add("-w");
                args.Add(string.Join("/", dir.Parent?.Name, dir.Name));
                dir = juniperTsDir;
            }

            var cmd = new ProxiedCommand(proxy, dir, "npm", args.ToArray());
            cmd.Info += Proxy_Info;
            cmd.Err += Proxy_Err;
            cmd.Warning += Proxy_Warning;
            return cmd;
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
