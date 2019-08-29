using System;
using System.IO;

using Juniper.HTTP;
using Juniper.Progress;

namespace Juniper.Imaging
{
    public abstract class AbstractImageDataTranscoder<SubDecoderT, SubDecoderImageT>
        : IImageCodec<ImageData>, IImageTranscoder<ImageData, SubDecoderImageT>
        where SubDecoderT : IImageCodec<SubDecoderImageT>
    {
        protected readonly SubDecoderT subCodec;

        protected AbstractImageDataTranscoder(SubDecoderT subCodec)
        {
            this.subCodec = subCodec;
        }

        public MediaType ContentType { get { return subCodec.ContentType; } }

        public ImageInfo GetImageInfo(byte[] data)
        {
            return subCodec.GetImageInfo(data);
        }

        public int GetWidth(ImageData img)
        {
            return img.info.dimensions.width;
        }

        public int GetHeight(ImageData img)
        {
            return img.info.dimensions.height;
        }

        public int GetComponents(ImageData img)
        {
            return img.info.components;
        }

        public ImageData Concatenate(ImageData[,] images, IProgress prog = null)
        {
            this.ValidateImages(images, prog,
                out var rows, out var columns, out var components,
                out var tileWidth,
                out var tileHeight);

            var stride = columns * tileWidth * components;

            var combined = new ImageData(
                columns * tileWidth,
                rows * tileHeight,
                components);

            for (var i = 0; i < combined.data.Length; i += stride)
            {
                var bufferX = i % combined.info.stride;
                var bufferY = i / combined.info.stride;
                var tileX = bufferX / stride;
                var tileY = bufferY / tileHeight;
                var tile = images[tileY, tileX];
                if (tile != null)
                {
                    var imageY = bufferY % tileHeight;
                    var imageI = imageY * stride;
                    Array.Copy(tile.data, imageI, combined.data, i, stride);
                }

                prog?.Report(i, combined.data.Length);
            }

            return combined;
        }

        public void Serialize(Stream stream, ImageData value, IProgress prog = null)
        {
            var subProgs = prog.Split("Translate image", "Write image");
            var subImage = TranslateTo(value, subProgs[0]);
            try
            {
                subCodec.Serialize(stream, subImage, subProgs[1]);
            }
            finally
            {
                if(subImage is IDisposable d)
                {
                    d.Dispose();
                }
            }
        }

        public ImageData Deserialize(Stream stream)
        {
            var subImage = subCodec.Deserialize(stream);
            try
            {
                return TranslateFrom(subImage);
            }
            finally
            {
                if (subImage is IDisposable d)
                {
                    d.Dispose();
                }
            }
        }

        public abstract SubDecoderImageT TranslateTo(ImageData value, IProgress prog = null);

        public abstract ImageData TranslateFrom(SubDecoderImageT image, IProgress prog = null);
    }
}