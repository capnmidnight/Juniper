// Ignore Spelling: Ico

using System;
using System.Runtime.InteropServices;

namespace Juniper.Imaging.Ico;

public static partial class Ico
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IcoHeader
    {
        private readonly ushort reserved; // must always be zero
        public ushort Type; // 1 for ICO, 2 for CUR
        public ushort NumImages;

        public IcoHeader(IcoType type, int numImages)
        {
            if (numImages < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numImages), "Number of images must be greater than 0.");
            }

            if (numImages > ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(numImages), $"Number of images must be no more than {ushort.MaxValue}");
            }

            Type = (ushort)type;
            NumImages = (ushort)numImages;
        }
    }
}