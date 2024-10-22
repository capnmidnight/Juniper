namespace Hjg.Pngcs.Chunks;

/// <summary>
/// Unknown (for our chunk factory) chunk type.
/// </summary>
public class PngChunkUNKNOWN : PngChunkMultiple
{
    private byte[] data;

    public PngChunkUNKNOWN(string id, ImageInfo info)
        : base(id, info)
    {
    }

    public override ChunkOrderingConstraint GetOrderingConstraint()
    {
        return ChunkOrderingConstraint.NONE;
    }

    public override ChunkRaw CreateRawChunk()
    {
        var p = CreateEmptyChunk(data.Length, false);
        p.Data = data;
        return p;
    }

    public override void ParseFromRaw(ChunkRaw c)
    {
        if (c is null)
        {
            throw new System.ArgumentNullException(nameof(c));
        }

        data = c.Data;
    }

    /* does not copy! */

    public byte[] GetData()
    {
        return data;
    }

    /* does not copy! */

    public void SetData(byte[] data_0)
    {
        data = data_0;
    }

    public override void CloneDataFromRead(AbstractPngChunk other)
    {
        // THIS SHOULD NOT BE CALLED IF ALREADY CLONED WITH COPY CONSTRUCTOR
        data = CloneData((PngChunkUNKNOWN)other); // not deep copy
    }

    private static byte[] CloneData(PngChunkUNKNOWN other)
    {
        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        return other.data;
    }
}