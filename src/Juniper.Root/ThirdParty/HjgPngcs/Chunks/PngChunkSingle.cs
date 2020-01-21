using System;
using System.Collections.Generic;

namespace Hjg.Pngcs.Chunks
{
    /// <summary>
    /// A Chunk type that does not allow duplicate in an image
    /// </summary>
    public abstract class PngChunkSingle : AbstractPngChunk
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
            return prime +  EqualityComparer<string>.Default.GetHashCode(Id);
        }

        public override bool Equals(object obj)
        {
            return obj is PngChunkSingle pngChunkSingle
                && Id?.Equals(pngChunkSingle.Id, System.StringComparison.Ordinal) == true;
        }
    }
}