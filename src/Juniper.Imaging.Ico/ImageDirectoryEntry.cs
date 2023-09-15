using System.Runtime.InteropServices;

namespace Juniper.Imaging.Ico;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ImageDirectoryEntry
{
    private byte width; // 0 means 256
    private byte height; // 0 means 256
    public byte NumColorsInPalette; // 0 means no palette
    private readonly byte reserved; // must always be zero
    public ushort ColorPlanes; // 0 or 1
    public ushort BitCount;
    public uint ImageDataLength;
    public uint ImageDataOffset;

    public int Width
    {
        readonly get
        {
            if (width == 0)
            {
                return 256;
            }

            return width;
        }

        set
        {
            if (value >= 256)
            {
                width = 0;
                return;
            }

            width = (byte)value;
        }
    }

    public int Height
    {
        readonly get
        {
            if (height == 0)
            {
                return 256;
            }

            return height;
        }

        set
        {
            if (value >= 256)
            {
                height = 0;
                return;
            }

            height = (byte)value;
        }
    }

    public int BitsPerPixel
    {
        readonly get => ColorPlanes * BitCount;
        set => BitCount = (ushort)(value / ColorPlanes);
    }

    public int BytesPerPixel
    {
        readonly get => BitsPerPixel >> 3;
        set => BitsPerPixel = value << 3;
    }
}
