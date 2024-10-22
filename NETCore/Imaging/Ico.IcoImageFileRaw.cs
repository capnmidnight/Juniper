// Ignore Spelling: Ico

using System.Runtime.InteropServices;

namespace Juniper.Imaging.Ico;

public static partial class Ico
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IcoImageFileRaw
    {
        public IcoHeader Header;
        public IcoImageDirectoryEntry[] ImageDirectory;
        public byte[][] ImageData;
    }
}