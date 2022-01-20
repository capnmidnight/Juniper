namespace Hjg.Pngcs.Chunks
{
    /// <summary>
    /// oFFs chunk: http://www.libpng.org/pub/png/spec/register/pngext-1.3.0-pdg.html#C.oFFs
    /// </summary>
    public class PngChunkOFFS : PngChunkSingle
    {
        public const string ID = "oFFs";

        private long posX;
        private long posY;
        private int units; // 0: pixel 1:micrometer

        public PngChunkOFFS(ImageInfo info)
            : base(ID, info) { }

        public override ChunkOrderingConstraint GetOrderingConstraint()
        {
            return ChunkOrderingConstraint.BEFORE_IDAT;
        }

        public override ChunkRaw CreateRawChunk()
        {
            var c = CreateEmptyChunk(9, true);
            PngHelperInternal.WriteInt4tobytes((int)posX, c.Data, 0);
            PngHelperInternal.WriteInt4tobytes((int)posY, c.Data, 4);
            c.Data[8] = (byte)units;
            return c;
        }

        public override void ParseFromRaw(ChunkRaw c)
        {
            if (c is null)
            {
                throw new System.ArgumentNullException(nameof(c));
            }

            if (c.Len != 9)
            {
                throw new PngjException("bad chunk length " + c);
            }

            posX = PngHelperInternal.ReadInt4fromBytes(c.Data, 0);
            if (posX < 0)
            {
                posX += 0x100000000L;
            }

            posY = PngHelperInternal.ReadInt4fromBytes(c.Data, 4);
            if (posY < 0)
            {
                posY += 0x100000000L;
            }

            units = PngHelperInternal.ReadInt1fromByte(c.Data, 8);
        }

        public override void CloneDataFromRead(AbstractPngChunk other)
        {
            CloneData((PngChunkOFFS)other);
        }

        private void CloneData(PngChunkOFFS other)
        {
            if (other is null)
            {
                throw new System.ArgumentNullException(nameof(other));
            }

            posX = other.posX;
            posY = other.posY;
            units = other.units;
        }

        /// <summary>
        /// 0: pixel, 1:micrometer
        /// </summary>
        /// <returns></returns>
        public int GetUnits()
        {
            return units;
        }

        /// <summary>
        /// 0: pixel, 1:micrometer
        /// </summary>
        /// <param name="units"></param>
        public void SetUnits(int units)
        {
            this.units = units;
        }

        public long GetPosX()
        {
            return posX;
        }

        public void SetPosX(long posX)
        {
            this.posX = posX;
        }

        public long GetPosY()
        {
            return posY;
        }

        public void SetPosY(long posY)
        {
            this.posY = posY;
        }
    }
}