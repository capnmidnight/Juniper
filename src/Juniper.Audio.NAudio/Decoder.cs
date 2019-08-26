using System;
using System.IO;

using Juniper.Serialization;

namespace Juniper.Audio
{
    public class Decoder : IDeserializer<AudioData>
    {
        private IDeserializer<AudioData> decoder;
        private AudioFormat format;

        public Decoder(AudioFormat format)
        {
            Format = format;
        }

        public AudioFormat Format
        {
            get { return format; }
            set
            {
                if (value == AudioFormat.MP3)
                {
                    format = value;
                    decoder = new MP3.Mp3Decoder();
                }
                else if (value == AudioFormat.WAV)
                {
                    format = value;
                    decoder = new WAV.WavDecoder();
                }
                else if (value == AudioFormat.Vorbis)
                {
                    format = value;
                    decoder = new Vorbis.VorbisDecoder();
                }
                else
                {
                    throw new ArgumentException($"Audio format `{format}` has not been implemented yet.");
                }
            }
        }

        public AudioData Deserialize(Stream imageStream)
        {
            return decoder.Deserialize(imageStream);
        }
    }
}