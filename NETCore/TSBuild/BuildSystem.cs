using Juniper.Logging;
using Juniper.Processes;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Text.RegularExpressions;

namespace Juniper.TSBuild;

public class BuildSystem<BuildConfigT> : ILoggingSource
    where BuildConfigT : IBuildConfig, new()
{
    public static async Task Run(string[] args)
    {
        var canceller = new CancellationTokenSource();
        AppDomain.CurrentDomain.ProcessExit += (sender, e) => canceller.Cancel();

        var opts = new BuildRunOptions(args);

        var services = new ServiceCollection();
        services.AddLogging(options => options.AddConsole());
        var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<ILogger<BuildSystem<BuildConfigT>>>();
        var build = new BuildSystem<BuildConfigT>(logger, opts.workingDir);

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
                logger.LogError("{Message}\r\n{Trace}", exp.Message, exp.Unroll());
            }
        } while (!opts.Finished);
    }

    private readonly DirectoryInfo[] cleanDirs;
    private readonly DirectoryInfo workingDir;
    private readonly FileInfo projectPackage;
    private readonly FileInfo projectAppSettings;

    private readonly List<FileInfo> NPMProjects = new();
    private readonly List<FileInfo> InstallProjects = new();
    private readonly List<FileInfo> BuildProjects = new();
    private readonly List<FileInfo> WatchProjects = new();
    private readonly List<FileInfo> CheckProjects = new();
    private readonly List<CopyCommand> copyCommands = new();

    private readonly bool performPreBuild;
    private readonly bool isInProjectProcess;
    private readonly bool hasNPM;

    private readonly TaskCompletionSource starting = new();
    public Task Started => starting.Task;
    public event EventHandler? NewBuildCompleted;

    private static DirectoryInfo TestDir(string message, DirectoryInfo? dir)
    {
        if (dir?.Exists != true)
        {
            throw new BuildSystemProjectRootNotFoundException(message);
        }

        return dir;
    }

    public BuildSystem(ILogger logger, DirectoryInfo? startDir = null)
    {
        Info += (sender, e) =>
            logger.LogInformation(e.Value.RemoveClearScreen());
        Warning += (sender, e) =>
            logger.LogWarning(e.Value.RemoveClearScreen());
        Err += (sender, e) =>
            logger.LogError(e.Value.Unroll().RemoveClearScreen());


        isInProjectProcess = startDir is null;
        startDir ??= new DirectoryInfo(Environment.CurrentDirectory);

        workingDir = TestDir($"Couldn't find project root from {startDir.FullName}", startDir);

        var options = new BuildConfigT().Options;

        var inProjectDir = TestDir("You must specify at least one of InProject or OutProject in your BuildConfig.", options.Project ?? options.InProject ?? options.OutProject);

        var outProjectDir = TestDir("You must specify at least one of InProject or OutProject in your BuildConfig.", options.Project ?? options.OutProject ?? options.InProject);

        performPreBuild = options.PerformPreBuild == true;

        cleanDirs = options.CleanDirs
            ?.Where(dir => dir?.Exists ?? false)
            ?.ToArray()
            ?? Array.Empty<DirectoryInfo>();

        projectPackage = inProjectDir.Touch("package.json");
        projectAppSettings = outProjectDir.Touch("appsettings.json");

        hasNPM = ShellCommand.IsAvailable("npm");

        if (hasNPM)
        {
            var dirs = new List<DirectoryInfo>{
                    inProjectDir
                };

            if (options.AdditionalNPMProjects is not null)
            {
                dirs.AddRange(options.AdditionalNPMProjects);
            }

            Task.WaitAll(dirs.Select(CheckNPMProjectAsync).ToArray());

            AddDependencies(options.Dependencies, true);
            AddDependencies(options.OptionalDependencies, false);

            BannedDependencies = options.BannedDependencies;
        }
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
            try
            {
                var package = await NPMPackage.ReadAsync(pkgFile)
                    ?? throw new FileNotFoundException("Couldn't read package.json", pkgFile.FullName);

                var isWorkSpaceSubProject = workspaceProjects.Contains(pkgFile.FullName);

                if (!isWorkSpaceSubProject)
                {
                    NPMProjects.Add(pkgFile);

                    if (package.dependencies is not null || package.devDependencies is not null)
                    {
                        InstallProjects.Add(pkgFile);
                    }
                }

                if (package.scripts is not null)
                {
                    if (isInProjectProcess && package.scripts.ContainsKey("juniper-build")
                        || !isInProjectProcess && !isWorkSpaceSubProject && package.scripts.ContainsKey("build"))
                    {
                        BuildProjects.Add(pkgFile);
                    }

                    if (isInProjectProcess && package.scripts.ContainsKey("juniper-watch")
                        || !isInProjectProcess && !isWorkSpaceSubProject && package.scripts.ContainsKey("watch"))
                    {
                        WatchProjects.Add(pkgFile);
                    }

                    if (!isWorkSpaceSubProject && package.scripts.ContainsKey("check"))
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
            catch (Exception exp)
            {
                throw new Exception($"CheckNPMProjectAsync: Could not process {pkgFile.FullName}", exp);
            }
        }
    }

    private void AddDependencies(IEnumerable<BuildSystemDependency>? deps, bool warnIfNotExists)
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
        if (from.Name.EndsWith(".js.map"))
        {
            copyCommands.Add(new CopyCommand(name, from, to, warnIfNotExists, (from.Name, to.Name)));
        }
        else
        {
            copyCommands.Add(new CopyCommand(name, from, to, warnIfNotExists));
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
        for (var attempts = 2; attempts > 0 && !cancellationToken.IsCancellationRequested; attempts--)
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
        for (var attempts = 2; attempts > 0 && !cancellationToken.IsCancellationRequested; attempts--)
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
        d.Name != "node_modules"
            && d.Name.ToLower() != "bin"
            && d.Name.ToLower() != "obj";

    private static IEnumerable<DirectoryInfo> FindDirectories(string name, params FileInfo[] files) =>
        FindDirectories(name, files.Select(file => file.Directory!).ToArray());

    private static IEnumerable<DirectoryInfo> FindDirectories(string name, params DirectoryInfo[] dirs)
    {
        var q = new Queue<DirectoryInfo>();
        q.AddRange(dirs);
        while (q.Count > 0)
        {
            var here = q.Dequeue();
            if (IsNotBinDir(here))
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


    private Task BuildAsync(CancellationToken cancellationToken) =>
        WithCommandTree(GetBuildCommands, cancellationToken);

    private void GetBuildCommands(CommandTree commands)
    {
        commands
            .AddMessage("Starting build")
            .AddCommands(GetInstallCommands())
            .AddCommands(GetCleanCommands())
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
            starting.SetResult();
            var buildCanceller = new CancellationTokenSource();
            cancellationToken.Register(buildCanceller.Cancel);
            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
                buildCanceller.Cancel();

            var proxy = new CommandProxier(workingDir);
            await proxy.Start(buildCanceller.Token);

            buildCanceller.Token.Register(() =>
            {
                OnInfo("Stopping build");
                proxy.Stop().Wait();
                OnInfo("Build stopped");
            });

            var timer = new Timer(new TimerCallback((_) =>
            {
                lock (copyCommands)
                {
                    foreach (var dep in copyCommands)
                    {
                        if (dep.Recheck())
                        {
                            OnInfo($"{dep.CommandName} copied".Colorize("watch", 36));
                        };
                    }
                }
            }), null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3));
            buildCanceller.Token.Register(timer.Dispose);

            var installCommands = GetInstallCommands();
            await WithCommandTree(commands =>
            {
                commands
                    .AddMessage("Installing NPM Packages")
                    .AddCommands(GetCleanCommands())
                    .AddCommands(installCommands);
            }, buildCanceller.Token);

            var watchProjectPaths = WatchProjects.Select(pkg => pkg.FullName).ToHashSet();

            var bundles = TryMake(
                WatchProjects,
                file => MakeProxiedWatchCommand(proxy, file)
            ).ToArray();

            if (performPreBuild)
            {
                var buildOnlyProjects = BuildProjects.Where(pkg => !watchProjectPaths.Contains(pkg.FullName));
                var preBuilds = TryMake(
                    buildOnlyProjects,
                    file => MakeProxiedBuildCommand(proxy, file)
                ).ToArray();

                await WithCommandTree(commands =>
                {
                    commands
                        .AddMessage("Starting build")
                        .AddCommands(preBuilds)
                        .AddCommands(copyCommands);
                }, buildCanceller.Token);
            }
            else
            {
                await WithCommandTree(commands =>
                {
                    commands
                        .AddMessage("Starting build")
                        .AddCommands(copyCommands);
                }, buildCanceller.Token);
            }

            await ValidateDependencies();

            await RunWatchAsync(continueAfterFirstBuild, proxy, bundles, buildCanceller.Token);
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

    private Task RunWatchAsync(bool continueAfterFirstBuild, CommandProxier proxy, AbstractCommand[] bundles, CancellationToken buildCancelled)
    {
        if (bundles.Length == 0)
        {
            // nothing to do;
            return Task.CompletedTask;
        }

        var completeBuildTask = WithCommandTree(commands =>
        {
            commands
                .AddMessage("Starting watch")
                .AddCommands(bundles);
        }, buildCancelled);

        if (!continueAfterFirstBuild)
        {
            return completeBuildTask;
        }

        var firstBuild = new TaskCompletionSource();
        buildCancelled.Register(() =>
        {
            firstBuild.TrySetCanceled();
        });

        var buildCount = 0;

        void checkBuilt(object? sender, StringEventArgs e)
        {
            if (e.Value.Contains("build finished, waiting for changes..."))
            {
                if (buildCount == 0)
                {
                    firstBuild.TrySetResult();
                }
                else
                {
                    NewBuildCompleted?.Invoke(this, EventArgs.Empty);
                }
                ++buildCount;
                if (buildCount == 0)
                {
                    proxy.Info -= checkBuilt;
                }
            }
        }

        proxy.Info += checkBuilt;

        buildCancelled.Register(() =>
        {
            proxy.Info -= checkBuilt;
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

    private AbstractCommand MakeProxiedBuildCommand(CommandProxier proxy, FileInfo pkg) =>
        WithCopyDiscovery(new ProxiedCommand(proxy, pkg.Directory!, "npm", "run", "juniper-build"));

    private AbstractCommand MakeProxiedWatchCommand(CommandProxier proxy, FileInfo pkg) =>
        WithCopyDiscovery(new ProxiedCommand(proxy, pkg.Directory!, "npm", "run", "juniper-watch"));

    private static readonly Regex COPY_PATTERN = new("COPY (.+) -> (.+)");
    private ProxiedCommand WithCopyDiscovery(ProxiedCommand cmd)
    {
        cmd.Info += delegate (object? sender, StringEventArgs e)
        {
            var match = COPY_PATTERN.Match(e.Value);
            if (match?.Success ?? false)
            {
                var inFile = cmd.WorkingDir.Touch(match.Groups[1].Value);
                var outFile = cmd.WorkingDir.Touch(match.Groups[2].Value);
                lock (copyCommands)
                {
                    copyCommands.Add(new CopyCommand(inFile.Name, inFile, outFile, true));
                }
            }
        };
        return cmd;
    }

    public event EventHandler<StringEventArgs>? Info;
    public event EventHandler<StringEventArgs>? Warning;
    public event EventHandler<ErrorEventArgs>? Err;

    private void OnInfo(string message) => Info?.Invoke(this, new StringEventArgs(message));
    private void OnWarning(string message) => Warning?.Invoke(this, new StringEventArgs(message));
    private void OnError(Exception exp) => Err?.Invoke(this, new ErrorEventArgs(exp));
}
