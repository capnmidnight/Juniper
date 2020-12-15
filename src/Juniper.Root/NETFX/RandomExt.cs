using System.Numerics;

namespace System
{
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

        private const string DEFAULT_CHAR_SET = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXTZ";
        public static char NextChar(this Random rand, string charSet)
        {
            if (charSet is null)
            {
                charSet = DEFAULT_CHAR_SET;
            }

            if(charSet.Length == 0)
            {
                throw new ArgumentException(nameof(charSet), "Character set for random selection must not be the empty string");
            }

            var idx = rand.Next(0, charSet.Length);
            return charSet[idx];
        }
    }
}