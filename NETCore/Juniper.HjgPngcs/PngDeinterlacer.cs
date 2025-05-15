namespace Hjg.Pngcs
{
    internal class PngDeinterlacer
    {
        private readonly ImageInfo imi;
        private int pass; // 1-7
        private int rows, cols, dY, dX, oY, oX, oXsamples, dXsamples; // at current pass

        // current row in the virtual subsampled image; this incrementes from 0 to cols/dy 7 times
        private int currRowSubimg = -1;

        // in the real image, this will cycle from 0 to im.rows in different steps, 7 times
        private int currRowReal = -1;

        private readonly int packedValsPerPixel;
        private readonly int packedMask;
        private readonly int packedShift;
        private int[][] imageInt; // FULL image -only used for PngReader as temporary storage
        private byte[][] imageByte;

        internal PngDeinterlacer(ImageInfo iminfo)
        {
            imi = iminfo;
            pass = 0;
            if (imi.Packed)
            {
                packedValsPerPixel = 8 / imi.BitDepth;
                packedShift = imi.BitDepth;
                if (imi.BitDepth == 1)
                {
                    packedMask = 0x80;
                }
                else if (imi.BitDepth == 2)
                {
                    packedMask = 0xc0;
                }
                else
                {
                    packedMask = 0xf0;
                }
            }
            else
            {
                packedMask = packedShift = packedValsPerPixel = 1;// dont care
            }

            SetPass(1);
            SetRow(0);
        }

        /** this refers to the row currRowSubimg */

        internal void SetRow(int n)
        {
            currRowSubimg = n;
            currRowReal = (n * dY) + oY;
            if (currRowReal < 0 || currRowReal >= imi.Rows)
            {
                throw new PngjInternalException("bad row - this should not happen");
            }
        }

        internal void SetPass(int p)
        {
            if (pass == p)
            {
                return;
            }

            pass = p;
            switch (pass)
            {
                case 1:
                dY = dX = 8;
                oX = oY = 0;
                break;

                case 2:
                dY = dX = 8;
                oX = 4;
                oY = 0;
                break;

                case 3:
                dX = 4;
                dY = 8;
                oX = 0;
                oY = 4;
                break;

                case 4:
                dX = dY = 4;
                oX = 2;
                oY = 0;
                break;

                case 5:
                dX = 2;
                dY = 4;
                oX = 0;
                oY = 2;
                break;

                case 6:
                dX = dY = 2;
                oX = 1;
                oY = 0;
                break;

                case 7:
                dX = 1;
                dY = 2;
                oX = 0;
                oY = 1;
                break;

                default:
                throw new PngjInternalException("bad interlace pass" + pass);
            }

            rows = ((imi.Rows - oY) / dY) + 1;
            if (((rows - 1) * dY) + oY >= imi.Rows)
            {
                rows--; // can be 0
            }

            cols = ((imi.Cols - oX) / dX) + 1;
            if (((cols - 1) * dX) + oX >= imi.Cols)
            {
                cols--; // can be 0
            }

            if (cols == 0)
            {
                rows = 0; // really...
            }

            dXsamples = dX * imi.Channels;
            oXsamples = oX * imi.Channels;
        }

        // notice that this is a "partial" deinterlace, it will be called several times for the same row!
        internal void DeinterlaceInt(int[] src, int[] dst, bool readInPackedFormat)
        {
            if (!(imi.Packed && readInPackedFormat))
            {
                for (int i = 0, j = oXsamples; i < cols * imi.Channels; i += imi.Channels, j += dXsamples)
                {
                    for (var k = 0; k < imi.Channels; k++)
                    {
                        dst[j + k] = src[i + k];
                    }
                }
            }
            else
            {
                DeinterlaceIntPacked(src, dst);
            }
        }

        // interlaced+packed = monster; this is very clumsy!
        private void DeinterlaceIntPacked(int[] src, int[] dst)
        {
            int spos, smod, smask; // source byte position, bits to shift to left (01,2,3,4
            int tpos, tmod, p, d;
            smask = packedMask;
            smod = -1;
            // can this really work?
            for (int i = 0, j = oX; i < cols; i++, j += dX)
            {
                spos = i / packedValsPerPixel;
                ++smod;
                if (smod >= packedValsPerPixel)
                {
                    smod = 0;
                }

                smask >>= packedShift; // the source mask cycles
                if (smod == 0)
                {
                    smask = packedMask;
                }

                tpos = j / packedValsPerPixel;
                tmod = j % packedValsPerPixel;
                p = src[spos] & smask;
                d = tmod - smod;
                if (d > 0)
                {
                    p >>= (d * packedShift);
                }
                else if (d < 0)
                {
                    p <<= ((-d) * packedShift);
                }

                dst[tpos] |= p;
            }
        }

        // yes, duplication of code is evil, normally
        internal void DeinterlaceByte(byte[] src, byte[] dst, bool readInPackedFormat)
        {
            if (!(imi.Packed && readInPackedFormat))
            {
                for (int i = 0, j = oXsamples; i < cols * imi.Channels; i += imi.Channels, j += dXsamples)
                {
                    for (var k = 0; k < imi.Channels; k++)
                    {
                        dst[j + k] = src[i + k];
                    }
                }
            }
            else
            {
                DeinterlacePackedByte(src, dst);
            }
        }

        private void DeinterlacePackedByte(byte[] src, byte[] dst)
        {
            int spos, smod, smask; // source byte position, bits to shift to left (01,2,3,4
            int tpos, tmod, p, d;
            // what the heck are you reading here? I told you would not enjoy this. Try Dostoyevsky or Simone Weil instead
            smask = packedMask;
            smod = -1;
            // Arrays.fill(dst, 0);
            for (int i = 0, j = oX; i < cols; i++, j += dX)
            {
                spos = i / packedValsPerPixel;
                ++smod;
                if (smod >= packedValsPerPixel)
                {
                    smod = 0;
                }

                smask >>= packedShift; // the source mask cycles
                if (smod == 0)
                {
                    smask = packedMask;
                }

                tpos = j / packedValsPerPixel;
                tmod = j % packedValsPerPixel;
                p = src[spos] & smask;
                d = tmod - smod;
                if (d > 0)
                {
                    p >>= (d * packedShift);
                }
                else if (d < 0)
                {
                    p <<= ((-d) * packedShift);
                }

                dst[tpos] |= (byte)p;
            }
        }

        /**
         * Is current row the last row for the lass pass??
         */

        internal bool IsAtLastRow()
        {
            return pass == 7 && currRowSubimg == rows - 1;
        }

        /**
         * current row number inside the "sub image"
         */

        internal int GetCurrRowSubimg()
        {
            return currRowSubimg;
        }

        /**
         * current row number inside the "real image"
         */

        internal int GetCurrRowReal()
        {
            return currRowReal;
        }

        /**
         * current pass number (1-7)
         */

        internal int GetPass()
        {
            return pass;
        }

        /**
         * How many rows has the current pass?
         **/

        internal int GetRows()
        {
            return rows;
        }

        /**
         * How many columns (pixels) are there in the current row
         */

        internal int GetCols()
        {
            return cols;
        }

        internal int GetPixelsToRead()
        {
            return GetCols();
        }

        internal int[][] GetImageInt()
        {
            return imageInt;
        }

        internal void SetImageInt(int[][] imageInt)
        {
            this.imageInt = imageInt;
        }

        internal byte[][] GetImageByte()
        {
            return imageByte;
        }

        internal void SetImageByte(byte[][] imageByte)
        {
            this.imageByte = imageByte;
        }
    }
}