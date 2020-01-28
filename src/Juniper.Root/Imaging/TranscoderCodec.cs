using System;
using System.IO;

namespace Juniper.Imaging
{
    public class TranscoderCodec<FromImageT, ToImageT>
        : IImageCodec<ToImageT>
    {
        private readonly IImageCodec<FromImageT> codec;
        private readonly IImageTranscoder<FromImageT, ToImageT> transcoder;

        public TranscoderCodec(IImageCodec<FromImageT> codec, IImageTranscoder<FromImageT, ToImageT> transcoder)
        {
            this.codec = codec;
            this.transcoder = transcoder;
        }

        public MediaType.Image ContentType => codec.ContentType;

        public ToImageT Deserialize(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var img = codec.Deserialize(stream);
            return transcoder.Translate(img);
        }

        public long Serialize(Stream stream, ToImageT value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var img = transcoder.Translate(value);
            return codec.Serialize(stream, img);
        }
    }
}
