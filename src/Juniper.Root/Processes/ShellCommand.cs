using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Juniper.Processes
{
    public class ShellCommand : AbstractCommand
    {
        private static readonly Dictionary<PlatformID, string[]> exts = new()
        {
            { PlatformID.Unix, new[] { "", ".app" } },
            { PlatformID.Win32NT, new[] { ".exe", ".cmd" } },
            { PlatformID.Other, new[] { "" } }
        };

        public static string FindCommandPath(string command)
        {
            return FindCommandPaths(command).FirstOrDefault();
        }

        public static IEnumerable<string> FindCommandPaths(string command)
        {
            var PATH = Environment.GetEnvironmentVariable("PATH");
            var directories = PATH.Split(Path.PathSeparator);
            var execDir = new FileInfo(Environment.ProcessPath).Directory;
            var platform = Environment.OSVersion.Platform;
            return from dir in directories
                        .Prepend(Environment.CurrentDirectory)
                        .Prepend(execDir.FullName)
                        .Distinct()
                   from ext in exts[Environment.OSVersion.Platform]
                   let exe = Path.Combine(dir, command + ext)
                   where File.Exists(exe)
                   select exe;
        }

        private readonly string command;
        private readonly string[] args;
        private readonly DirectoryInfo workingDir;
        private readonly Dictionary<Regex, ICommand[]> stdOutputCommands = new();
        private readonly Dictionary<Regex, ICommand[]> stdErrorCommands = new();

        private CancellationTokenSource canceller;
        private Task task;

        public bool AccumulateOutput { get; set; }

        public bool LoadWindowsUserProfile { get; set; }

        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public List<string> TotalStandardOutput { get; private set; }

        public List<string> TotalStandardError { get; private set; }

        public int? ExitCode { get; private set; }

        public ShellCommand(string command, params string[] args)
            : this(null, command, args)
        {
        }

        public ShellCommand(DirectoryInfo workingDir, string command, params string[] args)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (command.Length == 0)
            {
                throw new InvalidOperationException($"{nameof(command)} cannot be an empty string.");
            }

            this.command = FindCommandPath(command);
            this.args = args ?? Array.Empty<string>();
            this.workingDir = workingDir;

            CommandName = this.args.Prepend(command).ToArray().Join(' ');
            if (workingDir is not null)
            {
                CommandName = $"({workingDir.Name}) {CommandName}";
            }
        }

        public ShellCommand OnStandardOutput(Regex pattern, IEnumerable<ICommand> command)
        {
            stdOutputCommands.Add(pattern, command.ToArray());
            return this;
        }

        public ShellCommand OnStandardError(Regex pattern, IEnumerable<ICommand> command)
        {
            stdErrorCommands.Add(pattern, command.ToArray());
            return this;
        }

        protected override void OnDisposing()
        {
            base.OnDisposing();

            canceller?.Dispose();
            canceller = null;
        }

        public override async Task RunAsync(CancellationToken? token = null)
        {
            if (task is not null)
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
                WorkingDirectory = workingDir?.FullName,
                Arguments = args.ToArray().Join(' '),
                StandardErrorEncoding = Encoding,
                StandardOutputEncoding = Encoding,
                ErrorDialog = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            if (Environment.OSVersion.Platform == PlatformID.Win32NT
                && LoadWindowsUserProfile)
            {
                // We have to use reflection here because the property doesn't exist on other platforms.
                typeof(ProcessStartInfo)
                    .GetProperty("LoadUserProfile")
                    .SetValue(startInfo, true);
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

                void Proc_AccumOutputData(object sender, StringEventArgs e)
                {
                    TotalStandardOutput.Add(e.Value);
                }

                void Proc_AccumErrorData(object sender, StringEventArgs e)
                {
                    TotalStandardError.Add(e.Value);
                }

                Info += Proc_AccumOutputData;
                Warning += Proc_AccumErrorData;

                await ExecuteProcess(proc, token);

                Info -= Proc_AccumOutputData;
                Warning -= Proc_AccumErrorData;
            }
            else
            {
                await ExecuteProcess(proc, token);
            }
        }

        public async Task<string[]> RunForStdOutAsync(CancellationToken? token = null)
        {
            var accum = AccumulateOutput;
            AccumulateOutput = true;
            await RunAsync(token);
            var output = TotalStandardOutput.ToArray();
            if (!accum)
            {
                AccumulateOutput = false;
                TotalStandardOutput = null;
                TotalStandardError = null;
            }
            return output;
        }

        private async Task ExecuteProcess(Process proc, CancellationToken? token)
        {
            canceller = new CancellationTokenSource();
            try
            {
                AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
                AppDomain.CurrentDomain.DomainUnload += CurrentDomain_ProcessExit;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_ProcessExit;
                proc.OutputDataReceived += Proc_OutputDataReceived;
                proc.ErrorDataReceived += Proc_ErrorDataReceived;

                if (!proc.Start())
                {
                    throw new InvalidOperationException("Could not start process.");
                }

                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();

                if (token is null)
                {
                    await RunProcess(proc, canceller.Token).ConfigureAwait(false);
                }
                else
                {
                    using var linkedCanceller = CancellationTokenSource.CreateLinkedTokenSource(token.Value, canceller.Token);
                    await RunProcess(proc, linkedCanceller.Token).ConfigureAwait(false);
                }

                AppDomain.CurrentDomain.ProcessExit -= CurrentDomain_ProcessExit;
                AppDomain.CurrentDomain.DomainUnload -= CurrentDomain_ProcessExit;
                AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_ProcessExit;
                proc.OutputDataReceived -= Proc_OutputDataReceived;
                proc.ErrorDataReceived -= Proc_ErrorDataReceived;

                ExitCode = proc.ExitCode;

                if (ExitCode != 0)
                {
                    throw new Exception($"Non-zero exit value = {ExitCode}");
                }
            }
            finally
            {
                canceller?.Dispose();
                canceller = null;
            }
        }

        private async Task RunProcess(Process proc, CancellationToken token)
        {
            task = Task.Run(proc.WaitForExit, token);
            await task.ConfigureAwait(false);
            if (task.IsCanceled && !proc.HasExited)
            {
                proc.Kill();
            }
            task = null;
        }

        public void Kill()
        {
            canceller?.Cancel();
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Kill();
        }

        private async Task ProcessCommands(Dictionary<Regex, ICommand[]> outputCommands, string line)
        {
            foreach (var (pattern, commands) in outputCommands)
            {
                if (pattern.IsMatch(line))
                {
                    await Task.WhenAll(commands.Select(RunCommand));
                }
            }
        }

        private async Task RunCommand(ICommand command)
        {
            command.Info += Command_Info;
            command.Warning += Command_Warning;
            command.Err += Command_Error;
            await command.RunSafeAsync();
            command.Info -= Command_Info;
            command.Warning -= Command_Warning;
            command.Err -= Command_Error;
        }

        private void Command_Info(object sender, StringEventArgs e)
        {
            OnInfo(e.Value);
        }

        private void Command_Warning(object sender, StringEventArgs e)
        {
            OnWarning(e.Value);
        }

        private void Command_Error(object sender, ErrorEventArgs e)
        {
            OnError(e.Value);
        }

        private void Proc_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data is not null && e.Data.Length > 0)
            {
                OnInfo(e.Data);
                _ = ProcessCommands(stdOutputCommands, e.Data);
            }
        }

        private void Proc_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data is not null && e.Data.Length > 0)
            {
                OnWarning(e.Data);
                _ = ProcessCommands(stdErrorCommands, e.Data);
            }
        }
    }
}
