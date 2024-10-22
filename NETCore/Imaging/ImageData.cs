namespace Juniper.Imaging;

/// <summary>
/// The raw bytes and dimensions of an image that has been loaded either off disk or across the 'net.
/// </summary>
public partial class ImageData
{
    public static T?[,] CubeCross<T>(T[] images)
    where T : class
    {
        if (images is null)
        {
            throw new ArgumentNullException(nameof(images));
        }

        return new T?[,]
        {
            { default, images[0], default, default },
            { images[1], images[2], images[3], images[4] },
            { default, images[5], default, default }
        };
    }

    public ImageInfo Info { get; }

    private readonly byte[] _data;

    public byte[] GetData()
    {
        return (byte[])_data.Clone();
    }

    public Color3 GetRGB(int i)
    {
        return new Color3(_data.GetRGB(i), ColorSpace.RGB);
    }

    public Color3 GetRGB(int x, int y)
    {
        var i = y * Info.Stride + x * Info.Components;
        return GetRGB(i);
    }

    public void SetRGB(int i, Color3 rgb)
    {
        _data.SetRGB(i, rgb);
    }

    public void SetRGB(int x, int y, Color3 rgb)
    {
        if (rgb.Space != ColorSpace.RGB)
        {
            rgb = rgb.ConvertTo(ColorSpace.RGB);
        }

        var i = y * Info.Stride + x * Info.Components;
        SetRGB(i, rgb);
    }

    public ImageData(ImageInfo info, byte[] data)
    {
        Info = info ?? throw new ArgumentNullException(nameof(info));
        _data = data ?? throw new ArgumentNullException(nameof(data));
    }

    public ImageData(Size size, int components, byte[] data)
        : this(new ImageInfo(size, components), data) { }

    public ImageData(int width, int height, int components, byte[] data)
        : this(new Size(width, height), components, data)
    {
    }

    public ImageData(Size size, int components)
        : this(size ?? throw new ArgumentNullException(nameof(size)), components, new byte[size.Height * size.Width * components])
    {
    }

    public ImageData(int width, int height, int components)
        : this(new Size(width, height), components)
    {
    }
}