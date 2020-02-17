using System.Threading;

namespace Juniper.Input
{
    public abstract class AbstractPollingKeyEventSource<KeyT> :
        AbstractKeyEventSource<KeyT>
    {
        private readonly Thread poller;

        public AbstractPollingKeyEventSource()
        {
            var threadStart = new ThreadStart(Update);
            poller = new Thread(threadStart);
        }

        public override void Start()
        {
            base.Start();
            poller.Start();
        }

        public override void Stop()
        {
            base.Stop();
            poller.Join();
        }

        private void Update()
        {
            while (poller.ThreadState == ThreadState.Running)
            {
                lock (KeyState)
                {
                    foreach (var key in Keys)
                    {
                        KeyState[key] = IsKeyDown(key);
                    }

                    UpdateStates();
                }
            }
        }

        protected abstract bool IsKeyDown(KeyT key);
    }
}