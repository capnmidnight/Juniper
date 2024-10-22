namespace Hjg.Pngcs.Chunks;

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

        var b1 = PngHelperInternal.charsetLatin1.GetBytes(Key);
        var b2 = PngHelperInternal.charsetLatin1.GetBytes(Val);
        var chunk = CreateEmptyChunk(b1.Length + b2.Length + 1, true);
        Array.Copy(b1, 0, chunk.Data, 0, b1.Length);
        chunk.Data[b1.Length] = 0;
        Array.Copy(b2, 0, chunk.Data, b1.Length + 1, b2.Length);
        return chunk;
    }

    public override void ParseFromRaw(ChunkRaw c)
    {
        if (c is null)
        {
            throw new ArgumentNullException(nameof(c));
        }

        int i;
        for (i = 0; i < c.Data.Length; i++)
        {
            if (c.Data[i] == 0)
            {
                break;
            }
        }

        Key = PngHelperInternal.charsetLatin1.GetString(c.Data, 0, i);
        i++;
        Val = i < c.Data.Length ? PngHelperInternal.charsetLatin1.GetString(c.Data, i, c.Data.Length - i) : "";
    }

    public override void CloneDataFromRead(AbstractPngChunk other)
    {
        CloneData((PngChunkTEXT)other);
    }

    private void CloneData(PngChunkTEXT other)
    {
        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        Key = other.Key;
        Val = other.Val;
    }
}