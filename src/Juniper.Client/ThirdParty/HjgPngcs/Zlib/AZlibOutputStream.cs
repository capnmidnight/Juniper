using System;
using System.IO;

namespace Hjg.Pngcs.Zlib
{
    public abstract class AZlibOutputStream : Stream
    {
        protected Stream RawStream { get; set; }

        protected bool LeaveOpen { get; }

        protected int CompressLevel { get; set; }

        protected EDeflateCompressStrategy Strategy { get; }

        protected AZlibOutputStream(Stream st, int compressLevel, EDeflateCompressStrategy strat, bool leaveOpen)
        {
            RawStream = st;
            LeaveOpen = leaveOpen;
            Strategy = strat;
            CompressLevel = compressLevel;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !LeaveOpen)
            {
                RawStream.Dispose();
            }

            base.Dispose(disposing);
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override bool CanSeek => false;

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override long Length => throw new NotSupportedException();

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override bool CanRead => false;

        public override bool CanWrite => true;

        public override bool CanTimeout => false;

        /// <summary>
        /// mainly for debugging
        /// </summary>
        /// <returns></returns>
        public abstract string GetImplementationId();
    }
}