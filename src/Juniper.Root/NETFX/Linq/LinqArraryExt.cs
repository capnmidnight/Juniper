using System.Collections.Generic;

namespace System.Linq
{
    public static class LinqArraryExt
    {
        public static IEnumerable<T> Where<T>(this T[,] arr, Func<T, bool> predicate)
        {
            if (arr is null)
            {
                throw new ArgumentNullException(nameof(arr));
            }

            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var height = arr.GetLength(0);
            var width = arr.GetLength(1);
            for (var y = 0; y < height; ++y)
            {
                for (var x = 0; x < width; ++x)
                {
                    if (predicate(arr[y, x]))
                    {
                        yield return arr[y, x];
                    }
                }
            }
        }
    }
}
