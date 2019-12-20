using System.Numerics;

namespace System
{
    public static class RandomExt
    {
        public static double Number(this Random rand, float min, float max, float power)
        {
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
            return min + (rand.Next(0, (1 + max - min) / steps) * steps);
        }

        public static T Item<T>(this Random rand, T[] arr)
        {
            return arr[rand.Next(0, arr.Length)];
        }

        public static int Color(this Random rand)
        {
            var r = rand.Next(0, 256);
            var g = rand.Next(0, 256);
            var b = rand.Next(0, 256);
            return (r << 16) | (g << 8) | b;
        }

        public static bool Coin(this Random rand, double weight = 0.5)
        {
            return rand.NextDouble() < weight;
        }
    }
}