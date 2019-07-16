using System;

namespace Juniper.Threading
{
    /// <summary>
    /// Performs an operation in a loop on a background thread.
    /// </summary>
    public abstract class AbstractBackgroundRepeatingThread : AbstractBackgroundThread
    {
        /// <summary>
        /// Returns true if the thread is still running and not in the middle of shutting down
        /// </summary>
        public override bool Running
        {
            get
            {
                return base.Running && !stopping;
            }
        }

        /// <summary>
        /// Signal to the background thread that it should stop the repetition process and shut down.
        /// </summary>
        public void Stop()
        {
            stopping = true;
        }

        /// <summary>
        /// Create a new background thread, initialized in a non-running, non-shutting down state.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            stopping = false;
        }

        /// <summary>
        /// Code to execute when the thread first starts.
        /// </summary>
        protected override void OnBegin()
        {
            base.OnBegin();
            stopping = false;
        }

        /// <summary>
        /// The routine that runs on the background thread.
        /// </summary>
        protected override void Execute()
        {
            while (!stopping)
            {
                try
                {
                    Loop();
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception exp)
                {
                    OnError(exp);
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }

        /// <summary>
        /// Override to implement code that should execute on every repetition of the thread.
        /// </summary>
        protected abstract void Loop();

        /// <summary>
        /// Whether or not the thread is currently shutting down.
        /// </summary>
        private bool stopping;
    }
}
