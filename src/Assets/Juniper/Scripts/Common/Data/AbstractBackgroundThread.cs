using System;
using System.Threading.Tasks;

namespace Juniper.Data
{
    /// <summary>
    /// An abstract class for performing an operation on a background thread.
    /// </summary>
    public abstract class AbstractBackgroundThread
    {
        /// <summary>
        /// An event that triggers if the background operation throws an exception.
        /// </summary>
        public event EventHandler<BackgroundThreadErrorEventArgs> Error;

        /// <summary>
        /// Returns true when the background thread is still running.
        /// </summary>
        public virtual bool Running =>
            running;

        /// <summary>
        /// Trigger the <see cref="Error"/> event handler, if it has any listeners attached to it.
        /// </summary>
        /// <param name="exp"></param>
        protected void OnError(Exception exp)
        {
            if (Running)
            {
                Error?.Invoke(this, new BackgroundThreadErrorEventArgs(exp));
            }
        }

        /// <summary>
        /// Create a background thread runner in a stopped state.
        /// </summary>
        protected virtual void Awake() =>
            running = false;

        /// <summary>
        /// Get the background thread up and running.
        /// </summary>
        protected void Begin()
        {
            if (!Running)
            {
                OnBegin();
                Task.Run(new Action(Run));
            }
        }

        /// <summary>
        /// Respond to the thread starting.
        /// </summary>
        protected virtual void OnBegin() =>
            running = true;

        /// <summary>
        /// Override the Execute method in child classes to run functionality on the background thread.
        /// </summary>
        protected abstract void Execute();

        /// <summary>
        /// Whether or not the BackgroundThread is still running.
        /// </summary>
        private bool running;

        /// <summary>
        /// The thread body.
        /// </summary>
        private void Run()
        {
            try
            {
                Execute();
            }
            catch (Exception exp)
            {
                OnError(exp);
            }
            running = false;
        }
    }
}
