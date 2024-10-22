using System.Globalization;
using System.IO;

namespace Hjg.Pngcs.Chunks
{
    /// <summary>
    /// Wraps the raw chunk data
    /// </summary>
    /// <remarks>
    /// <para>
    /// Short lived object, to be created while
    /// serialing/deserializing
    /// </para>
    /// <para>Do not reuse it for different chunks</para>
    /// <para>See http://www.libpng.org/pub/png/spec/1.2/PNG-Chunks.html</para>
    ///</remarks>
    public class ChunkRaw
    {
        /// <summary>
        /// The length counts only the data field, not itself, the chunk type code, or the CRC. Zero is a valid length.
        /// Although encoders and decoders should treat the length as unsigned, its value must not exceed 2^31-1 bytes.
        /// </summary>
        public int Len { get; }

        /// <summary>
        /// Chunk Id, as array of 4 bytes
        /// </summary>
        public byte[] IdBytes { get; }

        public string Id { get; }

        /// <summary>
        /// Raw data, crc not included
        /// </summary>
        public byte[] Data { get; set; }

        private int crcval;

        /// <summary>
        /// Creates an empty raw chunk
        /// </summary>
        internal ChunkRaw(int length, string idb, bool alloc)
        {
            Id = idb;
            IdBytes = ChunkHelper.ToBytes(Id);
            Data = null;
            crcval = 0;
            Len = length;
            if (alloc)
            {
                AllocData();
            }
        }

        internal ChunkRaw(int length, byte[] idbytes, bool alloc) : this(length, ChunkHelper.ToString(idbytes), alloc)
        {
        }

        /// <summary>
        /// Called after setting data, before writing to os
        /// </summary>
        private int ComputeCrc()
        {
            var crcengine = PngHelperInternal.GetCRC();
            crcengine.Reset();
            crcengine.Update(IdBytes, 0, 4);
            if (Len > 0)
            {
                crcengine.Update(Data, 0, Len); //
            }

            return (int)crcengine.GetValue();
        }

        internal void WriteChunk(Stream os)
        {
            if (IdBytes.Length != 4)
            {
                throw new PngjOutputException("bad chunkid [" + ChunkHelper.ToString(IdBytes) + "]");
            }

            crcval = ComputeCrc();
            PngHelperInternal.WriteInt4(os, Len);
            PngHelperInternal.WriteBytes(os, IdBytes);
            if (Len > 0)
            {
                PngHelperInternal.WriteBytes(os, Data, 0, Len);
            }
            //Console.WriteLine("writing chunk " + this.ToString() + "crc=" + crcval);
            PngHelperInternal.WriteInt4(os, crcval);
        }

        /// <summary>
        /// Position before: just after chunk id. positon after: after crc Data should
        /// be already allocated. Checks CRC Return number of byte read.
        /// </summary>
        ///
        internal int ReadChunkData(Stream stream, bool checkCrc)
        {
            PngHelperInternal.ReadBytes(stream, Data, 0, Len);
            crcval = PngHelperInternal.ReadInt4(stream);
            if (checkCrc)
            {
                var crc = ComputeCrc();
                if (crc != crcval)
                {
                    throw new PngjBadCrcException("crc invalid for chunk " + ToString() + " calc="
                            + crc.ToString(CultureInfo.CurrentCulture) + " read=" + crcval.ToString(CultureInfo.CurrentCulture));
                }
            }

            return Len + 4;
        }

        internal MemoryStream GetAsByteStream()
        {
            // only the data
            return new MemoryStream(Data);
        }

        private void AllocData()
        {
            if (Data is null || Data.Length < Len)
            {
                Data = new byte[Len];
            }
        }

        /// <summary>
        /// Just id and length
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "chunkid=" + ChunkHelper.ToString(IdBytes) + " len=" + Len.ToString(CultureInfo.InvariantCulture);
        }
    }
}