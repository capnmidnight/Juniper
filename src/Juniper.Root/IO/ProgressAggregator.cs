using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Juniper.IO
{
    public class ProgressAggregator :
        IProgress,
        IReadOnlyList<IProgress>
    {
        private readonly ProgressSubdivision[] subProgs;
        private readonly IProgress parent;

        private ProgressAggregator(IProgress parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Split a progress tracker into <paramref name="numParts"/> sub-trackers.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="numParts"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public ProgressAggregator(IProgress parent, long numParts)
            : this(parent)
        {
            if (numParts <= 0)
            {
                throw new ArgumentException("Number of subdivisions must be at least 1.", nameof(numParts));
            }

            subProgs = new ProgressSubdivision[numParts];
            for (var i = 0; i < numParts; ++i)
            {
                subProgs[i] = new ProgressSubdivision(this);
            }
        }

        /// <summary>
        /// Split a progress tracker into sub-trackers with the provided prefixes, one per prefix.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="prefixes"></param>
        /// <returns></returns>
        public ProgressAggregator(IProgress parent, params string[] prefixes)
            : this(parent)
        {
            if (prefixes.Length == 0)
            {
                throw new ArgumentException("Must provide at least 1 prefix", nameof(prefixes));
            }

            subProgs = (from prefix in prefixes
                        select new ProgressSubdivision(this, prefix))
                       .ToArray();
        }

        public string Status { get; private set; }

        public float Progress
        {
            get
            {
                return subProgs.Sum(p => p.Progress) / subProgs.Length;
            }
        }

        public int Count
        {
            get
            {
                return subProgs.Length;
            }
        }

        public IProgress this[int index]
        {
            get
            {
                return subProgs[index];
            }
        }

        public void ReportWithStatus(float progress, string status)
        {
            Status = status;
            parent?.ReportWithStatus(Progress, Status);
        }

        public IEnumerator<IProgress> GetEnumerator()
        {
            foreach (var prog in subProgs)
            {
                yield return prog;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
