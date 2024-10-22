namespace Hjg.Pngcs.Chunks;

/// <summary>
/// Defines what to do with non critical chunks when reading
/// </summary>
public enum ChunkLoadBehaviour
{
    /// <summary>
    /// all non-critical chunks are skippped
    /// </summary>
    LOAD_CHUNK_NEVER,

    /// <summary>
    /// load chunk if 'known' (registered with the factory)
    /// </summary>
    LOAD_CHUNK_KNOWN,

    /// <summary>
    /// load chunk if 'known' or safe to copy
    /// </summary>
    LOAD_CHUNK_IF_SAFE,

    /// <summary>
    /// <para>load chunks always</para>
    /// <para> Notice that other restrictions might apply, see PngReader.SkipChunkMaxSize PngReader.SkipChunkIds</para>
    /// </summary>
    LOAD_CHUNK_ALWAYS,
}