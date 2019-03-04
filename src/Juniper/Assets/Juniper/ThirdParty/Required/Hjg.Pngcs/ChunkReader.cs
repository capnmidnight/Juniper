using Hjg.Pngcs;
using Hjg.Pngcs.Chunks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ar.Com.Hjg.Pngcs
{
    class ChunkReader
    {
        protected ChunkReaderMode mode;

	    protected int read = 0;

        public ChunkReader(int clen, String id, long offsetInPng, ChunkReaderMode mode)
        {
            if (mode == ChunkReaderMode.NONE || id.Length != 4 || clen < 0)
                throw new PngjExceptionInternal("Bad chunk paramenters: " + mode);
            this.mode = mode;
            new ChunkRaw(clen, id, mode == ChunkReaderMode.BUFFER);
        }
    }
}
