namespace Hjg.Pngcs.Chunks
{
    using Hjg.Pngcs;

    /// <summary>
    /// pHYs chunk: http://www.w3.org/TR/PNG/#11pHYs
    /// </summary>
    public class PngChunkPHYS : PngChunkSingle
    {
        public const string ID = ChunkHelper.pHYs;

        public long PixelsxUnitX { get; set; }
        public long PixelsxUnitY { get; set; }

        /// <summary>
        /// 0: unknown 1:metre
        /// </summary>
        public int Units { get; set; }

        public PngChunkPHYS(ImageInfo info)
            : base(ID, info)
        {
        }

        public override ChunkOrderingConstraint GetOrderingConstraint()
        {
            return ChunkOrderingConstraint.BEFORE_IDAT;
        }

        public override ChunkRaw CreateRawChunk()
        {
            var c = createEmptyChunk(9, true);
            Hjg.Pngcs.PngHelperInternal.WriteInt4tobytes((int)PixelsxUnitX, c.Data, 0);
            Hjg.Pngcs.PngHelperInternal.WriteInt4tobytes((int)PixelsxUnitY, c.Data, 4);
            c.Data[8] = (byte)Units;
            return c;
        }

        public override void CloneDataFromRead(PngChunk other)
        {
            var otherx = (PngChunkPHYS)other;
            PixelsxUnitX = otherx.PixelsxUnitX;
            PixelsxUnitY = otherx.PixelsxUnitY;
            Units = otherx.Units;
        }

        public override void ParseFromRaw(ChunkRaw chunk)
        {
            if (chunk.Len != 9)
            {
                throw new PngjException("bad chunk length " + chunk);
            }

            PixelsxUnitX = Hjg.Pngcs.PngHelperInternal.ReadInt4fromBytes(chunk.Data, 0);
            if (PixelsxUnitX < 0)
            {
                PixelsxUnitX += 0x100000000L;
            }

            PixelsxUnitY = Hjg.Pngcs.PngHelperInternal.ReadInt4fromBytes(chunk.Data, 4);
            if (PixelsxUnitY < 0)
            {
                PixelsxUnitY += 0x100000000L;
            }

            Units = Hjg.Pngcs.PngHelperInternal.ReadInt1fromByte(chunk.Data, 8);
        }

        /// <summary>
        /// returns -1 if not in meters, or not equal
        /// </summary>
        /// <returns></returns>
        public double GetAsDpi()
        {
            if (Units != 1 || PixelsxUnitX != PixelsxUnitY)
            {
                return -1;
            }

            return PixelsxUnitX * 0.0254d;
        }

        /// <summary>
        /// returns -1 if the physicial unit is unknown
        /// </summary>
        /// <returns></returns>
        public double[] GetAsDpi2()
        {
            if (Units != 1)
            {
                return new double[] { -1, -1 };
            }

            return new double[] { PixelsxUnitX * 0.0254, PixelsxUnitY * 0.0254 };
        }

        /// <summary>
        /// same in both directions
        /// </summary>
        /// <param name="dpi"></param>
        public void SetAsDpi(double dpi)
        {
            Units = 1;
            PixelsxUnitX = (long)((dpi / 0.0254d) + 0.5d);
            PixelsxUnitY = PixelsxUnitX;
        }

        public void SetAsDpi2(double dpix, double dpiy)
        {
            Units = 1;
            PixelsxUnitX = (long)((dpix / 0.0254) + 0.5);
            PixelsxUnitY = (long)((dpiy / 0.0254) + 0.5);
        }
    }
}