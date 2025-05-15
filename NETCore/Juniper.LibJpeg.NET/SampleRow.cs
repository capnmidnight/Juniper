using System;

namespace BitMiracle.LibJpeg
{
    /// <summary>
    /// Represents a row of image - collection of samples.
    /// </summary>
    public class SampleRow
    {
        private readonly byte[] m_bytes;
        private readonly Sample[] m_samples;

        /// <summary>
        /// Creates a row from raw samples data.
        /// </summary>
        /// <param name="row">Raw description of samples.<br/>
        /// You can pass collection with more than sampleCount samples - only sampleCount samples
        /// will be parsed and all remaining bytes will be ignored.</param>
        /// <param name="sampleCount">The number of samples in row.</param>
        /// <param name="bitsPerComponent">The number of bits per component.</param>
        /// <param name="componentsPerSample">The number of components per sample.</param>
        public SampleRow(byte[] row, int sampleCount, byte bitsPerComponent, byte componentsPerSample)
        {
            if (row is null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            if (row.Length == 0)
            {
                throw new ArgumentException("row is empty");
            }

            if (sampleCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sampleCount));
            }

            if (bitsPerComponent <= 0 || bitsPerComponent > 16)
            {
                throw new ArgumentOutOfRangeException(nameof(bitsPerComponent));
            }

            if (componentsPerSample <= 0 || componentsPerSample > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(componentsPerSample));
            }

            m_bytes = row;

            using var bitStream = new BitStream(row);
            m_samples = new Sample[sampleCount];
            for (var i = 0; i < sampleCount; ++i)
            {
                m_samples[i] = new Sample(bitStream, bitsPerComponent, componentsPerSample);
            }
        }

        /// <summary>
        /// Creates row from an array of components.
        /// </summary>
        /// <param name="sampleComponents">Array of color components.</param>
        /// <param name="bitsPerComponent">The number of bits per component.</param>
        /// <param name="componentsPerSample">The number of components per sample.</param>
        /// <remarks>The difference between this constructor and
        /// <see cref="BitMiracle.LibJpeg.SampleRow.#ctor(System.Byte[],System.Int32,System.Byte,System.Byte)">another one</see> -
        /// this constructor accept an array of prepared color components whereas
        /// another constructor accept raw bytes and parse them.
        /// </remarks>
        internal SampleRow(short[] sampleComponents, byte bitsPerComponent, byte componentsPerSample)
        {
            if (sampleComponents is null)
            {
                throw new ArgumentNullException(nameof(sampleComponents));
            }

            if (sampleComponents.Length == 0)
            {
                throw new ArgumentException("row is empty");
            }

            if (bitsPerComponent <= 0 || bitsPerComponent > 16)
            {
                throw new ArgumentOutOfRangeException(nameof(bitsPerComponent));
            }

            if (componentsPerSample <= 0 || componentsPerSample > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(componentsPerSample));
            }

            var sampleCount = sampleComponents.Length / componentsPerSample;
            m_samples = new Sample[sampleCount];
            for (var i = 0; i < sampleCount; ++i)
            {
                var components = new short[componentsPerSample];
                Buffer.BlockCopy(sampleComponents, i * componentsPerSample * sizeof(short), components, 0, componentsPerSample * sizeof(short));
                m_samples[i] = new Sample(components, bitsPerComponent);
            }

            using var bits = new BitStream();
            for (var i = 0; i < sampleCount; ++i)
            {
                for (var j = 0; j < componentsPerSample; ++j)
                {
                    bits.Write(sampleComponents[(i * componentsPerSample) + j], bitsPerComponent);
                }
            }

            m_bytes = new byte[bits.UnderlyingStream.Length];
            bits.UnderlyingStream.Seek(0, System.IO.SeekOrigin.Begin);
            bits.UnderlyingStream.Read(m_bytes, 0, (int)bits.UnderlyingStream.Length);
        }

        /// <summary>
        /// Gets the number of samples in this row.
        /// </summary>
        /// <value>The number of samples.</value>
        public int Length => m_samples.Length;

        /// <summary>
        /// Gets the sample at the specified index.
        /// </summary>
        /// <param name="sampleNumber">The number of sample.</param>
        /// <returns>The required sample.</returns>
        public Sample this[int sampleNumber]
        {
            get
            {
                return GetAt(sampleNumber);
            }
        }

        /// <summary>
        /// Gets the sample at the specified index.
        /// </summary>
        /// <param name="sampleNumber">The number of sample.</param>
        /// <returns>The required sample.</returns>
        public Sample GetAt(int sampleNumber)
        {
            return m_samples[sampleNumber];
        }

        /// <summary>
        /// Serializes this row to raw bytes.
        /// </summary>
        /// <returns>The row representation as array of bytes</returns>
        public byte[] ToBytes()
        {
            return m_bytes;
        }
    }
}
