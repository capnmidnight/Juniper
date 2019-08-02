using System.IO;

using NAudio.Vorbis;
using NAudio.Wave;

namespace Juniper.Audio.Vorbis
{
    public class Decoder : AbstractDecoder
    {
        /// <summary>
        /// Decodes OGG files into a raw stream of PCM bytes.
        /// </summary>
        /// <param name="audioStream"></param>
        /// <returns></returns>
        protected override WaveStream MakeDecodingStream(Stream stream)
        {
            return new VorbisWaveReader(stream);
        }
    }
}