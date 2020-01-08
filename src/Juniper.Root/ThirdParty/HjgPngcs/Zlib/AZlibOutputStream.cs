using System;
using System.IO;

namespace Hjg.Pngcs.Zlib
{
    public abstract class AZlibOutputStream : Stream
    {
        protected Stream rawStream { get; set; }
        protected bool leaveOpen { get; }
        protected int compressLevel { get; set; }

        protected EDeflateCompressStrategy Strategy { get; }

        protected AZlibOutputStream(Stream st, int compressLevel, EDeflateCompressStrategy strat, bool leaveOpen)
        {
            rawStream = st;
            this.leaveOpen = leaveOpen;
            Strategy = strat;
            this.compressLevel = compressLevel;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !leaveOpen)
            {
                rawStream.Dispose();
            }

            base.Dispose(disposing);
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override bool CanTimeout
        {
            get { return false; }
        }

        /// <summary>
        /// mainly for debugging
        /// </summary>
        /// <returns></returns>
        public abstract string GetImplementationId();
    }
}