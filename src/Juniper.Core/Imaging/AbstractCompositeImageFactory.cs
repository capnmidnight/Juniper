using System.IO;

using Juniper.Progress;

namespace Juniper.Imaging
{
    public abstract class AbstractCompositeImageFactory<FromImageType, ToImageType>
        : IImageTranscoder<FromImageType, ToImageType>
    {
        private readonly IImageCodec<FromImageType> codec;

        public AbstractCompositeImageFactory(IImageCodec<FromImageType> codec)
        {
            this.codec = codec;
        }

        public MediaType.Image ContentType { get { return codec.ContentType; } }

        public ToImageType Concatenate(ToImageType[,] images, IProgress prog)
        {
            throw new System.NotImplementedException();
        }

        public int GetComponents(ToImageType img)
        {
            var transImage = Translate(img, null);
            return codec.GetComponents(transImage);
        }

        public int GetHeight(ToImageType img)
        {
            var transImage = Translate(img, null);
            return codec.GetHeight(transImage);
        }

        public ImageInfo GetImageInfo(byte[] data)
        {
            return codec.GetImageInfo(data);
        }

        public int GetWidth(ToImageType img)
        {
            var transImage = Translate(img, null);
            return codec.GetWidth(transImage);
        }

        public ToImageType Deserialize(Stream stream, IProgress prog)
        {
            var subProgs = prog.Split("Read", "Translate");
            var img = codec.Deserialize(stream, subProgs[0]);
            return Translate(img, subProgs[1]);
        }

        public void Serialize(Stream stream, ToImageType image, IProgress prog)
        {
            var subProgs = prog.Split("Translate", "Write");
            var transImage = Translate(image, subProgs[0]);
            codec.Serialize(stream, transImage, subProgs[1]);
        }

        public abstract FromImageType Translate(ToImageType image, IProgress prog);

        public abstract ToImageType Translate(FromImageType image, IProgress prog);
    }
}