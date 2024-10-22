// Ignore Spelling: Codec

using System.Collections;

namespace Juniper.Imaging;

public class ImageCodecSet : IDictionary<MediaType, IImageFactory<ImageData>>
{
    public static readonly ImageCodecSet Default = new()
    {
        [MediaType.Image_Png] = new PngFactory().Pipe(new PngCodec()),
        [MediaType.Image_Jpeg] = new JpegFactory().Pipe(new JpegCodec(padAlpha: true))
    };

    private readonly Dictionary<MediaType, IImageFactory<ImageData>> decoders;

    public ImageCodecSet()
    {
        decoders = new Dictionary<MediaType, IImageFactory<ImageData>>();
    }

    public ImageCodecSet(int capacity)
    {
        decoders = new Dictionary<MediaType, IImageFactory<ImageData>>(capacity);
    }

    public ImageCodecSet(IEqualityComparer<MediaType> comparer)
    {
        decoders = new Dictionary<MediaType, IImageFactory<ImageData>>(comparer);
    }

    public ImageCodecSet(IDictionary<MediaType, IImageFactory<ImageData>> dictionary)
    {
        decoders = new Dictionary<MediaType, IImageFactory<ImageData>>(dictionary);
    }

    public ImageCodecSet(int capacity, IEqualityComparer<MediaType> comparer)
    {
        decoders = new Dictionary<MediaType, IImageFactory<ImageData>>(capacity, comparer);
    }

    public ImageCodecSet(IDictionary<MediaType, IImageFactory<ImageData>> dictionary, IEqualityComparer<MediaType> comparer)
    {
        decoders = new Dictionary<MediaType, IImageFactory<ImageData>>(dictionary, comparer);
    }

    public IImageFactory<ImageData> this[MediaType mediaType]
    {
        get => decoders[mediaType];

        set => decoders[mediaType] = value;
    }

    public IImageFactory<ImageData> this[string mediaTypeName]
    {
        get => this[MediaType.Lookup(mediaTypeName)];

        set => this[MediaType.Lookup(mediaTypeName)] = value;
    }

    public ICollection<MediaType> Keys => decoders.Keys;

    public ICollection<IImageFactory<ImageData>> Values => decoders.Values;

    public int Count => decoders.Count;

    public bool IsReadOnly => ((IDictionary<MediaType, IImageFactory<ImageData>>)decoders).IsReadOnly;

    public void Add(MediaType key, IImageFactory<ImageData> value)
    {
        decoders.Add(key, value);
    }

    public void Add(KeyValuePair<MediaType, IImageFactory<ImageData>> item)
    {
        ((IDictionary<MediaType, IImageFactory<ImageData>>)decoders).Add(item);
    }

    public void Clear()
    {
        decoders.Clear();
    }

    public bool Contains(KeyValuePair<MediaType, IImageFactory<ImageData>> item)
    {
        return ((IDictionary<MediaType, IImageFactory<ImageData>>)decoders).Contains(item);
    }

    public bool ContainsKey(MediaType key)
    {
        return decoders.ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<MediaType, IImageFactory<ImageData>>[] array, int arrayIndex)
    {
        ((IDictionary<MediaType, IImageFactory<ImageData>>)decoders).CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<MediaType, IImageFactory<ImageData>>> GetEnumerator()
    {
        return decoders.GetEnumerator();
    }

    public bool Remove(MediaType key)
    {
        return decoders.Remove(key);
    }

    public bool Remove(KeyValuePair<MediaType, IImageFactory<ImageData>> item)
    {
        return ((IDictionary<MediaType, IImageFactory<ImageData>>)decoders).Remove(item);
    }

    public bool TryGetValue(MediaType key, out IImageFactory<ImageData> value)
    {
        return decoders.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return decoders.GetEnumerator();
    }


    public ImageData? LoadImage(string fileName, Func<string, Stream> getStream)
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

        if (type is null || !decoders.ContainsKey(type))
        {
            throw new NotSupportedException($"Don't know how to decode image type {type}");
        }

        using var stream = getStream(fileName);
        return decoders[type]?.Deserialize(stream);
    }
}
