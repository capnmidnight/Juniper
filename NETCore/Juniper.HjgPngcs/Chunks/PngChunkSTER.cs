namespace Hjg.Pngcs.Chunks
{
    /// <summary>
    /// sTER chunk: http://www.libpng.org/pub/png/spec/register/pngext-1.3.0-pdg.html#C.sTER
    /// </summary>
    public class PngChunkSTER : PngChunkSingle
    {
        public const string ID = "sTER";

        /// <summary>
        /// 0: cross-fuse layout 1: diverging-fuse layout
        /// </summary>
        public byte Mode { get; set; }

        public PngChunkSTER(ImageInfo info)
            : base(ID, info) { }

        public override ChunkOrderingConstraint GetOrderingConstraint()
        {
            return ChunkOrderingConstraint.BEFORE_IDAT;
        }

        public override ChunkRaw CreateRawChunk()
        {
            var c = CreateEmptyChunk(1, true);
            c.Data[0] = Mode;
            return c;
        }

        public override void ParseFromRaw(ChunkRaw c)
        {
            if (c is null)
            {
                throw new System.ArgumentNullException(nameof(c));
            }

            if (c.Len != 1)
            {
                throw new PngjException("bad chunk length " + c);
            }

            Mode = c.Data[0];
        }

        public override void CloneDataFromRead(AbstractPngChunk other)
        {
            CloneData((PngChunkSTER)other);
        }

        private void CloneData(PngChunkSTER other)
        {
            if (other is null)
            {
                throw new System.ArgumentNullException(nameof(other));
            }

            Mode = other.Mode;
        }
    }
}