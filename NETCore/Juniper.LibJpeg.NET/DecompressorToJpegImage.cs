﻿using System.IO;

namespace BitMiracle.LibJpeg
{
    internal class DecompressorToJpegImage : IDecompressDestination
    {
        private readonly JpegImage m_jpegImage;

        internal DecompressorToJpegImage(JpegImage jpegImage)
        {
            m_jpegImage = jpegImage;
        }

        public Stream Output => null;

        public void SetImageAttributes(LoadedImageAttributes parameters)
        {
            m_jpegImage.Width = parameters.Width;
            m_jpegImage.Height = parameters.Height;
            m_jpegImage.BitsPerComponent = 8;
            m_jpegImage.ComponentsPerSample = (byte)parameters.ComponentsPerSample;
            m_jpegImage.Colorspace = parameters.Colorspace;
        }

        public void BeginWrite()
        {
        }

        public void ProcessPixelsRow(byte[] row)
        {
            var samplesRow = new SampleRow(row, m_jpegImage.Width, m_jpegImage.BitsPerComponent, m_jpegImage.ComponentsPerSample);
            m_jpegImage.AddSampleRow(samplesRow);
        }

        public void EndWrite()
        {
        }
    }
}
