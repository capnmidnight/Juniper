using System;

namespace Juniper.Data
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
        /// Signal to the background thread that it should stop the repeatition process and shut down.
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
                catch (Exception exp)
                {
                    OnError(exp);
                }
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
