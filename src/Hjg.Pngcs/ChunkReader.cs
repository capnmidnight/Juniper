using Hjg.Pngcs;
using Hjg.Pngcs.Chunks;

namespace Ar.Com.Hjg.Pngcs
{
    internal class ChunkReader
    {
        protected ChunkReaderMode mode;

        protected int read = 0;

        public ChunkReader(int clen, string id, long offsetInPng, ChunkReaderMode mode)
        {
            if (mode == ChunkReaderMode.NONE || id.Length != 4 || clen < 0)
                throw new PngjExceptionInternal("Bad chunk paramenters: " + mode);
            this.mode = mode;
            new ChunkRaw(clen, id, mode == ChunkReaderMode.BUFFER);
        }
    }
}