using System.Globalization;

namespace Hjg.Pngcs.Chunks
{
    /// <summary>
    /// IHDR chunk: http://www.w3.org/TR/PNG/#11IHDR
    /// </summary>
    public class PngChunkIHDR : PngChunkSingle
    {
        public const string ID = ChunkHelper.IHDR;
        public int Cols { get; set; }
        public int Rows { get; set; }
        public int Bitspc { get; set; }
        public int Colormodel { get; set; }
        public int Compmeth { get; set; }
        public int Filmeth { get; set; }
        public int Interlaced { get; set; }

        public PngChunkIHDR(ImageInfo info)
            : base(ID, info)
        {
        }

        public override ChunkOrderingConstraint GetOrderingConstraint()
        {
            return ChunkOrderingConstraint.NA;
        }

        public override ChunkRaw CreateRawChunk()
        {
            var c = new ChunkRaw(13, ChunkHelper.b_IHDR, true);
            var offset = 0;
            Hjg.Pngcs.PngHelperInternal.WriteInt4tobytes(Cols, c.Data, offset);
            offset += 4;
            Hjg.Pngcs.PngHelperInternal.WriteInt4tobytes(Rows, c.Data, offset);
            offset += 4;
            c.Data[offset++] = (byte)Bitspc;
            c.Data[offset++] = (byte)Colormodel;
            c.Data[offset++] = (byte)Compmeth;
            c.Data[offset++] = (byte)Filmeth;
            c.Data[offset++] = (byte)Interlaced;
            return c;
        }

        public override void ParseFromRaw(ChunkRaw c)
        {
            if (c.Len != 13)
            {
                throw new PngjException("Bad IDHR len " + c.Len.ToString(CultureInfo.CurrentCulture));
            }

            using var st = c.GetAsByteStream();
            Cols = Hjg.Pngcs.PngHelperInternal.ReadInt4(st);
            Rows = Hjg.Pngcs.PngHelperInternal.ReadInt4(st);
            // bit depth: number of bits per channel
            Bitspc = Hjg.Pngcs.PngHelperInternal.ReadByte(st);
            Colormodel = Hjg.Pngcs.PngHelperInternal.ReadByte(st);
            Compmeth = Hjg.Pngcs.PngHelperInternal.ReadByte(st);
            Filmeth = Hjg.Pngcs.PngHelperInternal.ReadByte(st);
            Interlaced = Hjg.Pngcs.PngHelperInternal.ReadByte(st);
        }

        public override void CloneDataFromRead(AbstractPngChunk other)
        {
            var otherx = (PngChunkIHDR)other;
            Cols = otherx.Cols;
            Rows = otherx.Rows;
            Bitspc = otherx.Bitspc;
            Colormodel = otherx.Colormodel;
            Compmeth = otherx.Compmeth;
            Filmeth = otherx.Filmeth;
            Interlaced = otherx.Interlaced;
        }
    }
}