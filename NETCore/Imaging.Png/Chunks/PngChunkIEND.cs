namespace Hjg.Pngcs.Chunks;

/// <summary>
/// IEND chunk  http://www.w3.org/TR/PNG/#11IEND
/// </summary>
public class PngChunkIEND : PngChunkSingle
{
    public const string ID = ChunkHelper.IEND;

    public PngChunkIEND(ImageInfo info)
        : base(ID, info)
    {
    }

    public override ChunkOrderingConstraint GetOrderingConstraint()
    {
        return ChunkOrderingConstraint.NA;
    }

    public override ChunkRaw CreateRawChunk()
    {
        var c = new ChunkRaw(0, ChunkHelper.b_IEND, false);
        return c;
    }

    public override void ParseFromRaw(ChunkRaw c)
    {
        // this is not used
    }

    public override void CloneDataFromRead(AbstractPngChunk other)
    {
    }
}