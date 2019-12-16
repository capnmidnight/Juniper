using System.IO;

using Juniper.Progress;

namespace Juniper.Imaging
{
    public class TranscoderCodec<FromImageT, ToImageT>
        : IImageCodec<ToImageT>
    {
        private IImageCodec<FromImageT> codec;
        private IImageTranscoder<FromImageT, ToImageT> transcoder;

        public TranscoderCodec(IImageCodec<FromImageT> codec, IImageTranscoder<FromImageT, ToImageT> transcoder)
        {
            this.codec = codec;
            this.transcoder = transcoder;
        }

        public MediaType.Image ContentType
        {
            get
            {
                return codec.ContentType;
            }
        }

        public ToImageT Deserialize(Stream stream, IProgress prog)
        {
            var progs = prog.Split("Read", "Translate");
            var img = codec.Deserialize(stream, progs[0]);
            return transcoder.Translate(img, progs[1]);
        }

        public void Serialize(Stream stream, ToImageT value, IProgress prog = null)
        {
            var progs = prog.Split("Translate", "Write");
            var img = transcoder.Translate(value, progs[0]);
            codec.Serialize(stream, img, progs[1]);
        }
    }
}
