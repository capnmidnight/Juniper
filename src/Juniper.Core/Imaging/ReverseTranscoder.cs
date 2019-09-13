using Juniper.Progress;

namespace Juniper.Imaging
{
    public class ReverseTranscoder<ImageTypeA, ImageTypeB> : IImageTranscoder<ImageTypeA, ImageTypeB>
    {
        private readonly IImageTranscoder<ImageTypeB, ImageTypeA> transcoder;

        public ReverseTranscoder(IImageTranscoder<ImageTypeB, ImageTypeA> transcoder)
        {
            this.transcoder = transcoder;
        }

        /// <summary>
        /// Decodes a raw file buffer of PNG data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="imageStream">Png bytes.</param>
        public ImageTypeA TranslateFrom(ImageTypeB rows, IProgress prog)
        {
            return transcoder.TranslateTo(rows, prog);
        }

        /// <summary>
        /// Encodes a raw file buffer of image data into a PNG image.
        /// </summary>
        /// <param name="outputStream">Png bytes.</param>
        public ImageTypeB TranslateTo(ImageTypeA image, IProgress prog)
        {
            return transcoder.TranslateFrom(image, prog);
        }
    }
}
