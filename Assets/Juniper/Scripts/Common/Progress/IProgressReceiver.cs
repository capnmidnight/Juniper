using System;
using System.Collections.Generic;
using System.Linq;

namespace Juniper.Progress
{
    public interface IProgressReceiver : IProgress
    {
        void SetProgress(float progress, string status = null);
    }

    public static class IProgressReceiverExt
    {
        public static void ForEach<T>(this IProgressReceiver prog, IEnumerable<T> arr, Action<T, IProgressReceiver> act, Action<Exception> error = null)
        {
            prog?.SetProgress(0);

            var len = arr.Count();
            var progs = prog.Split(len);
            var index = 0;
            foreach (var item in arr)
            {
                try
                {
                    progs[index]?.SetProgress(0);
                    act(item, progs[index]);
                    progs[index]?.SetProgress(1);
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

            prog?.SetProgress(1);
        }

        public static IProgressReceiver Subdivide(this IProgressReceiver parent, float start, float length, string prefix = null) =>
            new ProgressSubdivision(parent, start, length, prefix);

        public static IProgressReceiver Subdivide(this IProgressReceiver parent, int index, int count, string prefix = null) =>
            new ProgressSubdivision(parent, (float)index / count, 1f / count, prefix);

        public static IProgressReceiver[] Split(this IProgressReceiver parent, long numParts, string prefix = null)
        {
            var arr = new IProgressReceiver[numParts];
            var length = 1.0f / numParts;
            for (var i = 0; i < numParts; ++i)
            {
                arr[i] = parent?.Subdivide(i * length, length, prefix);
            }

            return arr;
        }

        public static void SetProgress(this IProgressReceiver prog, float count, float length, string status = null)
        {
            if (length > 0)
            {
                prog?.SetProgress(count / length, status);
            }
            else
            {
                prog?.SetProgress(1, status);
            }
        }
    }
}
