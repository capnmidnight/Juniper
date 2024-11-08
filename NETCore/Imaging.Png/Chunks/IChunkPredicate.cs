namespace Hjg.Pngcs.Chunks;

/// <summary>
/// Decides if another chunk "matches", according to some criterion
/// </summary>
public interface IChunkPredicate
{
    /// <summary>
    /// The other chunk matches with this one
    /// </summary>
    /// <param name="chunk">The other chunk</param>
    /// <returns>true if matches</returns>
    bool Matches(AbstractPngChunk chunk);
}