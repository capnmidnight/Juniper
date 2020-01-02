using System.Collections.Generic;
using System.Diagnostics;

namespace BitMiracle.LibJpeg
{
    internal class RawImage : IRawImage
    {
        private readonly List<SampleRow> m_samples;

        private int m_currentRow = -1;

        internal RawImage(List<SampleRow> samples, Colorspace colorspace)
        {
            Debug.Assert(samples is object);
            Debug.Assert(samples.Count > 0);
            Debug.Assert(colorspace != Colorspace.Unknown);

            m_samples = samples;
            Colorspace = colorspace;
        }

        public int Width
        {
            get { return m_samples[0].Length; }
        }

        public int Height
        {
            get { return m_samples.Count; }
        }

        public Colorspace Colorspace { get; }

        public int ComponentsPerPixel
        {
            get { return m_samples[0][0].ComponentCount; }
        }

        public void BeginRead()
        {
            m_currentRow = 0;
        }

        public byte[] GetPixelRow()
        {
            var row = m_samples[m_currentRow];
            var result = new List<byte>();
            for (var i = 0; i < row.Length; ++i)
            {
                var sample = row[i];
                for (var j = 0; j < sample.ComponentCount; ++j)
                {
                    result.Add((byte)sample[j]);
                }
            }

            ++m_currentRow;
            return result.ToArray();
        }

        public void EndRead()
        {
        }
    }
}
