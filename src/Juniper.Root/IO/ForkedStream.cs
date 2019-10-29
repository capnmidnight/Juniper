using System.Collections.Generic;
using System.IO;

namespace Juniper.IO
{
    public class ForkedStream : Stream
    {
        private readonly List<Stream> streams = new List<Stream>();

        public ForkedStream(params Stream[] streams)
        {
            this.streams.AddRange(streams);
        }

        public void AddStream(Stream stream)
        {
            streams.Add(stream);
        }

        public override bool CanRead
        {
            get
            {
                return false;
            }
        }

        public override bool CanSeek
        {
            get
            {
                foreach (var stream in streams)
                {
                    if (!stream.CanSeek)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public override bool CanWrite
        {
            get
            {
                foreach (var stream in streams)
                {
                    if (!stream.CanWrite)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public override long Length
        {
            get
            {
                return streams[0].Length;
            }
        }

        public override long Position
        {
            get
            {
                return streams[0].Position;
            }

            set
            {
                foreach (var stream in streams)
                {
                    stream.Position = value;
                }
            }
        }

        public override void Flush()
        {
            foreach (var stream in streams)
            {
                stream.Flush();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new System.NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long position = 0;
            foreach (var stream in streams)
            {
                position = stream.Seek(offset, origin);
            }
            return position;
        }

        public override void SetLength(long value)
        {
            foreach (var stream in streams)
            {
                stream.SetLength(value);
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            foreach (var stream in streams)
            {
                stream.Write(buffer, offset, count);
            }
        }

        public override void WriteByte(byte value)
        {
            foreach (var stream in streams)
            {
                stream.WriteByte(value);
            }
        }

        public override void Close()
        {
            foreach (var stream in streams)
            {
                stream.Close();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                foreach (var stream in streams)
                {
                    stream.Dispose();
                }

                streams.Clear();
            }
        }
    }
}
