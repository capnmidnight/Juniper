using System;
using System.Collections.Generic;
using System.Threading;

namespace Juniper.Serial
{
    public abstract class MockSerialPort : ISerialPort
    {
        protected MockSerialPort(int hertz)
        {
            sleep = 1000 / hertz;
            iterations = MINUTES * 60 * 1000 / sleep;
            BaseStream = new MockStream();
            msgQueue = new Queue<string>();
            generator = new Thread(new ThreadStart(Generate));
            generator.Start();
        }

        public System.IO.Stream BaseStream { get; }

        public int BaudRate { get; set; }

        public bool IsOpen
        {
            get { return isOpen; }
        }

        public string PortName { get; set; }
        public int ReadTimeout { get; set; }
        public bool RtsEnable { get; set; }

        public void Close()
        {
            isOpen = false;
        }

        public void DiscardInBuffer()
        {
        }

        public void DiscardOutBuffer()
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Open()
        {
            isOpen = PortName == FakePortName;
        }

        public string ReadExisting()
        {
            lock (msgQueue)
            {
                var msg = string.Join("\n", msgQueue);
                msgQueue.Clear();
                return msg + "\n";
            }
        }

        public string ReadLine()
        {
            lock (msgQueue)
            {
                return msgQueue.Dequeue();
            }
        }

        public void WriteLine(string msg)
        {
        }

        protected int iterations;
        protected int sleep;
        protected abstract string FakePortName { get; }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                BaseStream.Dispose();
            }
        }

        protected void Generate()
        {
            for (var i = PREFILL; i < iterations; ++i)
            {
                lock (msgQueue)
                {
                    msgQueue.Enqueue(MakeRecord(i));
                }
                Thread.Sleep(sleep);
            }
        }

        protected abstract string MakeRecord(int counter);

        private const int MINUTES = 5;
        private const int PREFILL = 4;
        private readonly Thread generator;

        private readonly Queue<string> msgQueue;

        private bool isOpen;

        private class MockStream : System.IO.Stream
        {
            public override bool CanRead { get { return canRead; } }

            public override bool CanSeek { get { return canSeek; } }

            public override bool CanWrite { get { return canWrite; } }

            public override long Length { get { return length; } }

            public override long Position { get; set; }

            public override void Flush()
            {
                // do nothing
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return 0;
            }

            public override long Seek(long offset, System.IO.SeekOrigin origin)
            {
                return 0;
            }

            public override void SetLength(long value)
            {
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotImplementedException();
            }

            internal MockStream()
            {
                canRead = true;
                canSeek = false;
                canWrite = false;
                length = 0;
            }

            private readonly bool canRead;
            private readonly bool canSeek;
            private readonly bool canWrite;
            private readonly long length;
        }
    }
}
