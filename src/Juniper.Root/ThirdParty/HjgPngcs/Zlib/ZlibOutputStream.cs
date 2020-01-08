using System.IO;
using System.IO.Compression;

namespace Hjg.Pngcs.Zlib
{
    internal class ZlibOutputStream : AZlibOutputStream
    {
        public ZlibOutputStream(Stream st, int compressLevel, EDeflateCompressStrategy strat, bool leaveOpen) : base(st, compressLevel, strat, leaveOpen)
        {
        }

        private DeflateStream deflateStream; // lazily created, if real read/write is called
        private readonly Adler32 adler32 = new Adler32();
        private bool initdone = false;
        private bool closed = false;

        public override void WriteByte(byte value)
        {
            if (!initdone)
            {
                DoInit();
            }

            if (deflateStream is null)
            {
                InitStream();
            }

            base.WriteByte(value);
            adler32.Update(value);
        }

        public override void Write(byte[] array, int offset, int count)
        {
            if (count == 0)
            {
                return;
            }

            if (!initdone)
            {
                DoInit();
            }

            if (deflateStream is null)
            {
                InitStream();
            }

            deflateStream.Write(array, offset, count);
            adler32.Update(array, offset, count);
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
            // sigh ... no only must I close the parent stream to force a flush, but I must save a reference
            // raw stream because (apparently) Close() sets it to null (shame on you, MS developers)
            if (deflateStream is object)
            {
                deflateStream.Close();
            }
            else
            {         // second hack: empty input?
                RawStream.WriteByte(3);
                RawStream.WriteByte(0);
            }
            // add crc
            var crcv = adler32.GetValue();
            RawStream.WriteByte((byte)((crcv >> 24) & 0xFF));
            RawStream.WriteByte((byte)((crcv >> 16) & 0xFF));
            RawStream.WriteByte((byte)((crcv >> 8) & 0xFF));
            RawStream.WriteByte((byte)(crcv & 0xFF));
            if (!LeaveOpen)
            {
                RawStream.Close();
            }
        }

        private void InitStream()
        {
            if (deflateStream is object)
            {
                return;
            }
            // I must create the DeflateStream only if necessary, because of its bug with empty input (sigh)
            // I must create with leaveopen=true always and do the closing myself, because MS moronic implementation of DeflateStream: I cant force a flush of the underlying stream witouth closing (sigh bis)
            var clevel = CompressionLevel.Optimal;
            // thaks for the granularity, MS!
            if (CompressLevel >= 1 && CompressLevel <= 5)
            {
                clevel = CompressionLevel.Fastest;
            }
            else if (CompressLevel == 0)
            {
                clevel = CompressionLevel.NoCompression;
            }

            deflateStream = new DeflateStream(RawStream, clevel, true);
        }

        private void DoInit()
        {
            if (initdone)
            {
                return;
            }

            initdone = true;
            // http://stackoverflow.com/a/2331025/277304
            const int cmf = 0x78;
            var flg = 218;  // sorry about the following lines
            if (CompressLevel >= 5 && CompressLevel <= 6)
            {
                flg = 156;
            }
            else if (CompressLevel >= 3 && CompressLevel <= 4)
            {
                flg = 94;
            }
            else if (CompressLevel <= 2)
            {
                flg = 1;
            }

            flg -= (((cmf * 256) + flg) % 31); // just in case
            if (flg < 0)
            {
                flg += 31;
            }

            RawStream.WriteByte(cmf);
            RawStream.WriteByte((byte)flg);
        }

        public override void Flush()
        {
            deflateStream?.Flush();
        }

        public override string GetImplementationId()
        {
            return "Zlib deflater: .Net CLR 4.5";
        }
    }
}