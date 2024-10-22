namespace Hjg.Pngcs.Chunks;

/// <summary>
/// hIST chunk, see http://www.w3.org/TR/PNG/#11hIST
/// Only for palette images
/// </summary>
public class PngChunkHIST : PngChunkSingle
{
    public static readonly string ID = ChunkHelper.hIST;

    private int[] hist = Array.Empty<int>(); // should have same lenght as palette

    public PngChunkHIST(ImageInfo info)
        : base(ID, info) { }

    public override ChunkOrderingConstraint GetOrderingConstraint()
    {
        return ChunkOrderingConstraint.AFTER_PLTE_BEFORE_IDAT;
    }

    public override ChunkRaw CreateRawChunk()
    {
        if (!ImgInfo.Indexed)
        {
            throw new PngjException("only indexed images accept a HIST chunk");
        }

        var c = CreateEmptyChunk(hist.Length * 2, true);
        for (var i = 0; i < hist.Length; i++)
        {
            PngHelperInternal.WriteInt2tobytes(hist[i], c.Data, i * 2);
        }

        return c;
    }

    public override void ParseFromRaw(ChunkRaw c)
    {
        if (c is null)
        {
            throw new ArgumentNullException(nameof(c));
        }

        if (!ImgInfo.Indexed)
        {
            throw new PngjException("only indexed images accept a HIST chunk");
        }

        var nentries = c.Data.Length / 2;
        hist = new int[nentries];
        for (var i = 0; i < hist.Length; i++)
        {
            hist[i] = PngHelperInternal.ReadInt2fromBytes(c.Data, i * 2);
        }
    }

    public override void CloneDataFromRead(AbstractPngChunk other)
    {
        CloneData((PngChunkHIST)other);
    }

    private void CloneData(PngChunkHIST other)
    {
        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        hist = new int[other.hist.Length];
        Array.Copy(other.hist, 0, hist, 0, other.hist.Length);
    }

    public int[] GetHist()
    {
        return hist;
    }

    /// <summary>
    /// should have same length as palette
    /// </summary>
    /// <param name="hist"></param>
    public void SetHist(int[] hist)
    {
        this.hist = hist;
    }
}