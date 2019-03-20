using System;
using System.Collections.Generic;
using System.Linq;

namespace Juniper.Progress
{
    public interface IProgress : IProgress<float>
    {
        float Progress
        {
            get;
        }

        void Report(float progress, string status);
    }

    public static class IProgressExt
    {
        const float ALPHA = 1e-3f;
        public static bool IsComplete(this IProgress prog)
        {
            return Math.Abs(prog.Progress - 1) < ALPHA;
        }

        public static void ForEach<T>(this IProgress prog, IEnumerable<T> arr, Action<T, IProgress> act, Action<Exception> error = null)
        {
            prog?.Report(0);

            var len = arr.Count();
            var progs = prog.Split(len);
            var index = 0;
            foreach (var item in arr)
            {
                try
                {
                    progs[index]?.Report(0);
                    act(item, progs[index]);
                    progs[index]?.Report(1);
                    ++index;
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception exp)
                {
                    error?.Invoke(exp);
                }
            }

            prog?.Report(1);
        }

        public static IProgress Subdivide(this IProgress parent, float start, float length, string prefix = null)
        {
            return new ProgressSubdivision(parent, start, length, prefix);
        }

        public static IProgress Subdivide(this IProgress parent, int index, int count, string prefix = null)
        {
            return new ProgressSubdivision(parent, (float)index / count, 1f / count, prefix);
        }

        public static IProgress[] Split(this IProgress parent, long numParts, string prefix = null)
        {
            var arr = new IProgress[numParts];
            var length = 1.0f / numParts;
            for (var i = 0; i < numParts; ++i)
            {
                arr[i] = parent?.Subdivide(i * length, length, prefix);
            }

            return arr;
        }

        public static void Report(this IProgress prog, float count, float length, string status = null)
        {
            if (length > 0)
            {
                prog?.Report(count / length, status);
            }
            else
            {
                prog?.Report(1, status);
            }
        }
    }
}
