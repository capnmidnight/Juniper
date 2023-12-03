using System.Numerics;

namespace System;

public static class RandomExt
{
    public static int Color(this Random rand)
    {
        if (rand is null)
        {
            throw new ArgumentNullException(nameof(rand));
        }

        var r = rand.Next(0, 256);
        var g = rand.Next(0, 256);
        var b = rand.Next(0, 256);
        return (r << (2 * Juniper.Units.Bits.PER_BYTE)) | (g << Juniper.Units.Bits.PER_BYTE) | b;
    }
}