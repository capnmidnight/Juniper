namespace Hjg.Pngcs.Chunks
{
    /// <summary>
    /// tRNS chunk: http://www.w3.org/TR/PNG/#11tRNS
    /// </summary>
    public class PngChunkTRNS : PngChunkSingle
    {
        public const string ID = ChunkHelper.tRNS;

        // this chunk structure depends on the image type
        // only one of these is meaningful
        private int gray;

        private int red, green, blue;
        private int[] paletteAlpha;

        public PngChunkTRNS(ImageInfo info)
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
                PngHelperInternal.WriteInt2tobytes(gray, c.Data, 0);
            }
            else if (ImgInfo.Indexed)
            {
                c = CreateEmptyChunk(paletteAlpha.Length, true);
                for (var n = 0; n < c.Len; n++)
                {
                    c.Data[n] = (byte)paletteAlpha[n];
                }
            }
            else
            {
                c = CreateEmptyChunk(6, true);
                PngHelperInternal.WriteInt2tobytes(red, c.Data, 0);
                PngHelperInternal.WriteInt2tobytes(green, c.Data, 0);
                PngHelperInternal.WriteInt2tobytes(blue, c.Data, 0);
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
                gray = PngHelperInternal.ReadInt2fromBytes(c.Data, 0);
            }
            else if (ImgInfo.Indexed)
            {
                var nentries = c.Data.Length;
                paletteAlpha = new int[nentries];
                for (var n = 0; n < nentries; n++)
                {
                    paletteAlpha[n] = c.Data[n] & 0xff;
                }
            }
            else
            {
                red = PngHelperInternal.ReadInt2fromBytes(c.Data, 0);
                green = PngHelperInternal.ReadInt2fromBytes(c.Data, 2);
                blue = PngHelperInternal.ReadInt2fromBytes(c.Data, 4);
            }
        }

        public override void CloneDataFromRead(AbstractPngChunk other)
        {
            CloneData((PngChunkTRNS)other);
        }

        private void CloneData(PngChunkTRNS other)
        {
            if (other is null)
            {
                throw new System.ArgumentNullException(nameof(other));
            }

            gray = other.gray;
            red = other.red;
            green = other.green;
            blue = other.blue;
            if (other.paletteAlpha is object)
            {
                paletteAlpha = new int[other.paletteAlpha.Length];
                System.Array.Copy(other.paletteAlpha, 0, paletteAlpha, 0, paletteAlpha.Length);
            }
        }

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

        public int[] GetRGB()
        {
            if (ImgInfo.Greyscale || ImgInfo.Indexed)
            {
                throw new PngjException("only rgb or rgba images support this");
            }

            return new int[] { red, green, blue };
        }

        public void SetGray(int g)
        {
            if (!ImgInfo.Greyscale)
            {
                throw new PngjException("only grayscale images support this");
            }

            gray = g;
        }

        public int GetGray()
        {
            if (!ImgInfo.Greyscale)
            {
                throw new PngjException("only grayscale images support this");
            }

            return gray;
        }

        /// <summary>
        /// WARNING: non deep copy
        /// </summary>
        /// <param name="palAlpha"></param>
        public void SetPalletteAlpha(int[] palAlpha)
        {
            if (!ImgInfo.Indexed)
            {
                throw new PngjException("only indexed images support this");
            }

            paletteAlpha = palAlpha;
        }

        /// <summary>
        /// utiliy method : to use when only one pallete index is set as totally transparent
        /// </summary>
        /// <param name="palAlphaIndex"></param>
        public void SetIndexEntryAsTransparent(int palAlphaIndex)
        {
            if (!ImgInfo.Indexed)
            {
                throw new PngjException("only indexed images support this");
            }

            paletteAlpha = new int[] { palAlphaIndex + 1 };
            for (var i = 0; i < palAlphaIndex; i++)
            {
                paletteAlpha[i] = 255;
            }

            paletteAlpha[palAlphaIndex] = 0;
        }

        /// <summary>
        /// WARNING: non deep copy
        /// </summary>
        /// <returns></returns>
        public int[] GetPalletteAlpha()
        {
            if (!ImgInfo.Indexed)
            {
                throw new PngjException("only indexed images support this");
            }

            return paletteAlpha;
        }
    }
}