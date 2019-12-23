namespace Hjg.Pngcs
{
    /// <summary>
    /// <para>Wraps a set of rows from a image, read in a single operation, stored in a int[][] or byte[][] matrix</para>
    /// <para>They can be a subset of the total rows, but in this case they are equispaced.</para>
    /// <para>See also ImageLine</para>
    /// </summary>
    public class ImageLines
    {
        public ImageInfo ImgInfo { get; }
        public ImageLine.ESampleType SampleType { get; }
        public bool SamplesUnpacked { get; }
        public int RowOffset { get; }
        public int Nrows { get; }
        public int RowStep { get; }
        public readonly int channels;
        public readonly int bitDepth;
        public readonly int elementsPerRow;
        public int[][] Scanlines { get; }
        public byte[][] ScanlinesB { get; }

        public ImageLines(ImageInfo ImgInfo, ImageLine.ESampleType sampleType, bool unpackedMode, int rowOffset, int nRows, int rowStep)
        {
            this.ImgInfo = ImgInfo;
            channels = ImgInfo.Channels;
            bitDepth = ImgInfo.BitDepth;
            SampleType = sampleType;
            SamplesUnpacked = unpackedMode || !ImgInfo.Packed;
            RowOffset = rowOffset;
            Nrows = nRows;
            RowStep = rowStep;
            elementsPerRow = unpackedMode ? ImgInfo.SamplesPerRow : ImgInfo.SamplesPerRowPacked;
            if (sampleType == ImageLine.ESampleType.INT)
            {
                Scanlines = new int[nRows][];
                for (var i = 0; i < nRows; i++)
                {
                    Scanlines[i] = new int[elementsPerRow];
                }

                ScanlinesB = null;
            }
            else if (sampleType == ImageLine.ESampleType.BYTE)
            {
                ScanlinesB = new byte[nRows][];
                for (var i = 0; i < nRows; i++)
                {
                    ScanlinesB[i] = new byte[elementsPerRow];
                }

                Scanlines = null;
            }
            else
            {
                throw new PngjInternalException("bad ImageLine initialization");
            }
        }

        /// <summary>
        /// Translates from image row number to matrix row.
        /// If you are not sure if this image row in included, use better ImageRowToMatrixRowStrict
        ///
        /// </summary>
        /// <param name="imrow">Row number in the original image (from 0) </param>
        /// <returns>Row number in the wrapped matrix. Undefined result if invalid</returns>
        public int ImageRowToMatrixRow(int imrow)
        {
            var r = (imrow - RowOffset) / RowStep;
            if (r < 0)
            {
                return 0;
            }
            else if (r < Nrows)
            {
                return r;
            }
            else
            {
                return Nrows - 1;
            }
        }

        /// <summary>
        /// translates from image row number to matrix row
        /// </summary>
        /// <param name="imrow">Row number in the original image (from 0) </param>
        /// <returns>Row number in the wrapped matrix. Returns -1 if invalid</returns>
        public int ImageRowToMatrixRowStrict(int imrow)
        {
            imrow -= RowOffset;
            var mrow = imrow >= 0 && imrow % RowStep == 0 ? imrow / RowStep : -1;
            return mrow < Nrows ? mrow : -1;
        }

        /// <summary>
        /// Translates from matrix row number to real image row number
        /// </summary>
        /// <param name="mrow"></param>
        /// <returns></returns>
        public int MatrixRowToImageRow(int mrow)
        {
            return (mrow * RowStep) + RowOffset;
        }

        /// <summary>
        /// Constructs and returns an ImageLine object backed by a matrix row.
        /// This is quite efficient, no deep copy.
        /// </summary>
        /// <param name="mrow">Row number inside the matrix</param>
        /// <returns></returns>
        public ImageLine GetImageLineAtMatrixRow(int mrow)
        {
            if (mrow < 0 || mrow > Nrows)
            {
                throw new PngjException("Bad row " + mrow + ". Should be positive and less than "
                        + Nrows);
            }

            var imline = SampleType == ImageLine.ESampleType.INT ? new ImageLine(ImgInfo, SampleType,
                    SamplesUnpacked, Scanlines[mrow], null) : new ImageLine(ImgInfo, SampleType,
                    SamplesUnpacked, null, ScanlinesB[mrow]);
            imline.Rown = MatrixRowToImageRow(mrow);
            return imline;
        }
    }
}