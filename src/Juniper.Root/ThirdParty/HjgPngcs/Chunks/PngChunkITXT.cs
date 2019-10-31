namespace Hjg.Pngcs.Chunks
{
    using System.IO;
    using Hjg.Pngcs;

    /// <summary>
    /// iTXt chunk:  http://www.w3.org/TR/PNG/#11iTXt
    /// One of the three text chunks
    /// </summary>
    public class PngChunkITXT : PngChunkTextVar
    {
        public const string ID = ChunkHelper.iTXt;

        private bool compressed = false;
        private string langTag = "";
        private string translatedTag = "";

        public PngChunkITXT(ImageInfo info)
            : base(ID, info)
        {
        }

        public override ChunkRaw CreateRawChunk()
        {
            if (key.Length == 0)
            {
                throw new PngjException("Text chunk key must be non empty");
            }

            var ba = new MemoryStream();
            ChunkHelper.WriteBytesToStream(ba, ChunkHelper.ToBytes(key));
            ba.WriteByte(0); // separator
            ba.WriteByte(compressed ? (byte)1 : (byte)0);
            ba.WriteByte(0); // compression method (always 0)
            ChunkHelper.WriteBytesToStream(ba, ChunkHelper.ToBytes(langTag));
            ba.WriteByte(0); // separator
            ChunkHelper.WriteBytesToStream(ba, ChunkHelper.ToBytesUTF8(translatedTag));
            ba.WriteByte(0); // separator
            var textbytes = ChunkHelper.ToBytesUTF8(val);
            if (compressed)
            {
                textbytes = ChunkHelper.compressBytes(textbytes, true);
            }
            ChunkHelper.WriteBytesToStream(ba, textbytes);
            var b = ba.ToArray();
            var chunk = createEmptyChunk(b.Length, false);
            chunk.Data = b;
            return chunk;
        }

        public override void ParseFromRaw(ChunkRaw c)
        {
            var nullsFound = 0;
            var nullsIdx = new int[3];
            for (var k = 0; k < c.Data.Length; k++)
            {
                if (c.Data[k] != 0)
                {
                    continue;
                }

                nullsIdx[nullsFound] = k;
                nullsFound++;
                if (nullsFound == 1)
                {
                    k += 2;
                }

                if (nullsFound == 3)
                {
                    break;
                }
            }
            if (nullsFound != 3)
            {
                throw new PngjException("Bad formed PngChunkITXT chunk");
            }

            key = ChunkHelper.ToString(c.Data, 0, nullsIdx[0]);
            var i = nullsIdx[0] + 1;
            compressed = c.Data[i] == 0 ? false : true;
            i++;
            if (compressed && c.Data[i] != 0)
            {
                throw new PngjException("Bad formed PngChunkITXT chunk - bad compression method ");
            }

            langTag = ChunkHelper.ToString(c.Data, i, nullsIdx[1] - i);
            translatedTag = ChunkHelper.ToStringUTF8(c.Data, nullsIdx[1] + 1, nullsIdx[2] - nullsIdx[1] - 1);
            i = nullsIdx[2] + 1;
            if (compressed)
            {
                var bytes = ChunkHelper.compressBytes(c.Data, i, c.Data.Length - i, false);
                val = ChunkHelper.ToStringUTF8(bytes);
            }
            else
            {
                val = ChunkHelper.ToStringUTF8(c.Data, i, c.Data.Length - i);
            }
        }

        public override void CloneDataFromRead(PngChunk other)
        {
            var otherx = (PngChunkITXT)other;
            key = otherx.key;
            val = otherx.val;
            compressed = otherx.compressed;
            langTag = otherx.langTag;
            translatedTag = otherx.translatedTag;
        }

        public bool IsCompressed()
        {
            return compressed;
        }

        public void SetCompressed(bool compressed)
        {
            this.compressed = compressed;
        }

        public string GetLangtag()
        {
            return langTag;
        }

        public void SetLangtag(string langtag)
        {
            langTag = langtag;
        }

        public string GetTranslatedTag()
        {
            return translatedTag;
        }

        public void SetTranslatedTag(string translatedTag)
        {
            this.translatedTag = translatedTag;
        }
    }
}