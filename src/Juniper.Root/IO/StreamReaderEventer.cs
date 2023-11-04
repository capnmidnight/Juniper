namespace Juniper.IO
{
    public class StreamReaderEventer : IDisposable
    {
        private StreamReader? reader;
        private Thread? thread;
        private bool running;
        private bool disposedValue;

        public event EventHandler<StringEventArgs>? Line;

        public StreamReaderEventer(Stream stream)
        {
            reader = new StreamReader(stream);
            thread = new Thread(Run);
        }

        private void Run()
        {
            if (reader is null)
            {
                throw new ObjectDisposedException(nameof(reader));
            }

            while (running)
            {
                var line = reader.ReadLine();
                if (line is not null)
                {
                    Line?.Invoke(this, new StringEventArgs(line));
                }
                else
                {
                    running = false;
                }
            }
        }

        public void Start()
        {
            if (thread is null)
            {
                throw new ObjectDisposedException(nameof(thread));
            }

            if (!running)
            {
                running = true;
                thread.Start();
            }
        }

        public void Stop()
        {
            if (thread is null)
            {
                throw new ObjectDisposedException(nameof(thread));
            }

            if (running)
            {
                running = false;
                thread.Join();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stop();
                    thread = null;

                    reader?.Dispose();
                    reader = null;
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
    }
}
