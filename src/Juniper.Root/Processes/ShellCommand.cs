using Juniper.Logging;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Juniper.Processes
{
    public class ShellCommand :
        ILoggingSource,
        IDisposable
    {
        private static readonly string[] exts = {
            "",
            ".exe",
            ".cmd"
        };

        private readonly string command;
        private readonly string[] args;
        private readonly CancellationTokenSource canceller = new();

        private bool disposedValue;
        private Task task;

        public event EventHandler<StringEventArgs> Info;
        public event EventHandler<StringEventArgs> Warning;
        public event EventHandler<ErrorEventArgs> Err;

        public bool LoadUserProfile { get; set; }

        public bool AccumulateOutput { get; set; }

        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public string CommandName { get; private set; }

        public string TotalStandardOutput { get; private set; }

        public string TotalStandardError { get; private set; }

        public static string FindCommandPath(string command)
        {
            var PATH = Environment.GetEnvironmentVariable("PATH");
            var directories = PATH.Split(Path.PathSeparator);
            var choices = from dir in directories.Prepend(Environment.CurrentDirectory)
                          from ext in exts
                          where ext.Length > 0 || Environment.OSVersion.Platform == PlatformID.Unix
                          let exe = Path.Combine(dir, command + ext)
                          where File.Exists(exe)
                          select exe;

            return choices.FirstOrDefault();
        }

        public ShellCommand(string command, params string[] args)
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

            CommandName = this.args.Prepend(command).ToArray().Join(' ');
        }

        public async Task<int> RunAsync(CancellationToken? token = null)
        {
            if (task is not null)
            {
                throw new InvalidOperationException("Cannot invoke the command a second time");
            }

            if (command is null)
            {
                throw new InvalidOperationException("Cannot find command: " + CommandName);
            }

            using var proc = new Process()
            {
                StartInfo = new ProcessStartInfo(command)
                {
                    Arguments = args.ToArray().Join(' '),
                    UseShellExecute = false,
                    LoadUserProfile = LoadUserProfile,
                    StandardErrorEncoding = Encoding,
                    StandardOutputEncoding = Encoding,
                    ErrorDialog = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };

            if (AccumulateOutput)
            {
                TotalStandardOutput = string.Empty;
                TotalStandardError = string.Empty;

                var outputAccum = new StringBuilder();
                var errorAccum = new StringBuilder();

                void Proc_AccumOutputData(object sender, DataReceivedEventArgs e)
                {
                    _ = outputAccum.AppendLine(e.Data);
                }

                void Proc_AccumErrorData(object sender, DataReceivedEventArgs e)
                {
                    _ = errorAccum.AppendLine(e.Data);
                }

                proc.OutputDataReceived += Proc_AccumOutputData;
                proc.ErrorDataReceived += Proc_AccumErrorData;

                await ExecuteProcess(proc, token);

                proc.OutputDataReceived -= Proc_AccumOutputData;
                proc.ErrorDataReceived -= Proc_AccumErrorData;

                TotalStandardOutput = outputAccum.ToString();
                TotalStandardError = errorAccum.ToString();
            }
            else
            {
                await ExecuteProcess(proc, token);
            }

            return proc.ExitCode;
        }

        private async Task ExecuteProcess(Process proc, CancellationToken? token)
        {
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

            proc.OutputDataReceived -= Proc_OutputDataReceived;
            proc.ErrorDataReceived -= Proc_ErrorDataReceived;
        }

        public async Task<int> RunSafeAsync(CancellationToken? token = null)
        {
            try
            {
                return await RunAsync(token);
            }
            catch (Exception ex)
            {
                OnError(ex);
                return -1;
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
            canceller.Cancel();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (task?.IsCompleted == false)
                    {
                        Kill();
                        task.RunSynchronously();
                    }
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

        private void Proc_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            OnInfo(e.Data);
        }

        private void Proc_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            OnWarning(e.Data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnInfo(string message)
        {
            Info?.Invoke(this, new StringEventArgs(message));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnWarning(string message)
        {
            Warning?.Invoke(this, new StringEventArgs(message));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnError(Exception exp)
        {
            Err?.Invoke(this, new ErrorEventArgs(exp));
        }
    }
}
