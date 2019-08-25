using System;
using System.IO;
using System.Linq;

using Juniper.Progress;

namespace Juniper.Imaging
{
    public abstract class AbstractImageDataDecoder<SubDecoderT, SubDecoderImageT>
        : IImageDecoder<ImageData>, IImageTranscoder<ImageData, SubDecoderImageT>
        where SubDecoderT : IImageDecoder<SubDecoderImageT>
    {
        protected readonly SubDecoderT subCodec;

        protected AbstractImageDataDecoder(SubDecoderT subCodec)
        {
            this.subCodec = subCodec;
        }

        public HTTP.MediaType.Image Format { get { return subCodec.Format; } }

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

        public ImageData Concatenate(ImageData[,] images, IProgress prog = null)
        {
            this.ValidateImages(images, prog,
                out var rows, out var columns,
                out var tileWidth,
                out var tileHeight);

            var firstImage = images.Where(img => img != null).First();

            var combined = new ImageData(
                columns * tileWidth,
                rows * tileHeight,
                firstImage.info.components);

            for (var i = 0; i < combined.data.Length; i += firstImage.info.stride)
            {
                var bufferX = i % combined.info.stride;
                var bufferY = i / combined.info.stride;
                var tileX = bufferX / firstImage.info.stride;
                var tileY = bufferY / tileHeight;
                var tile = images[tileY, tileX];
                if (tile != null)
                {
                    var imageY = bufferY % tileHeight;
                    var imageI = imageY * firstImage.info.stride;
                    Array.Copy(tile.data, imageI, combined.data, i, firstImage.info.stride);
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

        public abstract ImageData TranslateFrom(SubDecoderImageT image);
    }
}