using Juniper.Logging;
using Juniper.Processes;

namespace Juniper.TSBuild;

public class BuildSystem<BuildConfigT> : ILoggingSource
    where BuildConfigT : IBuildConfig, new()
{
    delegate void Writer(string format, params object[] args);

    static string Colorize(string tag, int color, string format, params object[] args)
    {
        if (args.Length > 0)
        {
            format = string.Format(format, args);
        }

        return $"\u001b[{color}m{tag}:\u001b[0m {format}";
    }

    static void WriteError(string format, params object[] values) =>
        Console.Error.WriteLine(Colorize("error", 31, format, values));

    static void WriteInfo(string format, params object[] values) =>
        Console.WriteLine(Colorize("info", 32, format, values));

    static void WriteWarning(string format, params object[] values) =>
        Console.WriteLine(Colorize("warn", 33, format, values));

    public static async Task Run(string[] args)
    {
        var canceller = new CancellationTokenSource();
        AppDomain.CurrentDomain.ProcessExit += (sender, e) => canceller.Cancel();

        var opts = new BuildOptions(args);

        var build = new BuildSystem<BuildConfigT>(opts.workingDir);

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
                    build.DeleteNodeModuleDirs(canceller.Token);
                }
                else if (opts.DeletePackageLockJsons)
                {
                    build.DeletePackageLockJsons(canceller.Token);
                }
                else if (opts.DeleteTSBuildInfos)
                {
                    build.DeleteTSBuildInfos(canceller.Token);
                }
                else if (opts.NPMInstalls)
                {
                    await build.NPMInstallsAsync(canceller.Token);
                }
                else if (opts.OpenPackageJsons)
                {
                    await build.OpenPackageJsonsAsync(canceller.Token);
                }
                else if (opts.OpenTSConfigJsons)
                {
                    await build.OpenTSConfigJsonsAsync(canceller.Token);
                }
                else if (opts.TypeCheck)
                {
                    await build.TypeCheckAsync(canceller.Token);
                }
                else if (opts.Watch)
                {
                    await build.WatchAsync(false, canceller.Token);
                }
                else if (!opts.Finished || opts.Build)
                {
                    await build.BuildAsync(canceller.Token);
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

    private readonly DirectoryInfo[] cleanDirs;
    private readonly DirectoryInfo inProjectDir;
    private readonly DirectoryInfo outProjectDir;
    private readonly FileInfo projectPackage;
    private readonly FileInfo projectAppSettings;

    private readonly Dictionary<FileInfo, (string, FileInfo, bool)> dependencies = new();
    private readonly Dictionary<string, (string, string)> mapFileReplacements = new();

    private readonly List<FileInfo> NPMProjects = new();
    private readonly List<FileInfo> InstallProjects = new();
    private readonly List<FileInfo> BuildProjects = new();
    private readonly List<FileInfo> WatchProjects = new();
    private readonly List<FileInfo> CheckProjects = new();

    private readonly bool isInProjectProcess;
    private readonly bool hasNPM;

    private static DirectoryInfo TestDir(string message, DirectoryInfo? dir)
    {
        if (dir?.Exists != true)
        {
            throw new BuildSystemProjectRootNotFoundException(message);
        }

        return dir;
    }

    public BuildSystem(DirectoryInfo? workingDir = null)
    {
        isInProjectProcess = workingDir is null;
        var options = new BuildConfigT().Options;
        workingDir ??= new DirectoryInfo(Environment.CurrentDirectory);

        var startDir = TestDir($"Couldn't find project root from {workingDir.FullName}", workingDir);

        inProjectDir = TestDir("You must specify at least one of InProject or OutProject in your BuildConfig.", options.InProject ?? options.OutProject);

        outProjectDir = TestDir("You must specify at least one of InProject or OutProject in your BuildConfig.", options.OutProject ?? options.InProject);

        var juniperDir = FindJuniperDir(startDir);
        var juniperTsDir = TestDir("Couldn't find Juniper TypeScript", juniperDir.CD("src", "Juniper.TypeScript"));

        cleanDirs = options.CleanDirs
            ?.Where(dir => dir?.Exists == true)
            ?.ToArray()
            ?? Array.Empty<DirectoryInfo>();

        projectPackage = inProjectDir.Touch("package.json");
        projectAppSettings = outProjectDir.Touch("appsettings.json");

        hasNPM = ShellCommand.IsAvailable("npm");

        if (hasNPM)
        {
            if (!options.SkipNPMInstall)
            {
                var dirs = new[]{
                    juniperTsDir,
                    inProjectDir
                };

                if (options.AdditionalNPMProjects is not null)
                {
                    dirs = dirs.Union(options.AdditionalNPMProjects).ToArray();
                }

                Task.WaitAll(dirs.Select(CheckNPMProjectAsync).ToArray());
            }

            if (options.Dependencies is not null)
            {
                AddDependencies(options.Dependencies, true);
            }

            if (options.OptionalDependencies is not null)
            {
                AddDependencies(options.OptionalDependencies, false);
            }

            BannedDependencies = options.BannedDependencies;
        }

        Info += (sender, e) => WriteInfo(e.Value);
        Warning += (sender, e) => WriteWarning(e.Value);
        Err += (sender, e) => WriteError(e.Value.Unroll());
    }

    private async Task CheckNPMProjectAsync(DirectoryInfo project)
    {
        var workspaceProjects = new HashSet<string>();

        var packages = project
            .LazyRecurse(IsNotBinDir)
            .Select(dir => dir.Touch("package.json"))
            .Where(file => file.Exists)
            .ToArray();

        foreach (var pkgFile in packages)
        {
            if (!workspaceProjects.Contains(pkgFile.FullName))
            {
                var package = await NPMPackage.ReadAsync(pkgFile)
                    ?? throw new FileNotFoundException("Couldn't read package.json", pkgFile.FullName);

                NPMProjects.Add(pkgFile);

                if (package.dependencies is not null || package.devDependencies is not null)
                {
                    InstallProjects.Add(pkgFile);
                }

                if (package.scripts is not null)
                {
                    if (isInProjectProcess && package.scripts.ContainsKey("juniper-build")
                        || !isInProjectProcess && package.scripts.ContainsKey("build"))
                    {
                        BuildProjects.Add(pkgFile);
                    }

                    if (isInProjectProcess && package.scripts.ContainsKey("juniper-watch")
                        || !isInProjectProcess && package.scripts.ContainsKey("watch"))
                    {
                        WatchProjects.Add(pkgFile);
                    }

                    if (package.scripts.ContainsKey("check"))
                    {
                        CheckProjects.Add(pkgFile);
                    }
                }

                if (package.workspaces is not null)
                {
                    var here = pkgFile.Directory!;
                    foreach (var pkg in package.workspaces
                        .Select(dirName => here.CD(dirName).Touch("package.json")))
                    {
                        if (pkg.Exists)
                        {
                            workspaceProjects.Add(pkg.FullName);
                        }
                        else
                        {
                            Console.Error.WriteLine($"Package file {pkgFile.FullName} specifies a workspace project that doesn't exist: {pkg.FullName}!");
                        }
                    }
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

    private void AddDependencies(IEnumerable<BuildSystemDependency> deps, bool warnIfNotExists)
    {
        if (deps is not null)
        {
            foreach (var (Name, From, To) in deps)
            {
                AddDependency(Name, From, To, warnIfNotExists);
            }
        }
    }

    private BuildSystem<BuildConfigT> AddDependency(string name, FileInfo from, FileInfo to, bool warnIfNotExists)
    {
        dependencies.Add(from, (name, to, warnIfNotExists));
        var scriptFile = from.FullName;
        if (from.Name.EndsWith(".js.map"))
        {
            scriptFile = scriptFile[..^4];
            mapFileReplacements.Add(scriptFile, (from.Name, to.Name));
        }

        return this;
    }

    private (string Name, string Version, string Reason)[]? BannedDependencies { get; }

    private async Task WithCommandTree(Action<CommandTree> buildTree, CancellationToken cancellationToken)
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
        await commands.ExecuteAsync(cancellationToken);
        var end = DateTime.Now;
        var delta = end - start;

        OnInfo($"Build finished in {delta.TotalSeconds:0.00}s");
    }

    private void DeleteDir(DirectoryInfo dir, CancellationToken cancellationToken)
    {
        for (int attempts = 2; attempts > 0 && !cancellationToken.IsCancellationRequested; attempts--)
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

    private void DeleteFile(FileInfo lockFile, CancellationToken cancellationToken)
    {
        for (int attempts = 2; attempts > 0 && !cancellationToken.IsCancellationRequested; attempts--)
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

    private void DeleteFiles(IEnumerable<FileInfo> files, CancellationToken cancellationToken)
    {
        foreach (var file in files.Where(f => f.Exists))
        {
            DeleteFile(file, cancellationToken);
        }
    }

    private void DeleteDirectories(IEnumerable<DirectoryInfo> dirs, CancellationToken cancellationToken)
    {
        foreach (var dir in dirs.Where(d => d.Exists))
        {
            DeleteDir(dir, cancellationToken);
        }
    }

    private void DeleteNodeModuleDirs(CancellationToken cancellationToken) =>
        DeleteDirectories(FindAllNodeModulesDirs(), cancellationToken);

    private IEnumerable<DirectoryInfo> FindAllNodeModulesDirs() =>
        FindDirectories("node_modules", InstallProjects.ToArray());

    private void DeletePackageLockJsons(CancellationToken cancellationToken) =>
        DeleteFiles(FindFiles("package-lock.json"), cancellationToken);

    private void DeleteTSBuildInfos(CancellationToken cancellationToken) =>
        DeleteFiles(FindFiles("tsconfig.tsbuildinfo"), cancellationToken);

    private IEnumerable<FileInfo> FindFiles(string name) =>
        FindFiles(name, NPMProjects.ToArray());

    private static IEnumerable<FileInfo> FindFiles(string name, params FileInfo[] files) =>
        FindFiles(name, files.Select(file => file.Directory!).ToArray());

    private static IEnumerable<FileInfo> FindFiles(string name, params DirectoryInfo[] dirs)
    {
        foreach (var dir in dirs)
        {
            foreach (var subDir in dir.LazyRecurse(IsNotBinDir))
            {
                var file = subDir.Touch(name);
                if (file.Exists)
                {
                    yield return file;
                }
            }
        }
    }

    private static bool IsNotBinDir(DirectoryInfo d) =>
        d.Name != "node_modules" && d.Name != "bin";

    private static IEnumerable<DirectoryInfo> FindDirectories(string name, params FileInfo[] files)
        => FindDirectories(name, files.Select(file => file.Directory!).ToArray());

    private static IEnumerable<DirectoryInfo> FindDirectories(string name, params DirectoryInfo[] dirs)
    {
        var q = new Queue<DirectoryInfo>();
        q.AddRange(dirs);
        while (q.Count > 0)
        {
            var here = q.Dequeue();
            if (here.Name != "node_modules")
            {
                q.AddRange(here.EnumerateDirectories());
                var dir = here.CD(name);
                if (dir.Exists)
                {
                    yield return dir;
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

    private IEnumerable<T> TryMake<V, T>(IEnumerable<V?> collection, Func<V, T> make) where T : class =>
        collection
            .Where(dir => dir is not null)
            .Select(dir => TryMake(() => make(dir!)))
            .Where(t => t is not null)
            .Cast<T>();

    private IEnumerable<DeleteDirectoryCommand> GetCleanCommands() =>
        TryMake(
            cleanDirs,
            dir => new DeleteDirectoryCommand(dir)
        );

    private IEnumerable<NPMInstallCommand> GetInstallCommands() =>
        TryMake(
            InstallProjects,
            dir => new NPMInstallCommand(dir)
        );

    private Task NPMInstallsAsync(CancellationToken cancellationToken) =>
        WithCommandTree(commands =>
            commands.AddCommands(GetInstallCommands()), cancellationToken);

    private Task OpenPackageJsonsAsync(CancellationToken cancellationToken) =>
        WithCommandTree(commands =>
            commands.AddCommands(TryMake(
                FindFiles("package.json"),
                f => new ShellCommand(f.Directory, "explorer", f.Name)
            )), cancellationToken);

    private Task OpenTSConfigJsonsAsync(CancellationToken cancellationToken) =>
        WithCommandTree(commands =>
            commands.AddCommands(TryMake(
                FindFiles("tsconfig.json"),
                f => new ShellCommand(f.Directory, "explorer", f.Name)
            )), cancellationToken);

    private Task TypeCheckAsync(CancellationToken cancellationToken) =>
        WithCommandTree(commands =>
            commands.AddCommands(TryMake(
                CheckProjects,
                file => new NPMRunCommand(file, "check")
            )), cancellationToken);

    private CopyCommand[] GetDependecies() =>
        dependencies
            .Select(kv =>
                mapFileReplacements.ContainsKey(kv.Key.FullName)
                    ? new CopyCommand(kv.Value.Item1, kv.Key, kv.Value.Item2, kv.Value.Item3, mapFileReplacements[kv.Key.FullName])
                    : new CopyCommand(kv.Value.Item1, kv.Key, kv.Value.Item2, kv.Value.Item3)
            )
            .ToArray();

    private Task BuildAsync(CancellationToken cancellationToken) =>
        WithCommandTree(GetBuildCommands, cancellationToken);

    private void GetBuildCommands(ICommandTree commands)
    {
        var copyCommands = GetDependecies();

        commands
            .AddMessage("Starting build")
            .AddCommands(GetCleanCommands())
            .AddCommands(GetInstallCommands())
            .AddCommands(copyCommands);

        if (BuildProjects.Count > 0)
        {
            commands.AddCommands(TryMake(
                BuildProjects,
                file => new NPMRunCommand(file, "build")
            ));
        }

        commands.AddCommands(
            new CopyJsonValueCommand(
                projectPackage, "version",
                projectAppSettings, "Version"));
    }



    public async Task WatchAsync(bool continueAfterFirstBuild, CancellationToken cancellationToken)
    {
        try
        {
            var buildCanceller = new CancellationTokenSource();
            cancellationToken.Register(buildCanceller.Cancel);
            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
                buildCanceller.Cancel();

            var proxy = new CommandProxier(inProjectDir);
            proxy.Info += Proxy_Info;
            proxy.Warning += Proxy_Warning;
            proxy.Err += Proxy_Err;
            await proxy.Start(buildCanceller.Token);

            buildCanceller.Token.Register(() =>
            {
                OnInfo("Stopping build");
                proxy.Stop().Wait();
                OnInfo("Build stopped");
            });

            var copyCommands = GetDependecies();
            var timer = new Timer(new TimerCallback((_) =>
            {
                foreach (var dep in copyCommands)
                {
                    if (dep.Recheck())
                    {
                        OnInfo(Colorize("watch", 36, "{0} copied", dep.CommandName));
                    };
                }
            }), null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3));
            buildCanceller.Token.Register(timer.Dispose);

            var watchProjectPaths = WatchProjects.Select(pkg => pkg.FullName).ToHashSet();
            var buildOnlyProjects = BuildProjects.Where(pkg => !watchProjectPaths.Contains(pkg.FullName));

            var preBuilds = TryMake(
                buildOnlyProjects,
                file => MakeProxiedBuildCommand(proxy, file)
            ).ToArray();

            var bundles = TryMake(
                WatchProjects,
                file => MakeProxiedWatchCommand(proxy, file)
            ).ToArray();

            await WithCommandTree(commands =>
            {
                commands
                    .AddMessage("Starting watch")
                    .AddCommands(GetCleanCommands())
                    .AddCommands(GetInstallCommands())
                    .AddCommands(preBuilds)
                    .AddCommands(copyCommands);
            }, buildCanceller.Token);

            await ValidateDependencies();
            await RunWatchAsync(continueAfterFirstBuild, bundles, buildCanceller.Token);
            if (!continueAfterFirstBuild && !buildCanceller.IsCancellationRequested)
            {
                await timer.DisposeAsync();
            }
        }
        catch (TaskCanceledException)
        {
            //do nothing
        }
    }

    private Task RunWatchAsync(bool continueAfterFirstBuild, AbstractShellCommand[] bundles, CancellationToken buildCancelled)
    {
        if (bundles.Length == 0)
        {
            // nothing to do;
            return Task.CompletedTask;
        }

        var completeBuildTask = Task.WhenAll(bundles.Select(b =>
            b.RunSafeAsync(buildCancelled)));

        if (!continueAfterFirstBuild)
        {
            return completeBuildTask;
        }

        var firstBuild = new TaskCompletionSource();
        buildCancelled.Register(() =>
        {
            firstBuild.TrySetCanceled();
        });

        EventHandler<StringEventArgs>? onInfo = null;
        onInfo = new EventHandler<StringEventArgs>((object? sender, StringEventArgs e) =>
        {
            if (e.Value.Contains("browser bundles built"))
            {
                Info -= onInfo;
                firstBuild.TrySetResult();
            }
        });

        Info += onInfo;
        buildCancelled.Register(() =>
        {
            Info -= onInfo;
        });

        return Task.WhenAny(completeBuildTask, firstBuild.Task);
    }


    private async Task ValidateDependencies()
    {
        if (BannedDependencies is not null)
        {
            foreach (var dir in FindAllNodeModulesDirs())
            {
                foreach (var (pkgName, version, reason) in BannedDependencies)
                {
                    var pkgFile = dir.CD(pkgName).Touch("package.json");
                    if (pkgFile.Exists)
                    {
                        var package = await NPMPackage.ReadAsync(pkgFile);
                        if (package?.version == version)
                        {
                            OnWarning($"Banned package found: {pkgName}: {version} -> {reason}");
                        }
                    }
                }
            }
        }
    }

    private AbstractShellCommand MakeProxiedBuildCommand(CommandProxier proxy, FileInfo pkg)
    {
        var cmd = new ProxiedCommand(proxy, pkg.Directory!, "npm", "run", "juniper-build");
        cmd.Info += Proxy_Info;
        cmd.Err += Proxy_Err;
        cmd.Warning += Proxy_Warning;
        return cmd;
    }

    private AbstractShellCommand MakeProxiedWatchCommand(CommandProxier proxy, FileInfo pkg)
    {
        var cmd = new ProxiedCommand(proxy, pkg.Directory!, "npm", "run", "juniper-watch");
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
}
