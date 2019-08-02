using System.IO;

using NAudio.Wave;

namespace Juniper.Audio.WAV
{
    public class Decoder : AbstractDecoder
    {
        /// <summary>
        /// Decodes WAV files into a raw stream of PCM bytes.
        /// </summary>
        /// <param name="audioStream"></param>
        /// <returns></returns>
        protected override WaveStream MakeDecodingStream(Stream stream)
        {
            return new WaveFileReader(stream);
        }
    }
}