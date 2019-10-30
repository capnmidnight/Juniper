#if !(NETFX_CORE || NET_STANDARD2_0)

using System.IO;

using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

// ONLY IF SHARPZIPLIB IS AVAILABLE

namespace Hjg.Pngcs.Zlib
{
    /// <summary>
    /// Zip input (inflater) based on ShaprZipLib
    /// </summary>
    internal class ZlibInputStream : AZlibInputStream
    {
        private readonly InflaterInputStream ist;

        public ZlibInputStream(Stream st, bool leaveOpen)
            : base(st, leaveOpen)
        {
            ist = new InflaterInputStream(st)
            {
                IsStreamOwner = !leaveOpen
            };
        }

        public override int Read(byte[] array, int offset, int count)
        {
            return ist.Read(array, offset, count);
        }

        public override int ReadByte()
        {
            return ist.ReadByte();
        }

        public override void Close()
        {
            ist.Close();
        }

        public override void Flush()
        {
            ist.Flush();
        }

        public override string GetImplementationId()
        {
            return "Zlib inflater: SharpZipLib";
        }
    }
}

#endif