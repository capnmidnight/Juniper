namespace System;

public static class IntegerExt
{
    public static sbyte Midpoint(this sbyte a, sbyte b)
    {
        return (sbyte)((sbyte)(a | b) - (sbyte)((sbyte)(a ^ b) >> 1));
    }

    public static byte Midpoint(this byte a, byte b)
    {
        return (byte)((byte)(a | b) - (byte)((byte)(a ^ b) >> 1));
    }

    public static short Midpoint(this short a, short b)
    {
        return (short)((short)(a | b) - (short)((short)(a ^ b) >> 1));
    }

    public static ushort Midpoint(this ushort a, ushort b)
    {
        return (ushort)((ushort)(a | b) - (ushort)((ushort)(a ^ b) >> 1));
    }

    public static int Midpoint(this int a, int b)
    {
        return ((a ^ b) >> 1) + (a & b);
    }

    public static uint Midpoint(this uint a, uint b)
    {
        return ((a ^ b) >> 1) + (a & b);
    }

    public static long Midpoint(this long a, long b)
    {
        return ((a ^ b) >> 1) + (a & b);
    }

    public static ulong Midpoint(this ulong a, ulong b)
    {
        return ((a ^ b) >> 1) + (a & b);
    }
}
