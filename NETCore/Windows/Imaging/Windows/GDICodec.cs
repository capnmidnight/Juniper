using GDIImageFormat = System.Drawing.Imaging.ImageFormat;

namespace Juniper.Imaging;

public class GDICodec : IImageFactory<Image>
{
    private readonly GDIImageFormat gdiFormat;

    public MediaType.Image InputContentType { get; }
    public MediaType.Image OutputContentType => InputContentType;


    public GDICodec(MediaType.Image format)
    {
        InputContentType = format ?? throw new ArgumentNullException(nameof(format));
        gdiFormat = format.ToGDIImageFormat();
    }

    public Image Deserialize(Stream stream)
    {
        if (stream is null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        return Image.FromStream(stream);
    }

    public long Serialize(Stream stream, Image value)
    {
        if (stream is null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        value.Save(stream, gdiFormat);
        return stream.Length;
    }
}