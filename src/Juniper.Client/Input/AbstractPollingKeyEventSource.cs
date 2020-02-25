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
            poller.SetApartmentState(ApartmentState.STA);
        }

        public override void Start()
        {
            base.Start();
            poller.Start();
        }

        public override void Quit()
        {
            base.Quit();
            poller.Join();
        }

        private void Update()
        {
            while (IsRunning)
            {
                foreach (var key in Keys)
                {
                    KeyState[key] = IsKeyDown(key);
                }

                UpdateStates();
            }
        }
    }
}