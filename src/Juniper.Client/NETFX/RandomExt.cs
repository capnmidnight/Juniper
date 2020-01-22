namespace System
{
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

        public static bool Coin(this Random rand, double weight = 0.5)
        {
            if (rand is null)
            {
                throw new ArgumentNullException(nameof(rand));
            }

            return rand.NextDouble() < weight;
        }
    }
}