using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Juniper.Logging;

namespace Juniper.Processes
{
    public abstract class AbstractShellCommand :
        ILoggingSource
    {
        private readonly string command;

        public event EventHandler<string> Info;
        public event EventHandler<string> Warning;
        public event EventHandler<ErrorEventArgs> Err;

        public bool UseShellExecute { get; set; }

        public bool LoadUserProfile { get; set; }

        public Encoding Encoding { get; set; }

        public string TotalStandardOutput { get; private set; }

        public string TotalStandardError { get; private set; }

        protected AbstractShellCommand(string command)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (command.Length == 0)
            {
                throw new InvalidOperationException($"{nameof(command)} cannot be an empty string.");
            }

            this.command = command;
        }

        protected abstract IEnumerable<string> Arguments
        {
            get;
        }

        public Task<int> RunAsync()
        {
            return RunAsync(Arguments);
        }

#if DEBUG
        public string lastCommand;
#endif

        protected virtual async Task<int> RunAsync(IEnumerable<string> arguments)
        {
            TotalStandardOutput = string.Empty;
            TotalStandardError = string.Empty;

            using var proc = new Process()
            {
                StartInfo = new ProcessStartInfo(command)
                {
                    Arguments = arguments.ToArray().Join(' '),
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

#if DEBUG
            lastCommand = $"{Environment.CurrentDirectory}> {proc.StartInfo.FileName} {proc.StartInfo.Arguments}";
#endif

            var outputAccum = new StringBuilder();
            void Proc_AccumOutputData(object sender, DataReceivedEventArgs e) =>
                _ = outputAccum.AppendLine(e.Data);

            var errorAccum = new StringBuilder();
            void Proc_AccumErrorData(object sender, DataReceivedEventArgs e) =>
                _ = errorAccum.AppendLine(e.Data);

            proc.OutputDataReceived += Proc_AccumOutputData;
            proc.OutputDataReceived += Proc_OutputDataReceived;
            proc.ErrorDataReceived += Proc_AccumErrorData;
            proc.ErrorDataReceived += Proc_ErrorDataReceived;

            try
            {
                if (!proc.Start())
                {
                    throw new InvalidOperationException($"Could not start process.");
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

            await Task.Run(proc.WaitForExit)
                .ConfigureAwait(false);

            TotalStandardOutput = outputAccum.ToString();
            TotalStandardError = errorAccum.ToString();

            proc.ErrorDataReceived -= Proc_ErrorDataReceived;
            proc.OutputDataReceived -= Proc_OutputDataReceived;
            proc.ErrorDataReceived -= Proc_AccumErrorData;
            proc.OutputDataReceived -= Proc_AccumOutputData;

            return proc.ExitCode;
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
            Info?.Invoke(this, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnWarning(string message)
        {
            Warning?.Invoke(this, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnError(Exception exp)
        {
            Err?.Invoke(this, new ErrorEventArgs(exp));
        }
    }
}
