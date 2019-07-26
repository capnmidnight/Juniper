using System.IO;

namespace Juniper.Image
{
    public interface IDecoder
    {
        /// <summary>
        /// Decodes a raw file buffer of  data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="imageStream">Jpeg bytes.</param>
        RawImage Decode(Stream imageStream, bool flipImage);
    }
}