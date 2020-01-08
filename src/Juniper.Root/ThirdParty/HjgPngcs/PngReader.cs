using System;
using System.Collections.Generic;
using System.IO;

using Hjg.Pngcs.Chunks;
using Hjg.Pngcs.Zlib;

namespace Hjg.Pngcs
{
    /// <summary>
    /// Reads a PNG image, line by line
    /// </summary>
    /// <remarks>
    /// <para>The typical reading sequence is as follows:</para>
    /// <para>1. At construction time, the header and IHDR chunk are read (basic image info)</para>
    /// <para>2  (Optional) you can set some global options: UnpackedMode CrcCheckDisabled</para>
    /// <para>3. (Optional) If you call GetMetadata() or or GetChunksLisk() before reading the pixels, the chunks before IDAT are automatically loaded and available</para>
    /// <para>
    /// 4a. The rows are read, one by one, with the <tt>ReadRowXXX</tt> methods: (ReadRowInt() , ReadRowByte(), etc)
    /// in order, from 0 to nrows-1 (you can skip or repeat rows, but not go backwards)
    /// </para>
    /// <para>
    /// 4b. Alternatively, you can read all rows, or a subset, in a single call: see ReadRowsInt(), ReadRowsByte()
    /// In general this consumes more memory, but for interlaced images this is equally efficient, and more so if reading a small subset of rows.
    /// </para>
    /// <para>5. Read of the last row automatically loads the trailing chunks, and ends the reader.</para>
    /// <para>6. End() forcibly finishes/aborts the reading and closes the stream</para>
    /// </remarks>
    public sealed class PngReader :
        IDisposable
    {
        /// <summary>
        /// Basic image info, inmutable
        /// </summary>
        public ImageInfo ImgInfo { get; }

        /// <summary>
        /// filename, or description - merely informative, can be empty
        /// </summary>
        private readonly string filename;

        /// <summary>
        /// Strategy for chunk loading. Default: LOAD_CHUNK_ALWAYS
        /// </summary>
        public ChunkLoadBehaviour ChunkLoadBehaviour { get; set; }

        /// <summary>
        /// Should close the underlying Input Stream when ends?
        /// </summary>
        public bool ShouldCloseStream { get; set; }

        /// <summary>
        /// Maximum amount of bytes from ancillary chunks to load in memory
        /// </summary>
        /// <remarks>
        ///  Default: 5MB. 0: unlimited. If exceeded, chunks will be skipped
        /// </remarks>
        public long MaxBytesMetadata { get; set; }

        /// <summary>
        /// Maximum total bytes to read from stream
        /// </summary>
        /// <remarks>
        ///  Default: 200MB. 0: Unlimited. If exceeded, an exception will be thrown
        /// </remarks>
        public long MaxTotalBytesRead { get; set; }

        /// <summary>
        /// Maximum ancillary chunk size
        /// </summary>
        /// <remarks>
        ///  Default: 2MB, 0: unlimited. Chunks exceeding this size will be skipped (nor even CRC checked)
        /// </remarks>
        public int SkipChunkMaxSize { get; set; }

        /// <summary>
        /// Ancillary chunks to skip
        /// </summary>
        /// <remarks>
        ///  Default: { "fdAT" }. chunks with these ids will be skipped (nor even CRC checked)
        /// </remarks>
        public string[] SkipChunkIds { get; set; }

        private Dictionary<string, int> skipChunkIdsSet = null; // lazily created

        /// <summary>
        /// A high level wrapper of a ChunksList : list of read chunks
        /// </summary>
        private readonly PngMetadata metadata;

        /// <summary>
        /// Read chunks
        /// </summary>
        private readonly ChunksList chunksList;

        /// <summary>
        /// buffer: last read line
        /// </summary>
        private ImageLine imgLine;

        /// <summary>
        /// raw current row, as array of bytes,counting from 1 (index 0 is reserved for filter type)
        /// </summary>
        private byte[] rowb;

        /// <summary>
        /// previuos raw row
        /// </summary>
        private byte[] rowbprev; // rowb previous

        /// <summary>
        /// raw current row, after unfiltered
        /// </summary>
        private readonly byte[] rowbfilter;

        // only set for interlaced PNG
        public bool Interlaced { get; }

        private readonly PngDeinterlacer deinterlacer;

        // this only influences the 1-2-4 bitdepth format
        private bool unpackedMode = false;

        /// <summary>
        /// number of chunk group (0-6) last read, or currently reading
        /// </summary>
        /// <remarks>see ChunksList.CHUNK_GROUP_NNN</remarks>
        public int CurrentChunkGroup { get; private set; }

        /// <summary>
        /// last read row number
        /// </summary>
        private int rowNum = -1; //

        private long offset = 0;  // offset in InputStream = bytes read
        private int bytesChunksLoaded = 0; // bytes loaded from anciallary chunks

        private readonly Stream inputStream;
        internal AZlibInputStream idatIstream;
        internal PngIDatChunkInputStream iIdatCstream;

        /// <summary>
        /// Constructs a PngReader from a Stream, with no filename information
        /// </summary>
        /// <param name="inputStream"></param>
        public PngReader(Stream inputStream)
            : this(inputStream, "[NO FILENAME AVAILABLE]")
        {
        }

        /// <summary>
        /// Constructs a PNGReader objet from a opened Stream
        /// </summary>
        /// <remarks>The constructor reads the signature and first chunk (IDHR)
        /// </remarks>
        ///
        /// <param name="inputStream"></param>
        /// <param name="filename">Optional, can be the filename or a description.</param>
        public PngReader(Stream inputStream, string filename)
        {
            this.inputStream = inputStream ?? throw new ArgumentNullException(nameof(inputStream));
            this.filename = filename ?? "";
            chunksList = new ChunksList(null);
            metadata = new PngMetadata(chunksList);
            offset = 0;
            // set default options
            CurrentChunkGroup = -1;
            ShouldCloseStream = true;
            MaxBytesMetadata = 5 * 1024 * 1024;
            MaxTotalBytesRead = 200 * 1024 * 1024; // 200MB
            SkipChunkMaxSize = 2 * 1024 * 1024;
            SkipChunkIds = new string[] { "fdAT" };
            ChunkLoadBehaviour = ChunkLoadBehaviour.LOAD_CHUNK_ALWAYS;
            // starts reading: signature
            var pngid = new byte[8];
            PngHelperInternal.ReadBytes(inputStream, pngid, 0, pngid.Length);
            offset += pngid.Length;
            if (!PngCsUtils.arraysEqual(pngid, PngHelperInternal.PNG_ID_SIGNATURE))
            {
                throw new PngjInputException("Bad PNG signature");
            }

            CurrentChunkGroup = ChunksList.CHUNK_GROUP_0_IDHR;
            // reads first chunk IDHR
            var clen = PngHelperInternal.ReadInt4(inputStream);
            offset += 4;
            if (clen != 13)
            {
                throw new Exception("IDHR chunk len != 13 ?? " + clen);
            }

            var chunkid = new byte[4];
            PngHelperInternal.ReadBytes(inputStream, chunkid, 0, 4);
            if (!PngCsUtils.arraysEqual4(chunkid, ChunkHelper.b_IHDR))
            {
                throw new PngjInputException("IHDR not found as first chunk??? ["
                        + ChunkHelper.ToString(chunkid) + "]");
            }

            offset += 4;
            var ihdr = (PngChunkIHDR)ReadChunk(chunkid, clen, false);
            var alpha = (ihdr.Colormodel & 0x04) != 0;
            var palette = (ihdr.Colormodel & 0x01) != 0;
            var grayscale = (ihdr.Colormodel == 0 || ihdr.Colormodel == 4);
            // creates ImgInfo and imgLine, and allocates buffers
            ImgInfo = new ImageInfo(ihdr.Cols, ihdr.Rows, ihdr.Bitspc, alpha, grayscale, palette);
            rowb = new byte[ImgInfo.BytesPerRow + 1];
            rowbprev = new byte[rowb.Length];
            rowbfilter = new byte[rowb.Length];
            Interlaced = ihdr.Interlaced == 1;
            deinterlacer = Interlaced ? new PngDeinterlacer(ImgInfo) : null;
            // some checks
            if (ihdr.Filmeth != 0 || ihdr.Compmeth != 0 || (ihdr.Interlaced & 0xFFFE) != 0)
            {
                throw new PngjInputException("compmethod or filtermethod or interlaced unrecognized");
            }

            if (ihdr.Colormodel < 0 || ihdr.Colormodel > 6 || ihdr.Colormodel == 1
                    || ihdr.Colormodel == 5)
            {
                throw new PngjInputException("Invalid colormodel " + ihdr.Colormodel);
            }

            if (ihdr.Bitspc != 1 && ihdr.Bitspc != 2 && ihdr.Bitspc != 4 && ihdr.Bitspc != 8
                    && ihdr.Bitspc != 16)
            {
                throw new PngjInputException("Invalid bit depth " + ihdr.Bitspc);
            }
        }

        private bool FirstChunksNotYetRead()
        {
            return CurrentChunkGroup < ChunksList.CHUNK_GROUP_1_AFTERIDHR;
        }

        /// <summary>
        /// Internally called after having read the last line.
        /// It reads extra chunks after IDAT, if present.
        /// </summary>
        private void ReadLastAndClose()
        {
            if (CurrentChunkGroup < ChunksList.CHUNK_GROUP_5_AFTERIDAT)
            {
                try
                {
                    idatIstream.Close();
                }
#pragma warning disable RCS1075 // Avoid empty catch clause that catches System.Exception.
                catch (Exception) { }
#pragma warning restore RCS1075 // Avoid empty catch clause that catches System.Exception.
                ReadLastChunks();
            }

            Close();
        }

        private void Close()
        {
            if (CurrentChunkGroup < ChunksList.CHUNK_GROUP_6_END)
            {
                // this could only happen if forced close
                try
                {
                    idatIstream.Close();
                }
#pragma warning disable RCS1075 // Avoid empty catch clause that catches System.Exception.
                catch (Exception)
#pragma warning restore RCS1075 // Avoid empty catch clause that catches System.Exception.
                {
                }

                CurrentChunkGroup = ChunksList.CHUNK_GROUP_6_END;
            }

            if (ShouldCloseStream)
            {
                inputStream.Close();
            }
        }

        private void UnfilterRow(int nbytes)
        {
            int ftn = rowbfilter[0];
            var ft = (FilterType)ftn;
            switch (ft)
            {
                case Hjg.Pngcs.FilterType.FILTER_NONE:
                UnfilterRowNone(nbytes);
                break;

                case Hjg.Pngcs.FilterType.FILTER_SUB:
                UnfilterRowSub(nbytes);
                break;

                case Hjg.Pngcs.FilterType.FILTER_UP:
                UnfilterRowUp(nbytes);
                break;

                case Hjg.Pngcs.FilterType.FILTER_AVERAGE:
                UnfilterRowAverage(nbytes);
                break;

                case Hjg.Pngcs.FilterType.FILTER_PAETH:
                UnfilterRowPaeth(nbytes);
                break;

                default:
                throw new PngjInputException("Filter type " + ftn + " not implemented");
            }
        }

        private void UnfilterRowAverage(int nbytes)
        {
            int i, j, x;
            for (j = 1 - ImgInfo.BytesPixel, i = 1; i <= nbytes; i++, j++)
            {
                x = (j > 0) ? rowb[j] : 0;
                rowb[i] = (byte)(rowbfilter[i] + ((x + (rowbprev[i] & 0xFF)) / 2));
            }
        }

        private void UnfilterRowNone(int nbytes)
        {
            for (var i = 1; i <= nbytes; i++)
            {
                rowb[i] = rowbfilter[i];
            }
        }

        private void UnfilterRowPaeth(int nbytes)
        {
            int i, j, x, y;
            for (j = 1 - ImgInfo.BytesPixel, i = 1; i <= nbytes; i++, j++)
            {
                x = (j > 0) ? rowb[j] : 0;
                y = (j > 0) ? rowbprev[j] : 0;
                rowb[i] = (byte)(rowbfilter[i] + PngHelperInternal.FilterPaethPredictor(x, rowbprev[i], y));
            }
        }

        private void UnfilterRowSub(int nbytes)
        {
            int i, j;
            for (i = 1; i <= ImgInfo.BytesPixel; i++)
            {
                rowb[i] = rowbfilter[i];
            }

            for (j = 1, i = ImgInfo.BytesPixel + 1; i <= nbytes; i++, j++)
            {
                rowb[i] = (byte)(rowbfilter[i] + rowb[j]);
            }
        }

        private void UnfilterRowUp(int nbytes)
        {
            for (var i = 1; i <= nbytes; i++)
            {
                rowb[i] = (byte)(rowbfilter[i] + rowbprev[i]);
            }
        }

        /// <summary>
        /// Reads chunks before first IDAT. Position before: after IDHR (crc included)
        /// Position after: just after the first IDAT chunk id Returns length of first
        /// IDAT chunk , -1 if not found
        /// </summary>
        ///
        private void ReadFirstChunks()
        {
            if (!FirstChunksNotYetRead())
            {
                return;
            }

            var clen = 0;
            var found = false;
            var chunkid = new byte[4]; // it's important to reallocate in each
            CurrentChunkGroup = ChunksList.CHUNK_GROUP_1_AFTERIDHR;
            while (!found)
            {
                clen = PngHelperInternal.ReadInt4(inputStream);
                offset += 4;
                if (clen < 0)
                {
                    break;
                }

                PngHelperInternal.ReadBytes(inputStream, chunkid, 0, 4);
                offset += 4;
                if (PngCsUtils.arraysEqual4(chunkid, Hjg.Pngcs.Chunks.ChunkHelper.b_IDAT))
                {
                    found = true;
                    CurrentChunkGroup = ChunksList.CHUNK_GROUP_4_IDAT;
                    // add dummy idat chunk to list
                    chunksList.AppendReadChunk(new PngChunkIDAT(ImgInfo, clen, offset - 8), CurrentChunkGroup);
                    break;
                }
                else if (PngCsUtils.arraysEqual4(chunkid, Hjg.Pngcs.Chunks.ChunkHelper.b_IEND))
                {
                    throw new PngjInputException("END chunk found before image data (IDAT) at offset=" + offset);
                }
                var chunkids = ChunkHelper.ToString(chunkid);
                if (chunkids.Equals(ChunkHelper.PLTE, System.StringComparison.Ordinal))
                {
                    CurrentChunkGroup = ChunksList.CHUNK_GROUP_2_PLTE;
                }

                ReadChunk(chunkid, clen, false);
                if (chunkids.Equals(ChunkHelper.PLTE, System.StringComparison.Ordinal))
                {
                    CurrentChunkGroup = ChunksList.CHUNK_GROUP_3_AFTERPLTE;
                }
            }

            var idatLen = found ? clen : -1;
            if (idatLen < 0)
            {
                throw new PngjInputException("first idat chunk not found!");
            }

            iIdatCstream = new PngIDatChunkInputStream(inputStream, idatLen, offset);
            idatIstream = new ZlibInputStream(iIdatCstream, true);
        }

        /// <summary>
        /// Reads (and processes ... up to a point) chunks after last IDAT.
        /// </summary>
        ///
        private void ReadLastChunks()
        {
            CurrentChunkGroup = ChunksList.CHUNK_GROUP_5_AFTERIDAT;
            // PngHelper.logdebug("idat ended? " + iIdatCstream.isEnded());
            if (!iIdatCstream.IsEnded())
            {
                iIdatCstream.ForceChunkEnd();
            }

            var clen = iIdatCstream.GetLenLastChunk();
            var chunkid = iIdatCstream.GetIdLastChunk();
            var endfound = false;
            var first = true;
            while (!endfound)
            {
                var skip = false;
                if (!first)
                {
                    clen = PngHelperInternal.ReadInt4(inputStream);
                    offset += 4;
                    if (clen < 0)
                    {
                        throw new PngjInputException("bad len " + clen);
                    }

                    PngHelperInternal.ReadBytes(inputStream, chunkid, 0, 4);
                    offset += 4;
                }

                first = false;
                if (PngCsUtils.arraysEqual4(chunkid, ChunkHelper.b_IDAT))
                {
                    skip = true; // extra dummy (empty?) idat chunk, it can happen, ignore it
                }
                else if (PngCsUtils.arraysEqual4(chunkid, ChunkHelper.b_IEND))
                {
                    CurrentChunkGroup = ChunksList.CHUNK_GROUP_6_END;
                    endfound = true;
                }
                ReadChunk(chunkid, clen, skip);
            }

            if (!endfound)
            {
                throw new PngjInputException("end chunk not found - offset=" + offset);
            }
            // PngHelper.logdebug("end chunk found ok offset=" + offset);
        }

        /// <summary>
        /// Reads chunkd from input stream, adds to ChunksList, and returns it.
        /// If it's skipped, a PngChunkSkipped object is created
        /// </summary>
        /// <returns></returns>
        private PngChunk ReadChunk(byte[] chunkid, int clen, bool skipforced)
        {
            if (clen < 0)
            {
                throw new PngjInputException("invalid chunk lenght: " + clen);
            }
            // skipChunksByIdSet is created lazyly, if fist IHDR has already been read
            if (skipChunkIdsSet is null && CurrentChunkGroup > ChunksList.CHUNK_GROUP_0_IDHR)
            {
                skipChunkIdsSet = new Dictionary<string, int>();
                if (SkipChunkIds is object)
                {
                    foreach (var id in SkipChunkIds)
                    {
                        skipChunkIdsSet.Add(id, 1);
                    }
                }
            }

            var chunkidstr = ChunkHelper.ToString(chunkid);
            var critical = ChunkHelper.IsCritical(chunkidstr);
            var skip = skipforced;
            if (MaxTotalBytesRead > 0 && clen + offset > MaxTotalBytesRead)
            {
                throw new PngjInputException("Maximum total bytes to read exceeeded: " + MaxTotalBytesRead + " offset:"
                        + offset + " clen=" + clen);
            }
            // an ancillary chunks can be skipped because of several reasons:
            if (CurrentChunkGroup > ChunksList.CHUNK_GROUP_0_IDHR && !ChunkHelper.IsCritical(chunkidstr))
            {
                skip = skip || (SkipChunkMaxSize > 0 && clen >= SkipChunkMaxSize) || skipChunkIdsSet.ContainsKey(chunkidstr)
                        || (MaxBytesMetadata > 0 && clen > MaxBytesMetadata - bytesChunksLoaded)
                        || !ChunkHelper.ShouldLoad(chunkidstr, ChunkLoadBehaviour);
            }

            PngChunk pngChunk;
            if (skip)
            {
                PngHelperInternal.SkipBytes(inputStream, clen);
                PngHelperInternal.ReadInt4(inputStream); // skip - we dont call PngHelperInternal.skipBytes(inputStream, clen + 4) for risk of overflow
                pngChunk = new PngChunkSkipped(chunkidstr, ImgInfo, clen);
            }
            else
            {
                var chunk = new ChunkRaw(clen, chunkid, true);
                _ = chunk.ReadChunkData(inputStream, critical);
                pngChunk = PngChunk.Factory(chunk, ImgInfo);
                if (!pngChunk.Crit)
                {
                    bytesChunksLoaded += chunk.Len;
                }
            }

            pngChunk.Offset = offset - 8L;
            chunksList.AppendReadChunk(pngChunk, CurrentChunkGroup);
            offset += clen + 4L;
            return pngChunk;
        }

        /// <summary>
        /// Logs/prints a warning.
        /// </summary>
        /// <remarks>
        /// The default behaviour is print to stderr, but it can be overriden.
        /// This happens rarely - most errors are fatal.
        /// </remarks>
        /// <param name="warn"></param>
        internal void LogWarn(string warn)
        {
            Console.Error.WriteLine(warn);
        }

        /// <summary>
        /// Returns the ancillary chunks available
        /// </summary>
        /// <remarks>
        /// If the rows have not yet still been read, this includes
        /// only the chunks placed before the pixels (IDAT)
        /// </remarks>
        /// <returns>ChunksList</returns>
        public ChunksList GetChunksList()
        {
            if (FirstChunksNotYetRead())
            {
                ReadFirstChunks();
            }

            return chunksList;
        }

        /// <summary>
        /// Returns the ancillary chunks available
        /// </summary>
        /// <remarks>
        /// see GetChunksList
        /// </remarks>
        /// <returns>PngMetadata</returns>
        public PngMetadata GetMetadata()
        {
            if (FirstChunksNotYetRead())
            {
                ReadFirstChunks();
            }

            return metadata;
        }

        /// <summary>
        /// reads the row using ImageLine as buffer
        /// </summary>
        ///<param name="nrow">row number - just as a check</param>
        /// <returns>the ImageLine that also is available inside this object</returns>
        public ImageLine ReadRow(int nrow)
        {
            return imgLine is null || imgLine.SampleType != ImageLine.ESampleType.BYTE ? ReadRowInt(nrow) : ReadRowByte(nrow);
        }

        public ImageLine ReadRowInt(int nrow)
        {
            if (imgLine is null)
            {
                imgLine = new ImageLine(ImgInfo, ImageLine.ESampleType.INT, unpackedMode);
            }

            if (imgLine.Rown == nrow) // already read
            {
                return imgLine;
            }

            ReadRowInt(imgLine.Scanline, nrow);
            imgLine.FilterUsed = (FilterType)rowbfilter[0];
            imgLine.Rown = nrow;
            return imgLine;
        }

        public ImageLine ReadRowByte(int nrow)
        {
            if (imgLine is null)
            {
                imgLine = new ImageLine(ImgInfo, ImageLine.ESampleType.BYTE, unpackedMode);
            }

            if (imgLine.Rown == nrow) // already read
            {
                return imgLine;
            }

            ReadRowByte(imgLine.ScanlineB, nrow);
            imgLine.FilterUsed = (FilterType)rowbfilter[0];
            imgLine.Rown = nrow;
            return imgLine;
        }

        public int[] ReadRow(int[] buffer, int nrow)
        {
            return ReadRowInt(buffer, nrow);
        }

        public int[] ReadRowInt(int[] buffer, int nrow)
        {
            if (buffer is null)
            {
                buffer = new int[unpackedMode ? ImgInfo.SamplesPerRow : ImgInfo.SamplesPerRowPacked];
            }

            if (!Interlaced)
            {
                if (nrow <= rowNum)
                {
                    throw new PngjInputException("rows must be read in increasing order: " + nrow);
                }

                var bytesread = 0;
                while (rowNum < nrow)
                {
                    bytesread = ReadRowRaw(rowNum + 1); // read rows, perhaps skipping if necessary
                }

                DecodeLastReadRowToInt(buffer, bytesread);
            }
            else
            { // interlaced
                if (deinterlacer.GetImageInt() is null)
                {
                    deinterlacer.SetImageInt(ReadRowsInt().Scanlines); // read all image and store it in deinterlacer
                }

                Array.Copy(deinterlacer.GetImageInt()[nrow], 0, buffer, 0, unpackedMode ? ImgInfo.SamplesPerRow
                        : ImgInfo.SamplesPerRowPacked);
            }

            return buffer;
        }

        public byte[] ReadRowByte(byte[] buffer, int nrow)
        {
            if (buffer is null)
            {
                buffer = new byte[unpackedMode ? ImgInfo.SamplesPerRow : ImgInfo.SamplesPerRowPacked];
            }

            if (!Interlaced)
            {
                if (nrow <= rowNum)
                {
                    throw new PngjInputException("rows must be read in increasing order: " + nrow);
                }

                var bytesread = 0;
                while (rowNum < nrow)
                {
                    bytesread = ReadRowRaw(rowNum + 1); // read rows, perhaps skipping if necessary
                }

                DecodeLastReadRowToByte(buffer, bytesread);
            }
            else
            { // interlaced
                if (deinterlacer.GetImageByte() is null)
                {
                    deinterlacer.SetImageByte(ReadRowsByte().ScanlinesB); // read all image and store it in deinterlacer
                }

                Array.Copy(deinterlacer.GetImageByte()[nrow], 0, buffer, 0, unpackedMode ? ImgInfo.SamplesPerRow
                        : ImgInfo.SamplesPerRowPacked);
            }

            return buffer;
        }

        [Obsolete("GetRow is deprecated,  use ReadRow/ReadRowInt/ReadRowByte instead.")]
        public ImageLine GetRow(int nrow)
        {
            return ReadRow(nrow);
        }

        private void DecodeLastReadRowToInt(int[] buffer, int bytesRead)
        {            // see http://www.libpng.org/pub/png/spec/1.2/PNG-DataRep.html
            if (ImgInfo.BitDepth <= 8)
            {
                for (int i = 0, j = 1; i < bytesRead; i++)
                {
                    buffer[i] = (rowb[j++]);
                }
            }
            else
            { // 16 bitspc
                for (int i = 0, j = 1; j < bytesRead; i++)
                {
                    buffer[i] = (rowb[j++] << 8) + rowb[j++];
                }
            }

            if (ImgInfo.Packed && unpackedMode)
            {
                ImageLine.UnpackInplaceInt(ImgInfo, buffer, buffer, false);
            }
        }

        private void DecodeLastReadRowToByte(byte[] buffer, int bytesRead)
        {            // see http://www.libpng.org/pub/png/spec/1.2/PNG-DataRep.html
            if (ImgInfo.BitDepth <= 8)
            {
                Array.Copy(rowb, 1, buffer, 0, bytesRead);
            }
            else
            { // 16 bitspc
                for (int i = 0, j = 1; j < bytesRead; i++, j += 2)
                {
                    buffer[i] = rowb[j]; // 16 bits in 1 byte: this discards the LSB!!!
                }
            }

            if (ImgInfo.Packed && unpackedMode)
            {
                ImageLine.UnpackInplaceByte(ImgInfo, buffer, buffer, false);
            }
        }

        public ImageLines ReadRowsInt(int rowOffset, int nRows, int rowStep)
        {
            if (nRows < 0)
            {
                nRows = (ImgInfo.Rows - rowOffset) / rowStep;
            }

            if (rowStep < 1
                || rowOffset < 0
                || (nRows * rowStep) + rowOffset > ImgInfo.Rows)
            {
                throw new PngjInputException("bad args");
            }

            var imlines = new ImageLines(ImgInfo, ImageLine.ESampleType.INT, unpackedMode, rowOffset, nRows, rowStep);
            if (!Interlaced)
            {
                for (var j = 0; j < ImgInfo.Rows; j++)
                {
                    var bytesread = ReadRowRaw(j); // read and perhaps discards
                    var mrow = imlines.ImageRowToMatrixRowStrict(j);
                    if (mrow >= 0)
                    {
                        DecodeLastReadRowToInt(imlines.Scanlines[mrow], bytesread);
                    }
                }
            }
            else
            { // and now, for something completely different (interlaced)
                var buf = new int[unpackedMode ? ImgInfo.SamplesPerRow : ImgInfo.SamplesPerRowPacked];
                for (var p = 1; p <= 7; p++)
                {
                    deinterlacer.SetPass(p);
                    for (var i = 0; i < deinterlacer.GetRows(); i++)
                    {
                        var bytesread = ReadRowRaw(i);
                        var j = deinterlacer.GetCurrRowReal();
                        var mrow = imlines.ImageRowToMatrixRowStrict(j);
                        if (mrow >= 0)
                        {
                            DecodeLastReadRowToInt(buf, bytesread);
                            deinterlacer.DeinterlaceInt(buf, imlines.Scanlines[mrow], !unpackedMode);
                        }
                    }
                }
            }

            End();
            return imlines;
        }

        public ImageLines ReadRowsInt()
        {
            return ReadRowsInt(0, ImgInfo.Rows, 1);
        }

        public ImageLines ReadRowsByte(int rowOffset, int nRows, int rowStep)
        {
            if (nRows < 0)
            {
                nRows = (ImgInfo.Rows - rowOffset) / rowStep;
            }

            if (rowStep < 1
                || rowOffset < 0
                || (nRows * rowStep) + rowOffset > ImgInfo.Rows)
            {
                throw new PngjInputException("bad args");
            }

            var imlines = new ImageLines(ImgInfo, ImageLine.ESampleType.BYTE, unpackedMode, rowOffset, nRows, rowStep);
            if (!Interlaced)
            {
                for (var j = 0; j < ImgInfo.Rows; j++)
                {
                    var bytesread = ReadRowRaw(j); // read and perhaps discards
                    var mrow = imlines.ImageRowToMatrixRowStrict(j);
                    if (mrow >= 0)
                    {
                        DecodeLastReadRowToByte(imlines.ScanlinesB[mrow], bytesread);
                    }
                }
            }
            else
            { // and now, for something completely different (interlaced)
                var buf = new byte[unpackedMode ? ImgInfo.SamplesPerRow : ImgInfo.SamplesPerRowPacked];
                for (var p = 1; p <= 7; p++)
                {
                    deinterlacer.SetPass(p);
                    for (var i = 0; i < deinterlacer.GetRows(); i++)
                    {
                        var bytesread = ReadRowRaw(i);
                        var j = deinterlacer.GetCurrRowReal();
                        var mrow = imlines.ImageRowToMatrixRowStrict(j);
                        if (mrow >= 0)
                        {
                            DecodeLastReadRowToByte(buf, bytesread);
                            deinterlacer.DeinterlaceByte(buf, imlines.ScanlinesB[mrow], !unpackedMode);
                        }
                    }
                }
            }

            End();
            return imlines;
        }

        public ImageLines ReadRowsByte()
        {
            return ReadRowsByte(0, ImgInfo.Rows, 1);
        }

        private int ReadRowRaw(int nrow)
        {
            //
            if (nrow == 0 && FirstChunksNotYetRead())
            {
                ReadFirstChunks();
            }

            if (nrow == 0 && Interlaced)
            {
                Array.Clear(rowb, 0, rowb.Length); // new subimage: reset filters: this is enough, see the swap that happens lines
            }
            // below
            var bytesRead = ImgInfo.BytesPerRow; // NOT including the filter byte
            if (Interlaced)
            {
                if (nrow < 0 || nrow > deinterlacer.GetRows() || (nrow != 0 && nrow != deinterlacer.GetCurrRowSubimg() + 1))
                {
                    throw new PngjInputException("invalid row in interlaced mode: " + nrow);
                }

                deinterlacer.SetRow(nrow);
                bytesRead = ((ImgInfo.BitspPixel * deinterlacer.GetPixelsToRead()) + 7) / 8;
                if (bytesRead < 1)
                {
                    throw new PngjInternalException("wtf??");
                }
            }
            else
            { // check for non interlaced
                if (nrow < 0 || nrow >= ImgInfo.Rows || nrow != rowNum + 1)
                {
                    throw new PngjInputException("invalid row: " + nrow);
                }
            }

            rowNum = nrow;
            // swap buffers
            var tmp = rowb;
            rowb = rowbprev;
            rowbprev = tmp;
            // loads in rowbfilter "raw" bytes, with filter
            PngHelperInternal.ReadBytes(idatIstream, rowbfilter, 0, bytesRead + 1);
            offset = iIdatCstream.GetOffset();
            if (offset < 0)
            {
                throw new PngjInternalException("bad offset ??" + offset);
            }

            if (MaxTotalBytesRead > 0 && offset >= MaxTotalBytesRead)
            {
                throw new PngjInputException("Reading IDAT: Maximum total bytes to read exceeeded: " + MaxTotalBytesRead
                        + " offset:" + offset);
            }

            rowb[0] = 0;
            UnfilterRow(bytesRead);
            rowb[0] = rowbfilter[0];
            if ((rowNum == ImgInfo.Rows - 1 && !Interlaced) || (Interlaced && deinterlacer.IsAtLastRow()))
            {
                ReadLastAndClose();
            }

            return bytesRead;
        }

        public void ReadSkippingAllRows()
        {
            if (FirstChunksNotYetRead())
            {
                ReadFirstChunks();
            }
            // we read directly from the compressed stream, we dont decompress nor chec CRC
            iIdatCstream.DisableCrcCheck();
            try
            {
                int r;
                do
                {
                    r = iIdatCstream.Read(rowbfilter, 0, rowbfilter.Length);

                } while (r >= 0);
            }
            catch (IOException e)
            {
                throw new PngjInputException("error in raw read of IDAT", e);
            }

            offset = iIdatCstream.GetOffset();
            if (offset < 0)
            {
                throw new PngjInternalException("bad offset ??" + offset);
            }

            if (MaxTotalBytesRead > 0 && offset >= MaxTotalBytesRead)
            {
                throw new PngjInputException("Reading IDAT: Maximum total bytes to read exceeeded: " + MaxTotalBytesRead
                        + " offset:" + offset);
            }

            ReadLastAndClose();
        }

        public override string ToString()
        { // basic info
            return "filename=" + filename + " " + ImgInfo.ToString();
        }

        /// <summary>
        /// Normally this does nothing, but it can be used to force a premature closing
        /// </summary>
        /// <remarks></remarks>
        public void End()
        {
            if (CurrentChunkGroup < ChunksList.CHUNK_GROUP_6_END)
            {
                Close();
            }
        }

        public bool IsInterlaced()
        {
            return Interlaced;
        }

        public void SetUnpackedMode(bool unPackedMode)
        {
            unpackedMode = unPackedMode;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    idatIstream?.Dispose();
                    iIdatCstream?.Dispose();
                    inputStream.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}