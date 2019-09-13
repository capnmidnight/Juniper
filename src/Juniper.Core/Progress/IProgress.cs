using System;
using System.Collections.Generic;
using System.Linq;

namespace Juniper.Progress
{
    /// <summary>
    /// Progress reporting interface for asynchronous operations.
    /// </summary>
    public interface IProgress
    {
        /// <summary>
        /// The value of the most recent progress report.
        /// </summary>
        float Progress
        {
            get;
        }

        /// <summary>
        /// Report progress to a listener implementation.
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="status"></param>
        void ReportWithStatus(float progress, string status);
    }

    /// <summary>
    /// Progress reporting interface mixin methods.
    /// </summary>
    public static class IProgressExt
    {
        /// <summary>
        /// The minimum amount of difference to allow between two floats to consider them the same number.
        /// </summary>
        private const float ALPHA = 1e-3f;

        public static void Report(this IProgress prog, float progress)
        {
            prog?.ReportWithStatus(progress, null);
        }

        public static void Report(this IProgress prog, float progress, string status)
        {
            prog?.ReportWithStatus(progress, status);
        }

        /// <summary>
        /// Check to see <paramref name="prog"/>'s progress is within <see cref="ALPHA"/> of 1.
        /// </summary>
        /// <param name="prog"></param>
        /// <returns>True when progress is reasonably close to 1</returns>
        public static bool IsComplete(this IProgress prog)
        {
            return Math.Abs(prog.Progress - 1) < ALPHA;
        }

        /// <summary>
        /// Perform a series of actions, updating a progress tracker along the way.
        /// </summary>
        /// <typeparam name="T">A type of items in a list of objects to iterate over, for progress tracking</typeparam>
        /// <param name="prog">The progress tracker that aggregates all of the sub-operations.</param>
        /// <param name="arr">The list of objects to iterate over, for progress tracking.</param>
        /// <param name="act">The action to take on each list item.</param>
        /// <param name="error">A callback to fire if an error occurs when processing a list item.</param>
        public static void ForEach<T>(this IProgress prog, IEnumerable<T> arr, Action<T, IProgress> act, Action<Exception> error)
        {
            var len = arr.Count();
            if (len == 0)
            {
                prog.Report(1, "Nothing to do");
            }
            else
            {
                var progs = prog.Split(len);
                var index = 0;
                foreach (var item in arr)
                {
                    try
                    {
                        progs[index].Report(0);
                        act(item, progs[index]);
                        progs[index].Report(1);
                        ++index;
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
#pragma warning disable CA1031 // Do not catch general exception types
                    catch (Exception exp)
                    {
                        error?.Invoke(exp);
                    }
#pragma warning restore CA1031 // Do not catch general exception types
                }
            }
        }

        public static void ForEach<T>(this IProgress prog, IEnumerable<T> arr, Action<T, IProgress> act)
        {
            prog.ForEach(arr, act, null);
        }

        /// <summary>
        /// Make a subdivision of a progress meter. Progress from [0, 1] on the subdivision
        /// will map to progress from [<paramref name="start"/>, <paramref name="start"/> + <paramref name="length"/>]
        /// on the parent progress tracker.
        /// </summary>
        /// <param name="parent">The tracker to subdivide</param>
        /// <param name="start">The beginning of the output range.</param>
        /// <param name="length">The length of the output range.</param>
        /// <param name="prefix">A text prefix to include as part of the status update.</param>
        /// <returns></returns>
        public static IProgress Subdivide(this IProgress parent, float start, float length, string prefix)
        {
            return new ProgressSubdivision(parent, start, length, prefix);
        }

        public static IProgress Subdivide(this IProgress parent, float start, float length)
        {
            return parent.Subdivide(start, length, null);
        }

        /// <summary>
        /// Make a subdivision of a progress meter. Progress from [0, 1] on the subdivision
        /// will map to progress from [<paramref name="index"/>/<paramref name="count"/>, (<paramref name="index"/> + 1) / <paramref name="count"/>]
        /// on the parent progress tracker. This is useful for keeping track of iterations over elements in an array.
        /// </summary>
        /// <param name="parent">The tracker to subdivide</param>
        /// <param name="index">Which part of the subdivision to create.</param>
        /// <param name="count">The total number of subdivisions that this subdivision will be a part of.</param>
        /// <param name="prefix">A text prefix to include as part of the status update.</param>
        /// <returns></returns>
        public static IProgress Subdivide(this IProgress parent, int index, int count, string prefix)
        {
            return new ProgressSubdivision(parent, (float)index / count, 1f / count, prefix);
        }

        public static IProgress Subdivide(this IProgress parent, int index, int count)
        {
            return parent.Subdivide(index, count, null);
        }

        /// <summary>
        /// Split a progress tracker into <paramref name="numParts"/> sub-trackers.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="numParts"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static IProgress[] Split(this IProgress parent, long numParts)
        {
            if (numParts <= 0)
            {
                throw new ArgumentException("Number of subdivisions must be at least 1.", nameof(numParts));
            }

            var arr = new IProgress[numParts];
            var length = 1.0f / numParts;
            for (var i = 0; i < numParts; ++i)
            {
                arr[i] = parent?.Subdivide(i * length, length);
            }

            return arr;
        }

        /// <summary>
        /// Split a progress tracker into sub-trackers with the provided prefixes, one per prefix.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="prefixes"></param>
        /// <returns></returns>
        public static IProgress[] Split(this IProgress parent, params string[] prefixes)
        {
            if (prefixes == null)
            {
                throw new ArgumentNullException(nameof(prefixes));
            }

            if (prefixes.Length == 0)
            {
                throw new ArgumentException("Must provide at least one prefix", nameof(prefixes));
            }

            var arr = new IProgress[prefixes.Length];
            var length = 1.0f / prefixes.Length;
            for (var i = 0; i < prefixes.Length; ++i)
            {
                arr[i] = parent?.Subdivide(i * length, length);
            }

            return arr;
        }

        /// <summary>
        /// Convert a ratio progress value to a proportion progress value.
        /// </summary>
        /// <param name="prog"></param>
        /// <param name="count"></param>
        /// <param name="length"></param>
        /// <param name="status"></param>
        public static void Report(this IProgress prog, float count, float length, string status)
        {
            if (length > 0)
            {
                prog.Report(count / length, status);
            }
            else
            {
                prog.Report(1, status);
            }
        }

        public static void Report(this IProgress prog, float count, float length)
        {
            prog.Report(count, length, null);
        }
    }
}