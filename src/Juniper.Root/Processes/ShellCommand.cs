using Juniper.Logging;
using System.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public bool UseShellExecute { get; set; }

        public bool LoadUserProfile { get; set; }

        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public string LastCommand { get; private set; }

        public string TotalStandardOutput { get; private set; }

        public string TotalStandardError { get; private set; }

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

            var path = Environment.GetEnvironmentVariable("PATH");
            var parts = new List<string>(path.Split(Path.PathSeparator));
            parts.Insert(0, Environment.CurrentDirectory);
            var choices = new List<string>();
            foreach (var part in parts)
            {
                foreach (var ext in exts)
                {
                    if (ext.Length > 0 || Environment.OSVersion.Platform == PlatformID.Unix)
                    {
                        choices.Add(Path.Combine(part, command + ext));
                    }
                }
            }

            this.command = choices.Where(File.Exists)
                .FirstOrDefault();

            if (this.command is null)
            {
                throw new FileNotFoundException(command);
            }

            this.args = args ?? Array.Empty<string>();
        }

        public async Task<int> RunAsync(CancellationToken? token = null)
        {
            if (task is not null)
            {
                throw new InvalidOperationException("Cannot invoke the command a second time");
            }

            TotalStandardOutput = string.Empty;
            TotalStandardError = string.Empty;

            using var proc = new Process()
            {
                StartInfo = new ProcessStartInfo(command)
                {
                    Arguments = args.ToArray().Join(' '),
                    UseShellExecute = UseShellExecute,
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

            LastCommand = $"{proc.StartInfo.FileName} {proc.StartInfo.Arguments}";

            var outputAccum = new StringBuilder();
            void Proc_AccumOutputData(object sender, DataReceivedEventArgs e)
            {
                _ = outputAccum.AppendLine(e.Data);
            }

            var errorAccum = new StringBuilder();
            void Proc_AccumErrorData(object sender, DataReceivedEventArgs e)
            {
                _ = errorAccum.AppendLine(e.Data);
            }

            proc.OutputDataReceived += Proc_AccumOutputData;
            proc.OutputDataReceived += Proc_OutputDataReceived;
            proc.ErrorDataReceived += Proc_AccumErrorData;
            proc.ErrorDataReceived += Proc_ErrorDataReceived;

            try
            {
                if (!proc.Start())
                {
                    throw new InvalidOperationException("Could not start process.");
                }
            }
            catch (Exception exp)
            {
                exp.Data.Add("ProcessStart", proc.StartInfo);
                OnError(exp);
                throw;
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

            TotalStandardOutput = outputAccum.ToString();
            TotalStandardError = errorAccum.ToString();

            proc.ErrorDataReceived -= Proc_ErrorDataReceived;
            proc.OutputDataReceived -= Proc_OutputDataReceived;
            proc.ErrorDataReceived -= Proc_AccumErrorData;
            proc.OutputDataReceived -= Proc_AccumOutputData;

            return proc.ExitCode;
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
