using System.IO;

using NAudio.Wave;

using NLayer.NAudioSupport;

namespace Juniper.Audio.MP3
{
    public class Mp3Decoder : AbstractDecoder
    {
        public Mp3Decoder()
            : base(AudioFormat.MP3) { }

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