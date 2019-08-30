using System.Collections;
using System.Collections.Generic;

namespace System.Threading.Tasks
{
    public static class EnumeratorTaskExt
    {
        public static async Task AsTask(this IEnumerator iter)
        {
            while (iter.MoveNext())
            {
                await Task.Yield();
            }
        }

        public static Task<IEnumerable<T>> AsTask<T>(this IEnumerator<T> iter)
        {
            return Task.Run(() =>
            {
                var output = new List<T>();
                while (iter.MoveNext())
                {
                    output.Add(iter.Current);
                }

                return (IEnumerable<T>)output;
            });
        }
    }
}