using System.Diagnostics;
using System.Text;

namespace Juniper.Processes;

public class ShellCommandNotFoundException : Exception
{
    public ShellCommandNotFoundException(string message)
        : base(message)
    {
    }
}

public class ShellCommand : AbstractCommand
{
    private static readonly Dictionary<PlatformID, string[]> exts = new()
    {
        { PlatformID.Unix, new[] { "", ".app" } },
        { PlatformID.Win32NT, new[] { ".exe", ".cmd" } },
        { PlatformID.Other, new[] { "" } }
    };

    public static bool IsAvailable(string command) =>
        FindCommandPath(command) is not null;

    public static string? FindCommandPath(string? command) =>
        FindCommandPaths(command)
            .Where(File.Exists)
            .FirstOrDefault();

    protected static string[] FindCommandPaths(string? command)
    {
        if (string.IsNullOrWhiteSpace(command))
        {
            return [];
        }

        command = command.Trim();

        var platform = Environment.OSVersion.Platform;
        var extensions = exts[platform];

        var PATH = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        var pathDirs = PATH.Split(Path.PathSeparator);
        var dirs = (from d in DirectoryInfoExt.LazyRecurse(Environment.CurrentDirectory)
                                            .Union(pathDirs)
                                            .Where(x => !string.IsNullOrWhiteSpace(x) && Directory.Exists(x))
                                            .Distinct()
                    from ext in extensions
                    select Path.Combine(d, command + ext))
               .ToArray();

        return dirs;
    }

    private static string MakeCommandName(DirectoryInfo? workingDir, ref string? command, ref string[] args)
    {
        var originalCommandName = command;
        command = FindCommandPath(command);

        if (command is null)
        {
            var attemptedCommands = FindCommandPaths(command)
                .ToArray()
                .Join("\n\t");
            throw new ShellCommandNotFoundException($"Could not find command: {originalCommandName}. Tried:\n\t{attemptedCommands}");
        }

        if (command.Length == 0)
        {
            throw new InvalidOperationException($"{nameof(command)} cannot be an empty string.");
        }

        args ??= [];

        var exeName = new FileInfo(command).GetShortName();

        var commandName = args
            .Prepend(exeName)
            .ToArray()
            .Join(' ');

        if (workingDir is not null)
        {
            var dirName = workingDir.Name;
            if (workingDir.Parent is not null)
            {
                dirName = Path.Combine(workingDir.Parent.Name, dirName);
            }
            commandName = $"({dirName}) {commandName}";
        }

        return commandName;
    }

    private readonly string command;
    protected readonly List<string> args = [];
    private readonly bool calledFromCurrentDirectory;
    protected readonly DirectoryInfo workingDir;

    public string Arguments => args.ToArray().Join(' ');

    internal Job? job;

    private bool running;

    public bool LoadWindowsUserProfile { get; set; }

    public bool CreateWindow { get; set; }

    public Encoding Encoding { get; set; } = Encoding.UTF8;

    public int? ExitCode { get; private set; }

    private event EventHandler<StringEventArgs>? Input;
    private event EventHandler? KillProc;

    public ShellCommand(string command, params string[] args)
        : this(null, command, args)
    {
    }

    public ShellCommand(DirectoryInfo? workingDir, string? command, params string[] args)
        : base(MakeCommandName(workingDir, ref command, ref args))
    {
        calledFromCurrentDirectory = workingDir is null;
        this.workingDir = workingDir ?? new DirectoryInfo(Directory.GetCurrentDirectory());
        this.command = command ?? throw new ArgumentNullException(nameof(command));
        this.args.AddRange(args);
    }

    public override async Task RunAsync(CancellationToken cancellationToken)
    {
        if (running)
        {
            throw new InvalidOperationException("Cannot invoke the command a second time");
        }

        if (command is null)
        {
            throw new InvalidOperationException("Cannot find command: " + CommandName);
        }

        ExitCode = null;

        var startInfo = new ProcessStartInfo(command)
        {
            WorkingDirectory = calledFromCurrentDirectory ? null : workingDir.FullName,
            Arguments = Arguments,
            StandardErrorEncoding = Encoding,
            StandardInputEncoding = Encoding,
            StandardOutputEncoding = Encoding,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            ErrorDialog = !CreateWindow,
            CreateNoWindow = !CreateWindow,
            WindowStyle = CreateWindow
                ? ProcessWindowStyle.Normal
                : ProcessWindowStyle.Hidden
        };

        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            var procInfoType = typeof(ProcessStartInfo);
            var prop = procInfoType.GetProperty("UseShellExecute");
            prop?.SetValue(startInfo, false);

            if (LoadWindowsUserProfile)
            {
                // We have to use reflection here because the property doesn't exist on other platforms.
                prop = procInfoType.GetProperty("LoadUserProfile");
                prop?.SetValue(startInfo, true);
            }
        }

        using var proc = new Process()
        {
            StartInfo = startInfo,
            EnableRaisingEvents = true
        };

        void Proc_InputDataReceived(object? sender, StringEventArgs e) =>
            proc.StandardInput.WriteLine(e.Value);

        void Proc_KillProc(object? sender, EventArgs e) => 
            proc.Kill(true);

        Input += Proc_InputDataReceived;
        KillProc += Proc_KillProc;
        AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        AppDomain.CurrentDomain.DomainUnload += CurrentDomain_ProcessExit;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_ProcessExit;
        proc.OutputDataReceived += Proc_OutputDataReceived;
        proc.ErrorDataReceived += Proc_ErrorDataReceived;

        try
        {
            if (!proc.Start())
            {
                throw new ProcessStartException("Could not start process.");
            }

            if (cancellationToken != CancellationToken.None)
            {
                cancellationToken.Register(() =>
                {
                    try
                    {
                        proc.Kill();
                    }
                    catch (NotSupportedException)
                    {
                    }
                    catch (InvalidOperationException)
                    {
                    }
                });
            }

            job?.AddProcess(proc);
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();

            await proc.WaitForExitAsync(cancellationToken);
            ExitCode = proc.ExitCode;
        }
        catch (TaskCanceledException)
        {
            // do nothing
            ExitCode = -1;
        }
        catch (Exception exp)
        {
            ExitCode = -1;
            throw new ProcessStartException("Could not start process.", exp);
        }
        finally
        {
            AppDomain.CurrentDomain.ProcessExit -= CurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.DomainUnload -= CurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_ProcessExit;
            proc.OutputDataReceived -= Proc_OutputDataReceived;
            proc.ErrorDataReceived -= Proc_ErrorDataReceived;
            Input -= Proc_InputDataReceived;
            KillProc -= Proc_KillProc;

            running = false;

            if (ExitCode != 0)
            {
                OnWarning($"Non-zero exit value = {ExitCode}. Invocation = {proc.StartInfo.FileName} {proc.StartInfo.Arguments}. Working directory = {proc.StartInfo.WorkingDirectory}");
            }
        }
    }

    public async Task<string[]> RunForStdOutAsync(CancellationToken cancellationToken)
    {
        var output = new List<string>();
        void ShellCommand_Info(object? sender, StringEventArgs e) =>
            output.Add(e.Value);
        Info += ShellCommand_Info;
        await RunAsync(cancellationToken);
        Info -= ShellCommand_Info;
        return [.. output];
    }

    private void Proc_OutputDataReceived(object? sender, DataReceivedEventArgs e)
    {
        if (e.Data is not null && e.Data.Length > 0)
        {
            OnInfo(e.Data);
        }
    }

    private void Proc_ErrorDataReceived(object? sender, DataReceivedEventArgs e)
    {
        if (e.Data is not null && e.Data.Length > 0)
        {
            OnWarning(e.Data);
        }
    }

    public void Send(string message) => 
        Input?.Invoke(this, new StringEventArgs(message));

    public void Kill() => 
        KillProc?.Invoke(this, EventArgs.Empty);

    private void CurrentDomain_ProcessExit(object? sender, EventArgs e) => 
        Kill();
}
