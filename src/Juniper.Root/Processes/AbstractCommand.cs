
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

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    OnDisposing();
                }

                disposedValue = true;
            }
        }

        protected virtual void OnDisposing() { }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public abstract Task RunAsync(CancellationToken? token = null);

        public async Task RunSafeAsync(CancellationToken? token = null)
        {
            try
            {
                await RunAsync(token);
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
