using System.IO;

using Juniper.HTTP;
using Juniper.Progress;
using Juniper.Serialization;

namespace Juniper.Imaging
{
    public class CompositeImageFactory<ImageTypeA, ImageTypeB> : IFactory<ImageTypeB>
    {
        private readonly IImageCodec<ImageTypeA> codec;
        private readonly IImageTranscoder<ImageTypeA, ImageTypeB> transcoder;

        public CompositeImageFactory(IImageCodec<ImageTypeA> codec, IImageTranscoder<ImageTypeA, ImageTypeB> transcoder)
        {
            this.codec = codec;
            this.transcoder = transcoder;
        }

        public MediaType ContentType { get { return codec.ContentType; } }

        public ImageTypeB Deserialize(Stream stream, IProgress prog)
        {
            var subProgs = prog.Split(2);
            var img = codec.Deserialize(stream, subProgs[0]);
            return transcoder.TranslateTo(img, subProgs[1]);
        }

        public void Serialize(Stream stream, ImageTypeB value, IProgress prog)
        {
            var subProgs = prog.Split(2);
            var img = transcoder.TranslateFrom(value, subProgs[0]);
            codec.Serialize(stream, img, subProgs[1]);
        }
    }
}