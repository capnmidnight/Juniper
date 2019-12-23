namespace Hjg.Pngcs.Chunks
{
    /// <summary>
    /// gAMA chunk, see http://www.w3.org/TR/PNG/#11gAMA
    /// </summary>
    public class PngChunkGAMA : PngChunkSingle
    {
        public const string ID = ChunkHelper.gAMA;

        private double gamma;

        public PngChunkGAMA(ImageInfo info)
            : base(ID, info)
        {
        }

        public override ChunkOrderingConstraint GetOrderingConstraint()
        {
            return ChunkOrderingConstraint.BEFORE_PLTE_AND_IDAT;
        }

        public override ChunkRaw CreateRawChunk()
        {
            var c = CreateEmptyChunk(4, true);
            var g = (int)((gamma * 100000) + 0.5d);
            Hjg.Pngcs.PngHelperInternal.WriteInt4tobytes(g, c.Data, 0);
            return c;
        }

        public override void ParseFromRaw(ChunkRaw c)
        {
            if (c.Len != 4)
            {
                throw new PngjException("bad chunk " + c);
            }

            var g = Hjg.Pngcs.PngHelperInternal.ReadInt4fromBytes(c.Data, 0);
            gamma = g / 100000.0d;
        }

        public override void CloneDataFromRead(PngChunk other)
        {
            gamma = ((PngChunkGAMA)other).gamma;
        }

        public double GetGamma()
        {
            return gamma;
        }

        public void SetGamma(double gamma)
        {
            this.gamma = gamma;
        }
    }
}