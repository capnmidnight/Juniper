namespace System;

public static class RandomExt
{
    public static double Number(this Random rand, float min, float max, float power)
    {
        if (rand is null)
        {
            throw new ArgumentNullException(nameof(rand));
        }

        var delta = max - min;
        var n = Math.Pow(rand.NextDouble(), power);
        return min + (n * delta);
    }

    public static double Number(this Random rand, float min, float max)
    {
        return rand.Number(min, max, 1);
    }

    public static double Number(this Random rand, float max)
    {
        return rand.Number(0, max);
    }

    public static int Steps(this Random rand, int min, int max, int steps)
    {
        if (rand is null)
        {
            throw new ArgumentNullException(nameof(rand));
        }

        return min + (rand.Next(0, (1 + max - min) / steps) * steps);
    }

    public static T Item<T>(this Random rand, T[] arr)
    {
        if (rand is null)
        {
            throw new ArgumentNullException(nameof(rand));
        }

        if (arr is null)
        {
            throw new ArgumentNullException(nameof(arr));
        }

        if (arr.Length == 0)
        {
            throw new ArgumentException("Array must have at least one element", nameof(arr));
        }

        return arr[rand.Next(0, arr.Length)];
    }

    public static bool Coin(this Random rand, double weight = 0.5)
    {
        if (rand is null)
        {
            throw new ArgumentNullException(nameof(rand));
        }

        return rand.NextDouble() < weight;
    }

    private const string DEFAULT_CHAR_SET = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXTZ";
    public static char NextChar(this Random rand, string? charSet = null)
    {
        charSet ??= DEFAULT_CHAR_SET;

        if (charSet.Length == 0)
        {
            throw new ArgumentException("Character set for random selection must not be the empty string", nameof(charSet));
        }

        var idx = rand.Next(0, charSet.Length);
        return charSet[idx];
    }

    /// <summary>
    /// Generates a random string of a specified length. The default character set is Arabic numberals and upper-case Latin alphabet letters, as used in English.
    /// </summary>
    /// <param name="length"></param>
    /// <param name="charSet"></param>
    /// <returns></returns>
    public static string NextString(this Random rand, uint length, string? charSet = null)
    {
        var str = "";
        for (var i = 0; i < length; ++i)
        {
            str += rand.NextChar(charSet);
        }

        return str;
    }
}