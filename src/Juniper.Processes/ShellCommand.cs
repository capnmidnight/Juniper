using System.Diagnostics;
using System.Text;

namespace Juniper.Processes
{
    public class ShellCommand : AbstractShellCommand
    {
        private static readonly Dictionary<PlatformID, string[]> exts = new()
        {
            { PlatformID.Unix, new[] { "", ".app" } },
            { PlatformID.Win32NT, new[] { ".exe", ".cmd" } },
            { PlatformID.Other, new[] { "" } }
        };

        public static string? FindCommandPath(string? command)
        {
            if (command is null)
            {
                return null;
            }

            return FindCommandPaths(command).FirstOrDefault();
        }

        public static IEnumerable<string> FindCommandPaths(string? command)
        {
            if (command is null)
            {
                return Array.Empty<string>();
            }

            var PATH = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            var directories = PATH.Split(Path.PathSeparator)
                .Prepend(Environment.CurrentDirectory);

            if (Environment.ProcessPath is not null)
            {
                var execDir = new FileInfo(Environment.ProcessPath).Directory;
                if (execDir is not null)
                {
                    directories = directories.Prepend(execDir.FullName);
                }
            }

            var platform = Environment.OSVersion.Platform;
            return from dir in directories.Distinct()
                   where !string.IsNullOrEmpty(dir)
                        && Directory.Exists(dir)
                   from ext in exts[Environment.OSVersion.Platform]
                   let exe = Path.Combine(dir, command + ext)
                   where File.Exists(exe)
                   select exe;
        }

        private static string MakeCommandName(DirectoryInfo? workingDir, ref string? command, ref string[] args)
        {
            command = FindCommandPath(command);

            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (command.Length == 0)
            {
                throw new InvalidOperationException($"{nameof(command)} cannot be an empty string.");
            }

            args ??= Array.Empty<string>();

            var commandName = args.Prepend(command).ToArray().Join(' ');
            if (workingDir is not null)
            {
                commandName = $"({workingDir.Name}) {commandName}";
            }

            return commandName;
        }

        private readonly string command;
        private readonly string[] args;
        private readonly bool calledFromCurrentDirectory;
        protected readonly DirectoryInfo workingDir;

        private bool running;

        public bool LoadWindowsUserProfile { get; set; }

        public bool CreateWindow { get; set; }

        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public List<string>? TotalStandardOutput { get; private set; }

        public List<string>? TotalStandardError { get; private set; }

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
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            calledFromCurrentDirectory = workingDir is null;
            this.workingDir = workingDir ?? new DirectoryInfo(Directory.GetCurrentDirectory());
            this.command = command;
            this.args = args;
        }

        public override async Task RunAsync()
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
                Arguments = args.ToArray().Join(' '),
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

            if (Environment.OSVersion.Platform == PlatformID.Win32NT
                && LoadWindowsUserProfile)
            {
                // We have to use reflection here because the property doesn't exist on other platforms.
                var prop = typeof(ProcessStartInfo).GetProperty("LoadUserProfile");
                prop?.SetValue(startInfo, true);
            }

            using var proc = new Process()
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };

            if (AccumulateOutput)
            {
                TotalStandardOutput = new();
                TotalStandardError = new();

                void Proc_AccumOutputData(object? sender, StringEventArgs e) =>
                    TotalStandardOutput.Add(e.Value);

                void Proc_AccumErrorData(object? sender, StringEventArgs e) =>
                    TotalStandardError.Add(e.Value);

                Info += Proc_AccumOutputData;
                Warning += Proc_AccumErrorData;

                await ExecuteProcess(proc);

                Info -= Proc_AccumOutputData;
                Warning -= Proc_AccumErrorData;
            }
            else
            {
                await ExecuteProcess(proc);
            }
        }

        public async Task<string[]> RunForStdOutAsync()
        {
            var accum = AccumulateOutput;
            AccumulateOutput = true;
            await RunAsync();
            var output = TotalStandardOutput?.ToArray() ?? Array.Empty<string>();
            if (!accum)
            {
                AccumulateOutput = false;
                TotalStandardOutput = null;
                TotalStandardError = null;
            }
            return output;
        }

        private async Task ExecuteProcess(Process proc)
        {
            var syncroot = new object();
            var completer = new TaskCompletionSource();
            var task = completer.Task;
            void Proc_InputDataReceived(object? sender, StringEventArgs e) =>
                proc.StandardInput.WriteLine(e.Value);

            void Proc_KillProc(object? sender, EventArgs e)
            {
                proc.Kill(true);
                Proc_Exited(sender, e);
            }

            void Proc_Exited(object? sender, EventArgs e)
            {
                if (completer is not null)
                {
                    lock (syncroot)
                    {
                        completer?.SetResult();
                        completer = null;
                    }
                }
            }

            Input += Proc_InputDataReceived;
            KillProc += Proc_KillProc;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_ProcessExit;
            proc.OutputDataReceived += Proc_OutputDataReceived;
            proc.ErrorDataReceived += Proc_ErrorDataReceived;
            proc.Exited += Proc_Exited;

            try
            {
                if (!proc.Start())
                {
                    throw new ProcessStartException("Could not start process.");
                }
            }
            catch(Exception exp)
            {
                throw new ProcessStartException("Could not start process.", exp);
            }

            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();

            await task;

            AppDomain.CurrentDomain.ProcessExit -= CurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.DomainUnload -= CurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_ProcessExit;
            proc.OutputDataReceived -= Proc_OutputDataReceived;
            proc.ErrorDataReceived -= Proc_ErrorDataReceived;
            proc.Exited -= Proc_Exited;
            Input -= Proc_InputDataReceived;
            KillProc -= Proc_KillProc;

            ExitCode = proc.ExitCode;
            running = false;

            if (ExitCode != 0)
            {
                throw new Exception($"Non-zero exit value = {ExitCode}");
            }
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

        public void Send(string message)
        {
            Input?.Invoke(this, new StringEventArgs(message));
        }

        public void Kill()
        {
            KillProc?.Invoke(this, EventArgs.Empty);
        }

        private void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {
            Kill();
        }
    }
}
