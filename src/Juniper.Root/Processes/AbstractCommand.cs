
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Juniper.Processes
{
    public abstract class AbstractCommand : ICommand
    {
        public string CommandName { get; protected set; }

        public event EventHandler<StringEventArgs> Info;
        public event EventHandler<StringEventArgs> Warning;
        public event EventHandler<ErrorEventArgs> Err;

        public abstract Task RunAsync();

        public async Task RunSafeAsync()
        {
            try
            {
                await RunAsync();
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void OnInfo(string message)
        {
            Info?.Invoke(this, new StringEventArgs(message));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void OnWarning(string message)
        {
            Warning?.Invoke(this, new StringEventArgs(message));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void OnError(Exception exp)
        {
            Err?.Invoke(this, new ErrorEventArgs(exp));
        }
    }
}
