using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;

namespace Hjg.Pngcs.Zlib
{
    /// <summary>
    /// Zip input (deflater) based on Ms DeflateStream (.net 4.5)
    /// </summary>
    internal class ZlibInputStream : AZlibInputStream
    {
        public ZlibInputStream(Stream st, bool leaveOpen)
            : base(st, leaveOpen)
        {
        }

        private DeflateStream deflateStream; // lazily created, if real read is called
        private bool initdone;
        private bool closed;

        // private Adler32 adler32 ; // we dont check adler32!
        private bool fdict;// merely informational, not used
        private byte[] dictid; // merely informational, not used
        private byte[] crcread; // merely informational, not checked

        public override int Read(byte[] array, int offset, int count)
        {
            if (!initdone)
            {
                DoInit();
            }

            if (deflateStream is null && count > 0)
            {
                InitStream();
            }
            // we dont't check CRC on reading
            var r = deflateStream.Read(array, offset, count);
            if (r < 1 && crcread is null)
            {  // deflater has ended. we try to read next 4 bytes from raw stream (crc)
                crcread = new byte[4];
                for (var i = 0; i < 4; i++)
                {
                    crcread[i] = (byte)rawStream.ReadByte(); // we dont really check/use this
                }
            }

            return r;
        }

        public override void Close()
        {
            if (!initdone)
            {
                DoInit(); // can happen if never called write
            }

            if (closed)
            {
                return;
            }

            closed = true;
            deflateStream?.Close();
            if (crcread is null)
            { // eat trailing 4 bytes
                crcread = new byte[4];
                for (var i = 0; i < 4; i++)
                {
                    crcread[i] = (byte)rawStream.ReadByte();
                }
            }

            if (!LeaveOpen)
            {
                rawStream.Close();
            }
        }

        private void InitStream()
        {
            if (deflateStream is object)
            {
                return;
            }

            deflateStream = new DeflateStream(rawStream, CompressionMode.Decompress, true);
        }

        private void DoInit()
        {
            if (initdone)
            {
                return;
            }

            initdone = true;
            // read zlib header : http://www.ietf.org/rfc/rfc1950.txt
            var cmf = rawStream.ReadByte();
            var flag = rawStream.ReadByte();
            if (cmf == -1 || flag == -1)
            {
                return;
            }

            if ((cmf & 0x0f) != 8)
            {
                throw new Exception("Bad compression method for ZLIB header: cmf=" + cmf.ToString(CultureInfo.CurrentCulture));
            }
            //cmdinfo = ((cmf & (0xf0)) >> 8);// not used?
            fdict = (flag & 32) != 0;
            if (fdict)
            {
                dictid = new byte[4];
                for (var i = 0; i < 4; i++)
                {
                    dictid[i] = (byte)rawStream.ReadByte(); // we eat but don't use this
                }
            }
        }

        public override void Flush()
        {
            deflateStream?.Flush();
        }

        public override string GetImplementationId()
        {
            return "Zlib inflater: .Net CLR 4.5";
        }
    }
}