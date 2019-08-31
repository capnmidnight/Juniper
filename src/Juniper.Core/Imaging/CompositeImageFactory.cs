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

        public ImageTypeB Deserialize(Stream stream)
        {
            var img = codec.Deserialize(stream);
            return transcoder.TranslateTo(img);
        }

        public void Serialize(Stream stream, ImageTypeB value, IProgress prog = null)
        {
            var img = transcoder.TranslateFrom(value);
            codec.Serialize(stream, img, prog);
        }
    }
}