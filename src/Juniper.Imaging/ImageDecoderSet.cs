using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Juniper.Imaging
{
    public class ImageDecoderSet : IDictionary<MediaType, IImageCodec<ImageData>>
    {
        public static readonly ImageDecoderSet Default = new ImageDecoderSet()
        {
            [MediaType.Image.Png] = new PngCodec().Pipe(new PngTranscoder()),
            [MediaType.Image.Jpeg] = new JpegCodec().Pipe(new JpegTranscoder(padAlpha: true))
        };

        private readonly Dictionary<MediaType, IImageCodec<ImageData>> decoders;

        public ImageDecoderSet()
        {
            decoders = new Dictionary<MediaType, IImageCodec<ImageData>>();
        }

        public ImageDecoderSet(int capacity)
        {
            decoders = new Dictionary<MediaType, IImageCodec<ImageData>>(capacity);
        }

        public ImageDecoderSet(IEqualityComparer<MediaType> comparer)
        {
            decoders = new Dictionary<MediaType, IImageCodec<ImageData>>(comparer);
        }

        public ImageDecoderSet(IDictionary<MediaType, IImageCodec<ImageData>> dictionary)
        {
            decoders = new Dictionary<MediaType, IImageCodec<ImageData>>(dictionary);
        }

        public ImageDecoderSet(int capacity, IEqualityComparer<MediaType> comparer)
        {
            decoders = new Dictionary<MediaType, IImageCodec<ImageData>>(capacity, comparer);
        }

        public ImageDecoderSet(IDictionary<MediaType, IImageCodec<ImageData>> dictionary, IEqualityComparer<MediaType> comparer)
        {
            decoders = new Dictionary<MediaType, IImageCodec<ImageData>>(dictionary, comparer);
        }

        public IImageCodec<ImageData> this[MediaType mediaType]
        {
            get => decoders[mediaType];

            set => decoders[mediaType] = value;
        }

        public IImageCodec<ImageData> this[string mediaTypeName]
        {
            get => this[MediaType.Lookup(mediaTypeName)];

            set => this[MediaType.Lookup(mediaTypeName)] = value;
        }

        public ICollection<MediaType> Keys => decoders.Keys;

        public ICollection<IImageCodec<ImageData>> Values => decoders.Values;

        public int Count => decoders.Count;

        public bool IsReadOnly => ((IDictionary<MediaType, IImageCodec<ImageData>>)decoders).IsReadOnly;

        public void Add(MediaType key, IImageCodec<ImageData> value)
        {
            decoders.Add(key, value);
        }

        public void Add(KeyValuePair<MediaType, IImageCodec<ImageData>> item)
        {
            ((IDictionary<MediaType, IImageCodec<ImageData>>)decoders).Add(item);
        }

        public void Clear()
        {
            decoders.Clear();
        }

        public bool Contains(KeyValuePair<MediaType, IImageCodec<ImageData>> item)
        {
            return ((IDictionary<MediaType, IImageCodec<ImageData>>)decoders).Contains(item);
        }

        public bool ContainsKey(MediaType key)
        {
            return decoders.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<MediaType, IImageCodec<ImageData>>[] array, int arrayIndex)
        {
            ((IDictionary<MediaType, IImageCodec<ImageData>>)decoders).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<MediaType, IImageCodec<ImageData>>> GetEnumerator()
        {
            return decoders.GetEnumerator();
        }

        public bool Remove(MediaType key)
        {
            return decoders.Remove(key);
        }

        public bool Remove(KeyValuePair<MediaType, IImageCodec<ImageData>> item)
        {
            return ((IDictionary<MediaType, IImageCodec<ImageData>>)decoders).Remove(item);
        }

        public bool TryGetValue(MediaType key, out IImageCodec<ImageData> value)
        {
            return decoders.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return decoders.GetEnumerator();
        }


        public ImageData LoadImage(string fileName, Func<string, Stream> getStream)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("Must provide an image name.", nameof(fileName));
            }

            if (getStream is null)
            {
                throw new ArgumentNullException(nameof(getStream));
            }

            var type = (from t in MediaType.GuessByFileName(fileName)
                              where t is MediaType.Image
                              select t)
                    .FirstOrDefault();

            if (!decoders.ContainsKey(type))
            {
                throw new NotSupportedException($"Don't know how to decode image type {type}");
            }

            using var stream = getStream(fileName);
            return decoders[type].Deserialize(stream);
        }
    }
}
