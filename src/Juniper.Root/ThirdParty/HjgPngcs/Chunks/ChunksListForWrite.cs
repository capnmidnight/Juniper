using System.Collections.Generic;
using System.IO;

using Hjg.Pngcs;

namespace Hjg.Pngcs.Chunks
{
    /// <summary>
    /// Chunks written or queued to be written
    /// http://www.w3.org/TR/PNG/#table53
    /// </summary>
    ///
    public class ChunksListForWrite : ChunksList
    {
        private readonly List<PngChunk> queuedChunks; // chunks not yet writen - does not include IHDR, IDAT, END, perhaps yes PLTE

        // redundant, just for eficciency
        private readonly Dictionary<string, int> alreadyWrittenKeys;

        internal ChunksListForWrite(ImageInfo info)
            : base(info)
        {
            queuedChunks = new List<PngChunk>();
            alreadyWrittenKeys = new Dictionary<string, int>();
        }

        /// <summary>
        /// Same as <c>getById()</c>, but looking in the queued chunks
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<PngChunk> GetQueuedById(string id)
        {
            return GetQueuedById(id, null);
        }

        /// <summary>
        /// Same as <c>getById()</c>, but looking in the queued chunks
        /// </summary>
        /// <param name="id"></param>
        /// <param name="innerid"></param>
        /// <returns></returns>
        public List<PngChunk> GetQueuedById(string id, string innerid)
        {
            return GetXById(queuedChunks, id, innerid);
        }

        /// <summary>
        /// Same as <c>getById()</c>, but looking in the queued chunks
        /// </summary>
        /// <param name="id"></param>
        /// <param name="innerid"></param>
        /// <param name="failIfMultiple"></param>
        /// <returns></returns>
        public PngChunk GetQueuedById1(string id, string innerid, bool failIfMultiple)
        {
            var list = GetQueuedById(id, innerid);
            if (list.Count == 0)
            {
                return null;
            }

            if (list.Count > 1 && (failIfMultiple || !list[0].AllowsMultiple()))
            {
                throw new PngjException("unexpected multiple chunks id=" + id);
            }

            return list[list.Count - 1];
        }

        /// <summary>
        /// Same as <c>getById1()</c>, but looking in the queued chunks
        /// </summary>
        /// <param name="id"></param>
        /// <param name="failIfMultiple"></param>
        /// <returns></returns>
        public PngChunk GetQueuedById1(string id, bool failIfMultiple)
        {
            return GetQueuedById1(id, null, failIfMultiple);
        }

        /// <summary>
        /// Same as getById1(), but looking in the queued chunks
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PngChunk GetQueuedById1(string id)
        {
            return GetQueuedById1(id, false);
        }

        /// <summary>
        ///Remove Chunk: only from queued
        /// </summary>
        /// <remarks>
        /// WARNING: this depends on chunk.Equals() implementation, which is straightforward for SingleChunks. For
        /// MultipleChunks, it will normally check for reference equality!
        /// </remarks>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool RemoveChunk(PngChunk c)
        {
            return queuedChunks.Remove(c);
        }

        /// <summary>
        /// Adds chunk to queue
        /// </summary>
        /// <remarks>Does not check for duplicated or anything</remarks>
        /// <param name="chunk"></param>
        /// <returns></returns>
        public bool Queue(PngChunk chunk)
        {
            queuedChunks.Add(chunk);
            return true;
        }

        /**
         * this should be called only for ancillary chunks and PLTE (groups 1 - 3 - 5)
         **/

        private static bool ShouldWrite(PngChunk c, int currentGroup)
        {
            if (currentGroup == CHUNK_GROUP_2_PLTE)
            {
                return c.Id.Equals(ChunkHelper.PLTE, System.StringComparison.Ordinal);
            }

            if (currentGroup % 2 == 0)
            {
                throw new PngjOutputException("bad chunk group?");
            }

            int minChunkGroup;
            int maxChunkGroup;
            if (c.MustGoBeforePLTE())
            {
                minChunkGroup = maxChunkGroup = ChunksList.CHUNK_GROUP_1_AFTERIDHR;
            }
            else if (c.MustGoBeforeIDAT())
            {
                maxChunkGroup = ChunksList.CHUNK_GROUP_3_AFTERPLTE;
                minChunkGroup = c.MustGoAfterPLTE() ? ChunksList.CHUNK_GROUP_3_AFTERPLTE
                        : ChunksList.CHUNK_GROUP_1_AFTERIDHR;
            }
            else
            {
                maxChunkGroup = ChunksList.CHUNK_GROUP_5_AFTERIDAT;
                minChunkGroup = ChunksList.CHUNK_GROUP_1_AFTERIDHR;
            }

            var preferred = maxChunkGroup;
            if (c.Priority)
            {
                preferred = minChunkGroup;
            }

            if (ChunkHelper.IsUnknown(c) && c.ChunkGroup > 0)
            {
                preferred = c.ChunkGroup;
            }

            if (currentGroup == preferred)
            {
                return true;
            }

            if (currentGroup > preferred && currentGroup <= maxChunkGroup)
            {
                return true;
            }

            return false;
        }

        internal int WriteChunks(Stream os, int currentGroup)
        {
            var written = new List<int>();
            for (var i = 0; i < queuedChunks.Count; i++)
            {
                var c = queuedChunks[i];
                if (!ShouldWrite(c, currentGroup))
                {
                    continue;
                }

                if (ChunkHelper.IsCritical(c.Id)
                    && !c.Id.Equals(ChunkHelper.PLTE, System.StringComparison.Ordinal))
                {
                    throw new PngjOutputException("bad chunk queued: " + c);
                }

                if (alreadyWrittenKeys.ContainsKey(c.Id) && !c.AllowsMultiple())
                {
                    throw new PngjOutputException("duplicated chunk does not allow multiple: " + c);
                }

                c.Write(os);
                pngChunks.Add(c);
                alreadyWrittenKeys[c.Id] = alreadyWrittenKeys.ContainsKey(c.Id) ? alreadyWrittenKeys[c.Id] + 1 : 1;
                written.Add(i);
                c.ChunkGroup = currentGroup;
            }

            for (var k = written.Count - 1; k >= 0; k--)
            {

                queuedChunks.RemoveAt(written[k]);
            }

            return written.Count;
        }

        /// <summary>
        /// chunks not yet writen - does not include IHDR, IDAT, END, perhaps yes PLTE
        /// </summary>
        /// <returns>THis is not a copy! Don't modify</returns>
        internal List<PngChunk> GetQueuedChunks()
        {
            return queuedChunks;
        }
    }
}