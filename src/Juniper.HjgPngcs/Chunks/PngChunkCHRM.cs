namespace Hjg.Pngcs.Chunks
{
    /// <summary>
    /// cHRM chunk, see http://www.w3.org/TR/PNG/#11cHRM
    /// </summary>
    public class PngChunkCHRM : PngChunkSingle
    {
        public const string ID = ChunkHelper.cHRM;

        private double whitex, whitey;
        private double redx, redy;
        private double greenx, greeny;
        private double bluex, bluey;

        public PngChunkCHRM(ImageInfo info)
            : base(ID, info)
        {
        }

        public override ChunkOrderingConstraint GetOrderingConstraint()
        {
            return ChunkOrderingConstraint.AFTER_PLTE_BEFORE_IDAT;
        }

        public override ChunkRaw CreateRawChunk()
        {
            var c = CreateEmptyChunk(32, true);
            PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(whitex), c.Data, 0);
            PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(whitey), c.Data, 4);
            PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(redx), c.Data, 8);
            PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(redy), c.Data, 12);
            PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(greenx), c.Data, 16);
            PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(greeny), c.Data, 20);
            PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(bluex), c.Data, 24);
            PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(bluey), c.Data, 28);
            return c;
        }

        public override void ParseFromRaw(ChunkRaw c)
        {
            if (c is null)
            {
                throw new System.ArgumentNullException(nameof(c));
            }

            if (c.Len != 32)
            {
                throw new PngjException("bad chunk " + c);
            }

            whitex = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 0));
            whitey = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 4));
            redx = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 8));
            redy = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 12));
            greenx = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 16));
            greeny = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 20));
            bluex = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 24));
            bluey = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 28));
        }

        public override void CloneDataFromRead(AbstractPngChunk other)
        {
            CloneData((PngChunkCHRM)other);
        }

        private void CloneData(PngChunkCHRM other)
        {
            if (other is null)
            {
                throw new System.ArgumentNullException(nameof(other));
            }

            whitex = other.whitex;
            whitey = other.whitex;
            redx = other.redx;
            redy = other.redy;
            greenx = other.greenx;
            greeny = other.greeny;
            bluex = other.bluex;
            bluey = other.bluey;
        }

        public void SetChromaticities(double whitex, double whitey, double redx, double redy, double greenx, double greeny,
                double bluex, double bluey)
        {
            this.whitex = whitex;
            this.redx = redx;
            this.greenx = greenx;
            this.bluex = bluex;
            this.whitey = whitey;
            this.redy = redy;
            this.greeny = greeny;
            this.bluey = bluey;
        }

        public double[] GetChromaticities()
        {
            return new double[] { whitex, whitey, redx, redy, greenx, greeny, bluex, bluey };
        }
    }
}