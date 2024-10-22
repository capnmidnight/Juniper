using System.Numerics;

namespace System;

public static class RandomExt
{
    public static Vector3 NextVector3(this Random rand)
    {
        if (rand is null)
        {
            throw new ArgumentNullException(nameof(rand));
        }

        return new Vector3(
            (float)rand.NextDouble() * 2 - 1,
            (float)rand.NextDouble() * 2 - 1,
            (float)rand.NextDouble() * 2 - 1);
    }

    public static Quaternion NextQuaternion(this Random rand)
    {
        if (rand is null)
        {
            throw new ArgumentNullException(nameof(rand));
        }

        return Quaternion.CreateFromYawPitchRoll(
            (float)rand.NextDouble() * 2 - 1,
            (float)rand.NextDouble() * 2 - 1,
            (float)rand.NextDouble() * 2 - 1);
    }
}