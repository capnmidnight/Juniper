using Juniper.Logging;
using Juniper.Processes;

using System.Net.NetworkInformation;
using System.Text.Json;

namespace Juniper.TSBuild
{
    public class BuildSystem<BuildConfigT> : ILoggingSource, IDisposable
        where BuildConfigT : IBuildConfig, new()
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

        static void WriteError(string format, params object[] values) =>
            Console.Error.WriteLine(Colorize("error", 31, format, values));

        static void WriteInfo(string format, params object[] values) =>
            Console.WriteLine(Colorize("info", 32, format, values));

        static void WriteWarning(string format, params object[] values) =>
            Console.WriteLine(Colorize("warn", 33, format, values));

        public static async Task Run(string[] args)
        {
            var opts = new Options(args);

            using var build = new BuildSystem<BuildConfigT>(opts.workingDir);

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
                    else if (opts.OpenPackageJsons)
                    {
                        await build.OpenPackageJsonsAsync();
                    }
                    else if (opts.OpenTSConfigJsons)
                    {
                        await build.OpenTSConfigJsonsAsync();
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

        private readonly DirectoryInfo[] cleanDirs;
        private readonly DirectoryInfo inProjectDir;
        private readonly DirectoryInfo outProjectDir;
        private readonly DirectoryInfo juniperTsDir;
        private readonly FileInfo projectPackage;
        private readonly FileInfo projectAppSettings;
        private readonly DeploymentOptions? deployment;

        private readonly Dictionary<FileInfo, (string, FileInfo, bool)> dependencies = new();
        private readonly Dictionary<string, (string, string)> mapFileReplacements = new();
        private readonly List<DirectoryInfo> TSProjects = new();
        private readonly List<DirectoryInfo> ESBuildProjects = new();
        private readonly List<DirectoryInfo> NPMProjects = new();

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

        public BuildSystem(DirectoryInfo? workingDir = null)
        {
            var options = new BuildConfigT().Options;
            workingDir ??= new DirectoryInfo(Environment.CurrentDirectory);
            var startDir = workingDir;

            var inProjectName = options.InProjectName ?? options.OutProjectName;
            var outProjectName = options.OutProjectName ?? options.InProjectName;

            while (startDir != null
                && !startDir.CD(inProjectName).Exists)
            {
                startDir = startDir.Parent;
            }

            startDir = TestDir($"Couldn't find project root from {workingDir.FullName}", startDir);

            var juniperDir = FindJuniperDir(startDir);

            juniperTsDir = TestDir("Couldn't find Juniper TypeScript", juniperDir.CD("src", "Juniper.TypeScript"));
            inProjectDir = TestDir($"Couldn't find project {inProjectName} from {startDir}", startDir.CD(inProjectName));
            outProjectDir = TestDir($"Couldn't find project {outProjectName} from {startDir}", startDir.CD(outProjectName));
            cleanDirs = options.CleanDirs
                ?.Where(dir => dir?.Exists == true)
                ?.ToArray()
                ?? Array.Empty<DirectoryInfo>();

            projectPackage = inProjectDir.Touch("package.json");
            projectAppSettings = outProjectDir.Touch("appsettings.json");

            if (options.Deployment is not null)
            {
                deployment = new DeploymentOptions(
                    options.Deployment.HostName,
                    options.Deployment.UserName,
                    options.Deployment.KeyFile,
                    options.Deployment.RemoteDirName,
                    options.Deployment.RemoteServiceName ?? options.Deployment.RemoteDirName);
            }

            hasNPM = ShellCommand.IsAvailable("npm");

            if (hasNPM)
            {
                CheckNPMProject(inProjectDir);
                CheckTSProject(inProjectDir);
                CheckESBuildProject(inProjectDir);

                CheckNPMProject(juniperTsDir);
                CheckTSProject(juniperTsDir);

                AddDependencies(options.Dependencies, true);
                AddDependencies(options.OptionalDependencies, false);
                BannedDependencies = options.BannedDependencies;
            }

            Info += (sender, e) => WriteInfo(e.Value);
            Warning += (sender, e) => WriteWarning(e.Value);
            Err += (sender, e) => WriteError(e.Value.Unroll());
        }

        private void CheckNPMProject(DirectoryInfo project)
        {
            var pkgFile = project.Touch("package.json");
            if (pkgFile.Exists)
            {
                NPMProjects.Add(project);
            }
        }

        private void CheckTSProject(DirectoryInfo project)
        {
            var tsConfigFile = project.Touch("tsconfig.json");
            if (tsConfigFile.Exists)
            {
                TSProjects.Add(project);
            }
        }

        private void CheckESBuildProject(DirectoryInfo project)
        {
            var esbuildFile = project.Touch("esbuild.config.js");
            if (esbuildFile.Exists)
            {
                ESBuildProjects.Add(project);
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

        private void AddDependencies(Dictionary<string, (FileInfo From, FileInfo To)>? deps, bool warnIfNotExists)
        {
            if (deps is not null)
            {
                foreach (var d in deps)
                {
                    AddDependency(d.Key, d.Value.From, d.Value.To, warnIfNotExists);
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

        private void DeleteDirectories(IEnumerable<DirectoryInfo> dirs)
        {
            foreach (var dir in dirs.Where(d => d.Exists))
            {
                DeleteDir(dir);
            }
        }

        private void DeleteNodeModuleDirs() =>
            DeleteDirectories(FindAllNodeModulesDirs());

        private IEnumerable<DirectoryInfo> FindAllNodeModulesDirs() =>
            FindDirectories("node_modules", inProjectDir, outProjectDir, juniperTsDir);

        private void DeletePackageLockJsons() =>
            DeleteFiles(FindFiles("package-lock.json"));

        private void DeleteTSBuildInfos() =>
            DeleteFiles(FindFiles("tsconfig.tsbuildinfo"));

        private IEnumerable<FileInfo> FindFiles(string name) =>
            FindFiles(name, inProjectDir, outProjectDir, juniperTsDir);

        private static IEnumerable<FileInfo> FindFiles(string name, params DirectoryInfo[] dirs)
        {
            var q = new Queue<DirectoryInfo>();
            q.AddRange(dirs);
            while (q.Count > 0)
            {
                var here = q.Dequeue();
                if (here.Name != "node_modules")
                {
                    q.AddRange(here.EnumerateDirectories());
                    var file = here.Touch(name);
                    if (file.Exists)
                    {
                        yield return file;
                    }
                }
            }
        }

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

        private IEnumerable<T> TryMake<V, T>(IEnumerable<V> collection, Func<V, T> make) where T : class =>
            collection
                .Select(dir => TryMake(() => make(dir)))
                .Where(t => t is not null)
                .Cast<T>();

        private IEnumerable<DeleteDirectoryCommand> GetCleanCommands() =>
            TryMake(
                cleanDirs,
                dir => new DeleteDirectoryCommand(dir)
            );

        private IEnumerable<NPMInstallCommand> GetInstallCommands() =>
            TryMake(
                NPMProjects,
                dir => new NPMInstallCommand(dir, true)
            );

        private Task NPMInstallsAsync() =>
            WithCommandTree(commands =>
                commands.AddCommands(GetInstallCommands()));

        private Task OpenPackageJsonsAsync() =>
            WithCommandTree(commands =>
                commands.AddCommands(TryMake(
                    FindFiles("package.json"),
                    f => new ShellCommand(f.Directory, "explorer", f.Name)
                )));

        private Task OpenTSConfigJsonsAsync() =>
            WithCommandTree(commands =>
                commands.AddCommands(TryMake(
                    FindFiles("tsconfig.json"),
                    f => new ShellCommand(f.Directory, "explorer", f.Name)
                )));

        private Task TypeCheckAsync() =>
            WithCommandTree(commands =>
                commands.AddCommands(TryMake(
                    NPMProjects,
                    dir =>
                        dir == inProjectDir
                            ? new ShellCommand(dir, "npm", "run", "check")
                            : new ShellCommand(dir, "npm", "run", "check", "--workspaces", "--if-present")
                )));

        private DirectoryInfo[] GetProjectESBuildDirectories() =>
            ESBuildProjects.Where(dir => dir == inProjectDir).ToArray();

        private CopyCommand[] GetDependecies() =>
            dependencies
                .Select(kv =>
                    mapFileReplacements.ContainsKey(kv.Key.FullName)
                        ? new CopyCommand(kv.Value.Item1, kv.Key, kv.Value.Item2, kv.Value.Item3, mapFileReplacements[kv.Key.FullName])
                        : new CopyCommand(kv.Value.Item1, kv.Key, kv.Value.Item2, kv.Value.Item3)
                )
                .ToArray();

        private Task BuildAsync() =>
            WithCommandTree(GetBuildCommands);

        private void GetBuildCommands(ICommandTree commands)
        {
            var copyCommands = GetDependecies();
            var projES = GetProjectESBuildDirectories();

            commands
                .AddMessage("Starting build")
                .AddCommands(GetCleanCommands())
                .AddCommands(GetInstallCommands())
                .AddCommands(copyCommands);

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
        }

        public void Watch() =>
            WatchAsync().Wait();

        public async Task WatchAsync()
        {
            var proxy = new CommandProxier(inProjectDir);
            proxy.Info += Proxy_Info;
            proxy.Warning += Proxy_Warning;
            proxy.Err += Proxy_Err;
            await proxy.Start();

            var copyCommands = GetDependecies();
            var bundles = TryMake(
                ESBuildProjects,
                dir => MakeWatchCommand(proxy, dir)
            ).ToArray();

            await WithCommandTree(commands =>
            {
                commands
                    .AddMessage("Starting watch")
                    .AddCommands(GetCleanCommands())
                    .AddCommands(GetInstallCommands())
                    .AddCommands(copyCommands);
            });

            await ValidateDependencies();

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

            foreach (var line in from i in NetworkInterface.GetAllNetworkInterfaces()
                                 where i.OperationalStatus == OperationalStatus.Up
                                    && i.NetworkInterfaceType != NetworkInterfaceType.Loopback
                                 let p = i.GetIPProperties()
                                 from a in p.UnicastAddresses
                                 where a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                                 select $"{i.Name}: {a.Address}")
            {
                WriteInfo(line);
            }
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
                            using var packageStream = pkgFile.OpenRead();
                            var package = await JsonSerializer.DeserializeAsync<NPMPackage>(packageStream);
                            if (package?.version == version)
                            {
                                OnWarning($"Banned package found: {pkgName}: {version} -> {reason}");
                            }
                        }
                    }
                }
            }
        }

        private AbstractShellCommand MakeWatchCommand(CommandProxier proxy, DirectoryInfo dir)
        {
            var args = new List<string>
            {
                "run",
                "watch"
            };

            if (dir != inProjectDir)
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
