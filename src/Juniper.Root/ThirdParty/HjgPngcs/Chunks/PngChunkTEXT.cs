using System;

namespace Hjg.Pngcs.Chunks
{
    /// <summary>
    /// tEXt chunk: latin1 uncompressed text
    /// </summary>
    public class PngChunkTEXT : AbstractPngChunkTextVar
    {
        public const string ID = ChunkHelper.tEXt;

        public PngChunkTEXT(ImageInfo info)
            : base(ID, info)
        {
        }

        public override ChunkRaw CreateRawChunk()
        {
            if (Key.Length == 0)
            {
                throw new PngjException("Text chunk key must be non empty");
            }

            var b1 = Hjg.Pngcs.PngHelperInternal.charsetLatin1.GetBytes(Key);
            var b2 = Hjg.Pngcs.PngHelperInternal.charsetLatin1.GetBytes(Val);
            var chunk = CreateEmptyChunk(b1.Length + b2.Length + 1, true);
            Array.Copy(b1, 0, chunk.Data, 0, b1.Length);
            chunk.Data[b1.Length] = 0;
            Array.Copy(b2, 0, chunk.Data, b1.Length + 1, b2.Length);
            return chunk;
        }

        public override void ParseFromRaw(ChunkRaw c)
        {
            int i;
            for (i = 0; i < c.Data.Length; i++)
            {
                if (c.Data[i] == 0)
                {
                    break;
                }
            }

            Key = Hjg.Pngcs.PngHelperInternal.charsetLatin1.GetString(c.Data, 0, i);
            i++;
            Val = i < c.Data.Length ? Hjg.Pngcs.PngHelperInternal.charsetLatin1.GetString(c.Data, i, c.Data.Length - i) : "";
        }

        public override void CloneDataFromRead(AbstractPngChunk other)
        {
            var otherx = (PngChunkTEXT)other;
            Key = otherx.Key;
            Val = otherx.Val;
        }
    }
}