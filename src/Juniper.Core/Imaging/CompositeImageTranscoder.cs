
using Juniper.Progress;

namespace Juniper.Imaging
{
    public class CompositeImageTranscoder<ImageTranscoderTypeA, ImageTypeA, ImageTranscoderTypeB, ImageTypeB>
        : IImageTranscoder<ImageTypeA, ImageTypeB>
        where ImageTranscoderTypeA : IImageTranscoder<ImageData, ImageTypeA>
        where ImageTranscoderTypeB : IImageTranscoder<ImageData, ImageTypeB>
    {
        private readonly ImageTranscoderTypeA transcoderA;
        private readonly ImageTranscoderTypeB transcoderB;

        public CompositeImageTranscoder(ImageTranscoderTypeA a, ImageTranscoderTypeB b)
        {
            transcoderA = a;
            transcoderB = b;
        }

        public ImageTypeB TranslateTo(ImageTypeA value, IProgress prog)
        {
            var subProgs = prog.Split(2);
            var image = transcoderA.TranslateFrom(value, subProgs[0]);
            return transcoderB.TranslateTo(image, subProgs[1]);
        }

        public ImageTypeA TranslateFrom(ImageTypeB value, IProgress prog)
        {
            var subProgs = prog.Split(2);
            var image = transcoderB.TranslateFrom(value, subProgs[0]);
            return transcoderA.TranslateTo(image, subProgs[1]);
        }
    }
}
