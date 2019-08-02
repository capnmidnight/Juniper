using System.IO;

using NAudio.Wave;

using NLayer.NAudioSupport;

namespace Juniper.Audio.MP3
{
    public class Decoder : AbstractDecoder
    {
        /// <summary>
        /// Decodes MP3 files into a raw stream of PCM bytes.
        /// </summary>
        /// <param name="audioStream"></param>
        /// <returns></returns>
        protected override WaveStream MakeDecodingStream(Stream stream)
        {
            return new Mp3FileReader(stream, wf => new Mp3FrameDecompressor(wf));
        }
    }
}