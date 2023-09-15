// Ignore Spelling: Ico

using System.Runtime.InteropServices;

namespace Juniper.Imaging.Ico;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct IcoImageFileRaw
{
    public Header Header;
    public ImageDirectoryEntry[] ImageDirectory;
    public byte[][] ImageData;
}
