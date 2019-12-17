namespace Hjg.Pngcs.Chunks
{
    /// <summary>
    /// A Chunk type that does not allow duplicate in an image
    /// </summary>
    public abstract class PngChunkSingle : PngChunk
    {
        protected PngChunkSingle(string id, ImageInfo imgInfo)
            : base(id, imgInfo)
        {
        }

        public sealed override bool AllowsMultiple()
        {
            return false;
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            return prime + (Id?.GetHashCode() ?? 0);
        }

        public override bool Equals(object obj)
        {
            return (obj is PngChunkSingle && Id != null && Id.Equals(((PngChunkSingle)obj).Id));
        }
    }
}