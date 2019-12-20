namespace Hjg.Pngcs.Chunks
{
    /// <summary>
    /// sRGB chunk: http://www.w3.org/TR/PNG/#11sRGB
    /// </summary>
    public class PngChunkSRGB : PngChunkSingle
    {
        public const string ID = ChunkHelper.sRGB;

        public const int RENDER_INTENT_Perceptual = 0;
        public const int RENDER_INTENT_Relative_colorimetric = 1;
        public const int RENDER_INTENT_Saturation = 2;
        public const int RENDER_INTENT_Absolute_colorimetric = 3;

        public int Intent { get; set; }

        public PngChunkSRGB(ImageInfo info)
            : base(ID, info)
        {
        }

        public override ChunkOrderingConstraint GetOrderingConstraint()
        {
            return ChunkOrderingConstraint.BEFORE_PLTE_AND_IDAT;
        }

        public override ChunkRaw CreateRawChunk()
        {
            ChunkRaw c = CreateEmptyChunk(1, true);
            c.Data[0] = (byte)Intent;
            return c;
        }

        public override void ParseFromRaw(ChunkRaw c)
        {
            if (c.Len != 1)
            {
                throw new PngjException("bad chunk length " + c);
            }

            Intent = PngHelperInternal.ReadInt1fromByte(c.Data, 0);
        }

        public override void CloneDataFromRead(PngChunk other)
        {
            var otherx = (PngChunkSRGB)other;
            Intent = otherx.Intent;
        }
    }
}