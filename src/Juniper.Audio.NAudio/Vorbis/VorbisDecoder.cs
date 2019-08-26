using System.IO;

using NAudio.Vorbis;
using NAudio.Wave;

namespace Juniper.Audio.Vorbis
{
    public class VorbisDecoder : AbstractDecoder
    {
        public VorbisDecoder()
            : base(AudioFormat.Vorbis) { }

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