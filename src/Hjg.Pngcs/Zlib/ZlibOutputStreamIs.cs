#if !(NETFX_CORE || NET_STANDARD2_0)

using System.IO;

using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

// ONLY IF SHARPZIPLIB IS AVAILABLE

namespace Hjg.Pngcs.Zlib
{
    /// <summary>
    /// Zlib output (deflater) based on ShaprZipLib
    /// </summary>
    internal class ZlibOutputStream : AZlibOutputStream
    {
        private readonly DeflaterOutputStream ost;
        private readonly Deflater deflater;

        public ZlibOutputStream(Stream st, int compressLevel, EDeflateCompressStrategy strat, bool leaveOpen)
            : base(st, compressLevel, strat, leaveOpen)
        {
            deflater = new Deflater(compressLevel);
            setStrat(strat);
            ost = new DeflaterOutputStream(st, deflater)
            {
                IsStreamOwner = !leaveOpen
            };
        }

        public void setStrat(EDeflateCompressStrategy strat)
        {
            if (strat == EDeflateCompressStrategy.Filtered)
            {
                deflater.SetStrategy(DeflateStrategy.Filtered);
            }
            else if (strat == EDeflateCompressStrategy.Huffman)
            {
                deflater.SetStrategy(DeflateStrategy.HuffmanOnly);
            }
            else
            {
                deflater.SetStrategy(DeflateStrategy.Default);
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ost.Write(buffer, offset, count);
        }

        public override void WriteByte(byte value)
        {
            ost.WriteByte(value);
        }

        public override void Close()
        {
            ost.Close();
        }

        public override void Flush()
        {
            ost.Flush();
        }

        public override string GetImplementationId()
        {
            return "Zlib deflater: SharpZipLib";
        }
    }
}

#endif