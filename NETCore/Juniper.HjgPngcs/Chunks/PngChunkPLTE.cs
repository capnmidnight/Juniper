using System.Globalization;

namespace Hjg.Pngcs.Chunks
{
    /// <summary>
    /// <para>PLTE Palette chunk: this is the only optional critical chunk</para>
    /// <para>http://www.w3.org/TR/PNG/#11PLTE</para>
    /// </summary>
    public class PngChunkPLTE : PngChunkSingle
    {
        public const string ID = ChunkHelper.PLTE;

        private int nentries;

        private int[] entries;

        public PngChunkPLTE(ImageInfo info)
            : base(ID, info)
        {
            nentries = 0;
        }

        public override ChunkOrderingConstraint GetOrderingConstraint()
        {
            return ChunkOrderingConstraint.NA;
        }

        public override ChunkRaw CreateRawChunk()
        {
            var len = 3 * nentries;
            var rgb = new int[3];
            var c = CreateEmptyChunk(len, true);
            for (int n = 0, i = 0; n < nentries; n++)
            {
                GetEntryRgb(n, rgb);
                c.Data[i++] = (byte)rgb[0];
                c.Data[i++] = (byte)rgb[1];
                c.Data[i++] = (byte)rgb[2];
            }

            return c;
        }

        public override void ParseFromRaw(ChunkRaw c)
        {
            if (c is null)
            {
                throw new System.ArgumentNullException(nameof(c));
            }

            SetNentries(c.Len / 3);
            for (int n = 0, i = 0; n < nentries; n++)
            {
                SetEntry(n, c.Data[i++] & 0xff, c.Data[i++] & 0xff,
                        c.Data[i++] & 0xff);
            }
        }

        public override void CloneDataFromRead(AbstractPngChunk other)
        {
            CloneData((PngChunkPLTE)other);
        }

        private void CloneData(PngChunkPLTE other)
        {
            if (other is null)
            {
                throw new System.ArgumentNullException(nameof(other));
            }

            SetNentries(other.GetNentries());
            System.Array.Copy(other.entries, 0, entries, 0, nentries);
        }

        /// <summary>
        /// Also allocates array
        /// </summary>
        /// <param name="nentries">1-256</param>
        public void SetNentries(int nentries)
        {
            this.nentries = nentries;
            if (nentries < 1 || nentries > 256)
            {
                throw new PngjException("invalid pallette - nentries=" + nentries.ToString(CultureInfo.CurrentCulture));
            }

            if (entries is null || entries.Length != nentries)
            { // alloc
                entries = new int[nentries];
            }
        }

        public int GetNentries()
        {
            return nentries;
        }

        public void SetEntry(int n, int r, int g, int b)
        {
            entries[n] = ((r << 16) | (g << 8) | b);
        }

        /// <summary>
        /// as packed RGB8
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public int GetEntry(int n)
        {
            return entries[n];
        }

        /// <summary>
        /// Gets n'th entry, filling 3 positions of given array, at given offset
        /// </summary>
        /// <param name="index"></param>
        /// <param name="rgb"></param>
        /// <param name="offset"></param>
        public void GetEntryRgb(int index, int[] rgb, int offset)
        {
            if (rgb is null)
            {
                throw new System.ArgumentNullException(nameof(rgb));
            }

            var v = entries[index];
            rgb[offset] = ((v & 0xff0000) >> 16);
            rgb[offset + 1] = ((v & 0xff00) >> 8);
            rgb[offset + 2] = (v & 0xff);
        }

        /// <summary>
        /// shortcut: GetEntryRgb(index, int[] rgb, 0)
        /// </summary>
        /// <param name="n"></param>
        /// <param name="rgb"></param>
        public void GetEntryRgb(int n, int[] rgb)
        {
            GetEntryRgb(n, rgb, 0);
        }

        /// <summary>
        /// minimum allowed bit depth, given palette size
        /// </summary>
        /// <returns>1-2-4-8</returns>
        public int MinBitDepth()
        {
            if (nentries <= 2)
            {
                return 1;
            }
            else if (nentries <= 4)
            {
                return 2;
            }
            else if (nentries <= 16)
            {
                return 4;
            }
            else
            {
                return 8;
            }
        }
    }
}