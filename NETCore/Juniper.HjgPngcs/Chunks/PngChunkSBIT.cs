namespace Hjg.Pngcs.Chunks
{
    /// <summary>
    /// <para>sBIT chunk: http://www.w3.org/TR/PNG/#11sBIT</para>
    /// <para>this chunk structure depends on the image type</para>
    /// </summary>
    public class PngChunkSBIT : PngChunkSingle
    {
        public const string ID = ChunkHelper.sBIT;

        //	significant bits
        public int Graysb { get; set; }

        public int Alphasb { get; set; }
        public int Redsb { get; set; }
        public int Greensb { get; set; }
        public int Bluesb { get; set; }

        public PngChunkSBIT(ImageInfo info)
            : base(ID, info)
        {
        }

        public override ChunkOrderingConstraint GetOrderingConstraint()
        {
            return ChunkOrderingConstraint.BEFORE_PLTE_AND_IDAT;
        }

        public override void ParseFromRaw(ChunkRaw c)
        {
            if (c is null)
            {
                throw new System.ArgumentNullException(nameof(c));
            }

            if (c.Len != GetLen())
            {
                throw new PngjException("bad chunk length " + c);
            }

            if (ImgInfo.Greyscale)
            {
                Graysb = PngHelperInternal.ReadInt1fromByte(c.Data, 0);
                if (ImgInfo.Alpha)
                {
                    Alphasb = PngHelperInternal.ReadInt1fromByte(c.Data, 1);
                }
            }
            else
            {
                Redsb = PngHelperInternal.ReadInt1fromByte(c.Data, 0);
                Greensb = PngHelperInternal.ReadInt1fromByte(c.Data, 1);
                Bluesb = PngHelperInternal.ReadInt1fromByte(c.Data, 2);
                if (ImgInfo.Alpha)
                {
                    Alphasb = PngHelperInternal.ReadInt1fromByte(c.Data, 3);
                }
            }
        }

        public override ChunkRaw CreateRawChunk()
        {
            var c = CreateEmptyChunk(GetLen(), true);
            if (ImgInfo.Greyscale)
            {
                c.Data[0] = (byte)Graysb;
                if (ImgInfo.Alpha)
                {
                    c.Data[1] = (byte)Alphasb;
                }
            }
            else
            {
                c.Data[0] = (byte)Redsb;
                c.Data[1] = (byte)Greensb;
                c.Data[2] = (byte)Bluesb;
                if (ImgInfo.Alpha)
                {
                    c.Data[3] = (byte)Alphasb;
                }
            }

            return c;
        }

        public override void CloneDataFromRead(AbstractPngChunk other)
        {
            CloneData((PngChunkSBIT)other);
        }

        private void CloneData(PngChunkSBIT other)
        {
            if (other is null)
            {
                throw new System.ArgumentNullException(nameof(other));
            }

            Graysb = other.Graysb;
            Redsb = other.Redsb;
            Greensb = other.Greensb;
            Bluesb = other.Bluesb;
            Alphasb = other.Alphasb;
        }

        private int GetLen()
        {
            var len = ImgInfo.Greyscale ? 1 : 3;
            if (ImgInfo.Alpha)
            {
                ++len;
            }

            return len;
        }
    }
}