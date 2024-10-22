using System;
using System.IO;

namespace Hjg.Pngcs.Zlib
{
    public abstract class AZlibInputStream : Stream
    {
#pragma warning disable IDE1006 // Naming Styles
        protected Stream rawStream { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        protected bool LeaveOpen { get; }

        protected AZlibInputStream(Stream st, bool leaveOpen)
        {
            rawStream = st;
            LeaveOpen = leaveOpen;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !LeaveOpen)
            {
                rawStream.Dispose();
            }

            base.Dispose(disposing);
        }

        public override bool CanRead => true;

        public override bool CanWrite => false;

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
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

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override bool CanTimeout => false;

        /// <summary>
        /// mainly for debugging
        /// </summary>
        /// <returns></returns>
        public abstract string GetImplementationId();
    }
}