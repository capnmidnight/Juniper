namespace Hjg.Pngcs.Chunks;

/// <summary>
/// match if have same id and, if Text (or SPLT) if have the asame key
/// </summary>
/// <remarks>
/// This is the same as ChunkPredicateEquivalent, the only difference is that does not requires
/// a chunk at construction time
/// </remarks>
internal class ChunkPredicateId2 : IChunkPredicate
{
    private readonly string id;
    private readonly string innerid;

    public ChunkPredicateId2(string id, string inner)
    {
        this.id = id;
        innerid = inner;
    }

    public bool Matches(AbstractPngChunk c)
    {
        if (!c.Id.Equals(id, System.StringComparison.Ordinal))
        {
            return false;
        }

        if (c is AbstractPngChunkTextVar pngChunkTextVar
            && !pngChunkTextVar.Key.Equals(innerid, System.StringComparison.Ordinal))
        {
            return false;
        }

        if (c is PngChunkSPLT pngChunkSPLT
            && !pngChunkSPLT.PalName.Equals(innerid, System.StringComparison.Ordinal))
        {
            return false;
        }

        return true;
    }
}