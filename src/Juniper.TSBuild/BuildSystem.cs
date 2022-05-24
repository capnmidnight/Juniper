using Juniper.Logging;
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

    public class BuildSystem : ILoggingSource
    {
        public static async Task Run(string projectName, string[] args)
        {
            var opts = new Options(args);

            var build = new BuildSystem(
                projectName,
                opts.workingDir);

            build.Info += (sender, e) =>
            {
                if (sender is ICommand command)
                {
                    Console.WriteLine($"Build Info [{command.CommandName}]: {e.Value}");
                }
                else
                {
                    Console.WriteLine($"Build Info: {e.Value}");
                }
            };

            build.Warning += (sender, e) =>
            {
                if (sender is ICommand command)
                {
                    Console.WriteLine($"Build Warning [{command.CommandName}]: {e.Value}");
                }
                else
                {
                    Console.WriteLine($"Build Warning: {e.Value}");
                }
            };

            build.Err += (sender, e) =>
            {
                if (sender is ICommand command)
                {
                    Console.Error.WriteLine($"Build Err [{command.CommandName}]: {e.Value.Unroll()}");
                }
                else
                {
                    Console.Error.WriteLine($"Build Err: {e.Value.Unroll()}");
                }
            };

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
                        await build
                            .AddDefaultDependencies()
                            .CheckAsync(false, opts.level);
                    }
                }
                catch (Exception exp)
                {
                    Console.Error.WriteLine("ERR: {0}\r\n{1}", exp.Message, exp.Unroll());
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
        private readonly List<DirectoryInfo> juniperTSProjects = new();
        private readonly List<DirectoryInfo> juniperBuildables = new();
        private readonly List<DirectoryInfo> juniperBundles = new();
        private readonly Dictionary<FileInfo, FileInfo> dependencies = new();

        private DirectoryInfo TestDir(string message, DirectoryInfo? dir)
        {
            if (dir?.Exists != true)
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
                && !startDir.EnumerateDirectories()
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

            var projects = juniperTsDir.EnumerateDirectories();

            foreach (var project in projects)
            {
                var pkgFile = project.Touch("package.json");
                if (pkgFile.Exists)
                {
                    using var pkgStream = pkgFile.OpenRead();
                    var pkg = JsonSerializer.Deserialize<NPMPackage>(pkgStream);
                    if (pkg is null)
                    {
                        continue;
                    }

                    if ((pkg.dependencies?.Count ?? 0) > 0
                        || (pkg.devDependencies?.Count ?? 0) > 0)
                    {
                        juniperInstallables.Add(project);
                    }

                    if (pkg.scripts?.ContainsKey("build") == true)
                    {
                        juniperBuildables.Add(project);
                    }

                    if (project.EnumerateFiles().Any(f => f.Name == "esbuild.config.js"))
                    {
                        juniperBundles.Add(project);
                    }
                }

                var tsConfigFile = project.Touch("tsconfig.json");
                if (tsConfigFile.Exists)
                {
                    juniperTSProjects.Add(project);
                }
            }
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
                else
                {
                    Console.WriteLine($"Info: {e.Value}");
                }
            };

            commands.Warning += (sender, e) =>
            {
                if (sender is ICommand command)
                {
                    Console.WriteLine($"Warning [{command.CommandName}]: {e.Value}");
                }
                else
                {
                    Console.WriteLine($"Warning: {e.Value}");
                }
            };

            commands.Err += (sender, e) =>
            {
                if (sender is ICommand command)
                {
                    Console.Error.WriteLine($"Err [{command.CommandName}]: {e.Value.Unroll()}");
                }
                else
                {
                    Console.Error.WriteLine($"Err: {e.Value.Unroll()}");
                }
            };

            buildTree(commands);

            var start = DateTime.Now;
            await commands.ExecuteAsync();
            var end = DateTime.Now;
            var delta = end - start;

            Console.WriteLine($"Done in {delta.TotalSeconds:0.00}s");
        }

        private IEnumerable<T> FSInfo<T>(Func<DirectoryInfo, T> selector)
            where T : FileSystemInfo
        {
            return juniperTsDir
                .EnumerateDirectories()
                .Append(projectDir)
                .Select(selector)
                .Where(v => v.Exists);
        }

        public void DeleteNodeModules()
        {
            foreach (var dir in FSInfo(dir => dir.CD("node_modules")))
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
            foreach (var lockFile in FSInfo(dir => dir.Touch("package-lock.json")))
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
            IEnumerable<DirectoryInfo> projects = buildLevel == Level.High
                ? juniperInstallables
                : juniperBuildables;

            if (projectDir.Touch("package.json").Exists)
            {
                projects = projects.Append(projectDir);
            }

            return projects.Select(dir =>
                new NPMInstallCommand(dir, buildLevel == Level.High));
        }

        public async Task InstallAsync()
        {
            await WithCommandTree(commands =>
                commands.AddCommands(GetInstallCommands(Level.High)));
        }

        private IEnumerable<TSBuildCommand> GetTSBuildCommands()
        {
            return FSInfo(dir => dir.Touch("tsconfig.json"))
                .Select(f => new TSBuildCommand(f.Directory));
        }

        public async Task TSBuildAsync()
        {
            await WithCommandTree(commands =>
                commands.AddCommands(GetTSBuildCommands()));
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

        public event EventHandler<StringEventArgs>? Info;
        public event EventHandler<StringEventArgs>? Warning;
        public event EventHandler<ErrorEventArgs>? Err;

        private void OnInfo(string message) => Info?.Invoke(this, new StringEventArgs(message));
        private void Proxy_Info(object? sender, StringEventArgs e) => OnInfo(e.Value);
        private void OnWarning(string message) => Warning?.Invoke(this, new StringEventArgs(message));
        private void Proxy_Warning(object? sender, StringEventArgs e) => OnWarning(e.Value);
        private void OnError(Exception exp) => Err?.Invoke(this, new ErrorEventArgs(exp));
        private void Proxy_Err(object? sender, ErrorEventArgs e) => OnError(e.Value);

        public Task Watch(out ICommand[] watchCommands)
        {
            var proxy = new CommandProxier(projectDir);
            proxy.Info += Proxy_Info;
            proxy.Warning += Proxy_Warning;
            proxy.Err += Proxy_Err;

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
                   cmd.ContinueAfter(watchAllDonePattern)))
                .ContinueWith((task) =>
                {
                    proxy.Info -= Proxy_Info;
                    proxy.Warning -= Proxy_Warning;
                    proxy.Err -= Proxy_Err;
                });
        }
    }
}
