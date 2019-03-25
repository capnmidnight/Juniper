using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hjg.Pngcs.Zlib
{


    public class ZlibStreamFactory
    {
        public static AZlibInputStream createZlibInputStream(Stream st, bool leaveOpen)
        {
            return new ZlibInputStream(st, leaveOpen);
        }

        public static AZlibInputStream createZlibInputStream(Stream st)
        {
            return createZlibInputStream(st, false);
        }

        public static AZlibOutputStream createZlibOutputStream(Stream st, int compressLevel, EDeflateCompressStrategy strat, bool leaveOpen)
        {
            return new ZlibOutputStream(st, compressLevel, strat, leaveOpen);
        }

        public static AZlibOutputStream createZlibOutputStream(Stream st)
        {
            return createZlibOutputStream(st, false);
        }

        public static AZlibOutputStream createZlibOutputStream(Stream st, bool leaveOpen)
        {
            return createZlibOutputStream(st, DeflateCompressLevel.DEFAULT, EDeflateCompressStrategy.Default, leaveOpen);
        }
    }
}
