using Juniper.HTTP;

using UnityEngine;

namespace Juniper.Imaging.Unity
{
    public class UnityTextureImageDataTranscoder : ReverseTranscoder<Texture2D, ImageData>
    {
        public UnityTextureImageDataTranscoder(MediaType.Image format)
            : base(new ImageDataUnityTextureTranscoder(format))
        { }

        public UnityTextureImageDataTranscoder(Texture2D.EXRFlags flags)
            : base(new ImageDataUnityTextureTranscoder(flags))
        { }

        public UnityTextureImageDataTranscoder(int jpegQuality = 80)
            : base(new ImageDataUnityTextureTranscoder(jpegQuality))
        { }
    }
}
