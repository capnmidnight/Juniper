using System;
using System.Globalization;

namespace Hjg.Pngcs.Chunks
{
    /// <summary>
    /// tIME chunk: http://www.w3.org/TR/PNG/#11tIME
    /// </summary>
    public class PngChunkTIME : PngChunkSingle
    {
        public const string ID = ChunkHelper.tIME;

        private DateTime timestamp;

        public PngChunkTIME(ImageInfo info)
            : base(ID, info)
        {
        }

        public override ChunkOrderingConstraint GetOrderingConstraint()
        {
            return ChunkOrderingConstraint.NONE;
        }

        public override ChunkRaw CreateRawChunk()
        {
            var c = CreateEmptyChunk(7, true);
            PngHelperInternal.WriteInt2tobytes(timestamp.Year, c.Data, 0);
            c.Data[2] = (byte)timestamp.Month;
            c.Data[3] = (byte)timestamp.Day;
            c.Data[4] = (byte)timestamp.Hour;
            c.Data[5] = (byte)timestamp.Minute;
            c.Data[6] = (byte)timestamp.Second;
            return c;
        }

        public override void ParseFromRaw(ChunkRaw c)
        {
            if (c is null)
            {
                throw new ArgumentNullException(nameof(c));
            }

            if (c.Len != 7)
            {
                throw new PngjException("bad chunk " + c);
            }

            var year = PngHelperInternal.ReadInt2fromBytes(c.Data, 0);
            var mon = PngHelperInternal.ReadInt1fromByte(c.Data, 2);
            var day = PngHelperInternal.ReadInt1fromByte(c.Data, 3);
            var hour = PngHelperInternal.ReadInt1fromByte(c.Data, 4);
            var min = PngHelperInternal.ReadInt1fromByte(c.Data, 5);
            var sec = PngHelperInternal.ReadInt1fromByte(c.Data, 6);

            timestamp = new DateTime(year, mon, day, hour, min, sec);
        }

        public override void CloneDataFromRead(AbstractPngChunk other)
        {
            CloneData((PngChunkTIME)other);
        }

        private void CloneData(PngChunkTIME other)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            timestamp = other.timestamp;
        }

        public void SetNow(int secsAgo)
        {
            timestamp = DateTime.Now.AddSeconds(-secsAgo);
        }

        internal void SetYMDHMS(int year, int mon, int day, int hour, int min, int sec)
        {
            timestamp = new DateTime(year, mon, day, hour, min, sec);
        }

        public int[] GetYMDHMS()
        {
            return new int[] {
                timestamp.Year,
                timestamp.Month,
                timestamp.Day,
                timestamp.Hour,
                timestamp.Minute,
                timestamp.Second
            };
        }

        /// <summary>
        /// format YYYY/MM/DD HH:mm:SS
        /// </summary>
        /// <returns></returns>
        public string GetAsString()
        {
            return timestamp.ToString("YYYY/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}