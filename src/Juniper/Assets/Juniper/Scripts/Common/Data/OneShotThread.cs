using System;

namespace Juniper.Data
{
    /// <summary>
    /// Run any System.Action in a background thread that starts right away.
    /// </summary>
    public class OneShotThread : AbstractBackgroundThread
    {
        /// <summary>
        /// Creates a background thread that starts right away, executing an action along the way.
        /// </summary>
        /// <param name="run">The action to execute when the thread runs</param>
        /// <returns>The background thread object that is doing the executing.</returns>
        public static void Run(Action run)
        {
#if NETFX_CORE
            System.Threading.Tasks.Task.Run(act).Start();;
#else
            var t = new OneShotThread(run);
            t.Begin();
#endif
        }

        /// <summary>
        /// Peform the thread's desired operation.
        /// </summary>
        protected override void Execute()
        {
            callback();
        }

        /// <summary>
        /// The callback to run as the body of the thread.
        /// </summary>
        private readonly Action callback;

        /// <summary>
        /// Initialize a thread that runs only once, providing a body for the thread.
        /// </summary>
        /// <param name="run"></param>
        private OneShotThread(Action run)
        {
            callback = run;
        }
    }
}