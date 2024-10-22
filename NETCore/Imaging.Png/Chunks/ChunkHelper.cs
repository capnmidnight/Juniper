using Hjg.Pngcs.Zlib;
using System.Text;

namespace Hjg.Pngcs.Chunks;

/// <summary>
/// Static utility methods for CHunks
/// </summary>
/// <remarks>
/// Client code should rarely need this, see PngMetada and ChunksList
/// </remarks>
public static class ChunkHelper
{
    internal const string IHDR = "IHDR";
    internal const string PLTE = "PLTE";
    internal const string IDAT = "IDAT";
    internal const string IEND = "IEND";
    internal const string cHRM = "cHRM";// No Before PLTE and IDAT
    internal const string gAMA = "gAMA";// No Before PLTE and IDAT
    internal const string iCCP = "iCCP";// No Before PLTE and IDAT
    internal const string sBIT = "sBIT";// No Before PLTE and IDAT
    internal const string sRGB = "sRGB";// No Before PLTE and IDAT
    internal const string bKGD = "bKGD";// No After PLTE; before IDAT
    internal const string hIST = "hIST";// No After PLTE; before IDAT
    internal const string tRNS = "tRNS";// No After PLTE; before IDAT
    internal const string pHYs = "pHYs";// No Before IDAT
    internal const string sPLT = "sPLT";// Yes Before IDAT
    internal const string tIME = "tIME";// No None
    internal const string iTXt = "iTXt";// Yes None
    internal const string tEXt = "tEXt";// Yes None
    internal static readonly byte[] b_IHDR = ToBytes(IHDR);
    internal static readonly byte[] b_PLTE = ToBytes(PLTE);
    internal static readonly byte[] b_IDAT = ToBytes(IDAT);
    internal static readonly byte[] b_IEND = ToBytes(IEND);

    /// <summary>
    /// Converts to bytes using Latin1 (ISO-8859-1)
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static byte[] ToBytes(string x)
    {
        return Hjg.Pngcs.PngHelperInternal.charsetLatin1.GetBytes(x);
    }

    /// <summary>
    /// Converts to string using Latin1 (ISO-8859-1)
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static string ToString(byte[] x)
    {
        return Hjg.Pngcs.PngHelperInternal.charsetLatin1.GetString(x);
    }

    /// <summary>
    ///  Converts to string using Latin1 (ISO-8859-1)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="offset"></param>
    /// <param name="len"></param>
    /// <returns></returns>
    public static string ToString(byte[] x, int offset, int len)
    {
        return Hjg.Pngcs.PngHelperInternal.charsetLatin1.GetString(x, offset, len);
    }

    /// <summary>
    /// Converts to bytes using UTF-8
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static byte[] ToBytesUTF8(string x)
    {
        return Encoding.UTF8.GetBytes(x);
    }

    /// <summary>
    /// Converts to string using UTF-8
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static string ToStringUTF8(byte[] x)
    {
        return Encoding.UTF8.GetString(x);
    }

    /// <summary>
    /// Converts to string using UTF-8
    /// </summary>
    /// <param name="x"></param>
    /// <param name="offset"></param>
    /// <param name="len"></param>
    /// <returns></returns>
    public static string ToStringUTF8(byte[] x, int offset, int len)
    {
        return Encoding.UTF8.GetString(x, offset, len);
    }

    /// <summary>
    /// Writes full array of bytes to stream
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="bytes"></param>
    public static void WriteBytesToStream(Stream stream, byte[] bytes)
    {
        if (stream is null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        if (bytes is null)
        {
            throw new ArgumentNullException(nameof(bytes));
        }

        stream.Write(bytes, 0, bytes.Length);
    }

    /// <summary>
    /// Critical chunks: first letter is uppercase
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool IsCritical(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentException("IsCritical flag occurs in first character of ID.", nameof(id));
        }

        // first letter is uppercase
        return char.IsUpper(id[0]);
    }

    /// <summary>
    /// Public chunks: second letter is uppercase
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool IsPublic(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentException("IsPublic flag occurs in second character of ID.", nameof(id));
        }

        // public chunk?
        return char.IsUpper(id[1]);
    }

    /// <summary>
    /// Safe to copy chunk: fourth letter is lower case
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool IsSafeToCopy(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentException("IsSafeToCopy flag occurs in fourth character of ID.", nameof(id));
        }

        // safe to copy?
        // fourth letter is lower case
        return !char.IsUpper(id[3]);
    }

    /// <summary>
    /// We consider a chunk as "unknown" if our chunk factory (even when it has been augmented by client code) doesn't recognize it
    /// </summary>
    /// <param name="chunk"></param>
    /// <returns></returns>
    public static bool IsUnknown(AbstractPngChunk chunk)
    {
        return chunk is PngChunkUNKNOWN;
    }

    /// <summary>
    /// Finds position of null byte in array
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns>-1 if not found</returns>
    public static int PosNullByte(byte[] bytes)
    {
        if (bytes is null)
        {
            throw new ArgumentNullException(nameof(bytes));
        }

        for (var i = 0; i < bytes.Length; i++)
        {
            if (bytes[i] == 0)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Decides if a chunk should be loaded, according to a ChunkLoadBehaviour
    /// </summary>
    /// <param name="id"></param>
    /// <param name="behav"></param>
    /// <returns></returns>
    public static bool ShouldLoad(string id, ChunkLoadBehaviour behav)
    {
        if (IsCritical(id))
        {
            return true;
        }

        var kwown = AbstractPngChunk.IsKnown(id);
        return behav switch
        {
            ChunkLoadBehaviour.LOAD_CHUNK_ALWAYS => true,
            ChunkLoadBehaviour.LOAD_CHUNK_IF_SAFE => kwown || IsSafeToCopy(id),
            ChunkLoadBehaviour.LOAD_CHUNK_KNOWN => kwown,
            ChunkLoadBehaviour.LOAD_CHUNK_NEVER => false,
            _ => false,// should not reach here
        };
    }

    internal static byte[] CompressBytes(byte[] ori, bool compress)
    {
        return CompressBytes(ori, 0, ori.Length, compress);
    }

    internal static byte[] CompressBytes(byte[] ori, int offset, int len, bool compress)
    {
        try
        {
            var inb = new MemoryStream(ori, offset, len);
            Stream inx = inb;
            if (!compress)
            {
                inx = new ZlibInputStream(inb, false);
            }

            var outb = new MemoryStream();
            Stream outx = outb;
            if (compress)
            {
                outx = new ZlibOutputStream(outb, DeflateCompressLevel.DEFAULT, EDeflateCompressStrategy.Default, false);
            }

            ShovelInToOut(inx, outx);
            inx.Close();
            outx.Close();
            var res = outb.ToArray();
            return res;
        }
        catch (Exception e)
        {
            throw new PngjException(e);
        }
    }

    private static void ShovelInToOut(Stream inx, Stream outx)
    {
        var buffer = new byte[1024];
        int len;
        while ((len = inx.Read(buffer, 0, 1024)) > 0)
        {
            outx.Write(buffer, 0, len);
        }
    }

    internal static bool MaskMatch(int v, int mask)
    {
        return (v & mask) != 0;
    }

    /// <summary>
    /// Filters a list of Chunks, keeping those which match the predicate
    /// </summary>
    /// <remarks>The original list is not altered</remarks>
    /// <param name="list"></param>
    /// <param name="predicateKeep"></param>
    /// <returns></returns>
    public static List<AbstractPngChunk> FilterList(List<AbstractPngChunk> list, IChunkPredicate predicateKeep)
    {
        if (list is null)
        {
            throw new ArgumentNullException(nameof(list));
        }

        if (predicateKeep is null)
        {
            throw new ArgumentNullException(nameof(predicateKeep));
        }

        var result = new List<AbstractPngChunk>();
        foreach (var element in list)
        {
            if (predicateKeep.Matches(element))
            {
                result.Add(element);

            }
        }

        return result;
    }

    /// <summary>
    /// Filters a list of Chunks, removing those which match the predicate
    /// </summary>
    /// <remarks>The original list is not altered</remarks>
    /// <param name="list"></param>
    /// <param name="predicateRemove"></param>
    /// <returns></returns>
    public static int TrimList(List<AbstractPngChunk> list, IChunkPredicate predicateRemove)
    {
        if (list is null)
        {
            throw new ArgumentNullException(nameof(list));
        }

        if (predicateRemove is null)
        {
            throw new ArgumentNullException(nameof(predicateRemove));
        }

        var cont = 0;
        for (var i = list.Count - 1; i >= 0; i--)
        {
            if (predicateRemove.Matches(list[i]))
            {
                list.RemoveAt(i);

                cont++;
            }
        }

        return cont;
    }

    /// <summary>
    /// Ad-hoc criteria for 'equivalent' chunks.
    /// </summary>
    ///  <remarks>
    /// Two chunks are equivalent if they have the same Id AND either:
    /// 1. they are Single
    /// 2. both are textual and have the same key
    /// 3. both are SPLT and have the same palette name
    /// Bear in mind that this is an ad-hoc, non-standard, nor required (nor wrong)
    /// criterion. Use it only if you find it useful. Notice that PNG allows to have
    /// repeated textual keys with same keys.
    /// </remarks>
    /// <param name="c1">Chunk1</param>
    /// <param name="c2">Chunk1</param>
    /// <returns>true if equivalent</returns>
    public static bool Equivalent(AbstractPngChunk c1, AbstractPngChunk c2)
    {
        if (c1 == c2)
        {
            return true;
        }

        if (c1 is null
            || c2 is null
            || !c1.Id.Equals(c2.Id, StringComparison.Ordinal))
        {
            return false;
        }

        // same id
        if (c1.GetType() != c2.GetType())
        {
            return false; // should not happen
        }

        if (!c2.AllowsMultiple())
        {
            return true;
        }

        if (c1 is AbstractPngChunkTextVar pngChunkTextVar)
        {
            return pngChunkTextVar
                .Key
                .Equals(((AbstractPngChunkTextVar)c2).Key, StringComparison.Ordinal);
        }

        if (c1 is PngChunkSPLT pngChunkSPLT)
        {
            return pngChunkSPLT.PalName.Equals(((PngChunkSPLT)c2).PalName, StringComparison.Ordinal);
        }

        // unknown chunks that allow multiple? consider they don't match
        return false;
    }

    public static bool IsText(AbstractPngChunk c)
    {
        return c is AbstractPngChunkTextVar;
    }
}