using System;
using System.IO;

namespace Hjg.Pngcs.Chunks
{
    /// <summary>
    /// zTXt chunk: http://www.w3.org/TR/PNG/#11zTXt
    ///
    /// </summary>
    public class PngChunkZTXT : AbstractPngChunkTextVar
    {
        public const string ID = "zTXt";

        public PngChunkZTXT(ImageInfo info)
            : base(ID, info ?? throw new ArgumentNullException(nameof(info)))
        { }

        public override ChunkRaw CreateRawChunk()
        {
            if (Key.Length == 0)
            {
                throw new PngjException("Text chunk key must be non empty");
            }

            using var ba = new MemoryStream();
            ChunkHelper.WriteBytesToStream(ba, ChunkHelper.ToBytes(Key));
            ba.WriteByte(0); // separator
            ba.WriteByte(0); // compression method: 0
            var textbytes = ChunkHelper.CompressBytes(ChunkHelper.ToBytes(Val), true);
            ChunkHelper.WriteBytesToStream(ba, textbytes);
            var b = ba.ToArray();
            var chunk = CreateEmptyChunk(b.Length, false);
            chunk.Data = b;
            return chunk;
        }

        public override void ParseFromRaw(ChunkRaw c)
        {
            if (c is null)
            {
                throw new ArgumentNullException(nameof(c));
            }

            var nullsep = -1;
            for (var i = 0; i < c.Data.Length; i++)
            { // look for first zero
                if (c.Data[i] != 0)
                {
                    continue;
                }

                nullsep = i;
                break;
            }

            if (nullsep < 0 || nullsep > c.Data.Length - 2)
            {
                throw new PngjException("bad zTXt chunk: no separator found");
            }

            Key = ChunkHelper.ToString(c.Data, 0, nullsep);
            var compmet = (int)c.Data[nullsep + 1];
            if (compmet != 0)
            {
                throw new PngjException("bad zTXt chunk: unknown compression method");
            }

            var uncomp = ChunkHelper.CompressBytes(c.Data, nullsep + 2, c.Data.Length - nullsep - 2, false); // uncompress
            Val = ChunkHelper.ToString(uncomp);
        }

        public override void CloneDataFromRead(AbstractPngChunk other)
        {
            CloneData((PngChunkZTXT)other);
        }

        private void CloneData(PngChunkZTXT other)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            Key = other.Key;
            Val = other.Val;
        }
    }
}