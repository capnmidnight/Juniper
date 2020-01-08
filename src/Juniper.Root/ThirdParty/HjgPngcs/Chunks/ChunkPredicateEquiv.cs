namespace Hjg.Pngcs.Chunks
{
    /// <summary>
    /// An ad-hoc criterion, perhaps useful, for equivalence.
    /// <see cref="ChunkHelper.Equivalent(AbstractPngChunk,AbstractPngChunk)"/>
    /// </summary>
    internal class ChunkPredicateEquiv : IChunkPredicate
    {
        private readonly AbstractPngChunk chunk;

        /// <summary>
        /// Creates predicate based of reference chunk
        /// </summary>
        /// <param name="chunk"></param>
        public ChunkPredicateEquiv(AbstractPngChunk chunk)
        {
            this.chunk = chunk;
        }

        /// <summary>
        /// Check for match
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool Matches(AbstractPngChunk c)
        {
            return ChunkHelper.Equivalent(c, chunk);
        }
    }
}