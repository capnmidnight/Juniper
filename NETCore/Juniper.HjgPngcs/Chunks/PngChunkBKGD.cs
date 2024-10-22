namespace Hjg.Pngcs.Chunks
{
    /// <summary>
    /// bKGD chunk, see http://www.w3.org/TR/PNG/#11bKGD
    /// </summary>
    public class PngChunkBKGD : PngChunkSingle
    {
        public const string ID = ChunkHelper.bKGD;

        // this chunk structure depends on the image type
        // only one of these is meaningful
        private int gray;

        private int red;
        private int green;
        private int blue;
        private int paletteIndex;

        public PngChunkBKGD(ImageInfo info)
            : base(ID, info)
        {
        }

        public override ChunkOrderingConstraint GetOrderingConstraint()
        {
            return ChunkOrderingConstraint.AFTER_PLTE_BEFORE_IDAT;
        }

        public override ChunkRaw CreateRawChunk()
        {
            ChunkRaw c;
            if (ImgInfo.Greyscale)
            {
                c = CreateEmptyChunk(2, true);
                Hjg.Pngcs.PngHelperInternal.WriteInt2tobytes(gray, c.Data, 0);
            }
            else if (ImgInfo.Indexed)
            {
                c = CreateEmptyChunk(1, true);
                c.Data[0] = (byte)paletteIndex;
            }
            else
            {
                c = CreateEmptyChunk(6, true);
                PngHelperInternal.WriteInt2tobytes(red, c.Data, 0);
                Hjg.Pngcs.PngHelperInternal.WriteInt2tobytes(green, c.Data, 0);
                Hjg.Pngcs.PngHelperInternal.WriteInt2tobytes(blue, c.Data, 0);
            }

            return c;
        }

        public override void ParseFromRaw(ChunkRaw c)
        {
            if (c is null)
            {
                throw new System.ArgumentNullException(nameof(c));
            }

            if (ImgInfo.Greyscale)
            {
                gray = Hjg.Pngcs.PngHelperInternal.ReadInt2fromBytes(c.Data, 0);
            }
            else if (ImgInfo.Indexed)
            {
                paletteIndex = c.Data[0] & 0xff;
            }
            else
            {
                red = Hjg.Pngcs.PngHelperInternal.ReadInt2fromBytes(c.Data, 0);
                green = Hjg.Pngcs.PngHelperInternal.ReadInt2fromBytes(c.Data, 2);
                blue = Hjg.Pngcs.PngHelperInternal.ReadInt2fromBytes(c.Data, 4);
            }
        }

        public override void CloneDataFromRead(AbstractPngChunk other)
        {
            CloneData((PngChunkBKGD)other);
        }

        private void CloneData(PngChunkBKGD other)
        {
            if (other is null)
            {
                throw new System.ArgumentNullException(nameof(other));
            }

            gray = other.gray;
            red = other.red;
            green = other.red;
            blue = other.red;
            paletteIndex = other.paletteIndex;
        }

        /// <summary>
        /// Set gray value (0-255 if bitdept=8)
        /// </summary>
        /// <param name="gray"></param>
        public void SetGray(int gray)
        {
            if (!ImgInfo.Greyscale)
            {
                throw new PngjException("only gray images support this");
            }

            this.gray = gray;
        }

        /// <summary>
        /// Gets gray value
        /// </summary>
        /// <returns>gray value  (0-255 if bitdept=8)</returns>
        public int GetGray()
        {
            if (!ImgInfo.Greyscale)
            {
                throw new PngjException("only gray images support this");
            }

            return gray;
        }

        /// <summary>
        /// Set pallette index - only for indexed
        /// </summary>
        /// <param name="index"></param>
        public void SetPaletteIndex(int index)
        {
            if (!ImgInfo.Indexed)
            {
                throw new PngjException("only indexed (pallete) images support this");
            }

            paletteIndex = index;
        }

        /// <summary>
        /// Get pallette index - only for indexed
        /// </summary>
        /// <returns></returns>
        public int GetPaletteIndex()
        {
            if (!ImgInfo.Indexed)
            {
                throw new PngjException("only indexed (pallete) images support this");
            }

            return paletteIndex;
        }

        /// <summary>
        /// Sets rgb value, only for rgb images
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public void SetRGB(int r, int g, int b)
        {
            if (ImgInfo.Greyscale || ImgInfo.Indexed)
            {
                throw new PngjException("only rgb or rgba images support this");
            }

            red = r;
            green = g;
            blue = b;
        }

        /// <summary>
        /// Gets rgb value, only for rgb images
        /// </summary>
        /// <returns>[r , g, b] array</returns>
        public int[] GetRGB()
        {
            if (ImgInfo.Greyscale || ImgInfo.Indexed)
            {
                throw new PngjException("only rgb or rgba images support this");
            }

            return new int[] { red, green, blue };
        }
    }
}