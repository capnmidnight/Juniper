#if !NETSTANDARD
using System;
using System.Drawing;
using System.IO;

using Juniper.Progress;

using GDIImageFormat = System.Drawing.Imaging.ImageFormat;

namespace Juniper.Imaging
{
    public class GDICodec : IImageCodec<Image>
    {
        private readonly GDIImageFormat gdiFormat;

        public MediaType.Image ContentType { get; }

        public GDICodec(MediaType.Image format)
        {
            ContentType = format ?? throw new ArgumentNullException(nameof(format));
            gdiFormat = format.ToGDIImageFormat();
        }

        public void Serialize(Stream stream, Image value, IProgress prog = null)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            prog.Report(0);
            value.Save(stream, gdiFormat);
            prog.Report(1);
        }

        public Image Deserialize(Stream stream, IProgress prog = null)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            prog.Report(0);
            var image = Image.FromStream(stream);
            prog.Report(1);
            return image;
        }
    }
}
#endif