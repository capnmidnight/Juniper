namespace Hjg.Pngcs.Chunks
{
    /// <summary>
    /// Match if have same Chunk Id
    /// </summary>
    internal class ChunkPredicateId : IChunkPredicate
    {
        private readonly string id;

        public ChunkPredicateId(string id)
        {
            this.id = id;
        }

        public bool Matches(AbstractPngChunk c)
        {
            return c.Id.Equals(id, System.StringComparison.Ordinal);
        }
    }
}