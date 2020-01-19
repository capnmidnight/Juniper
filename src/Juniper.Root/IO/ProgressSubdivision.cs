using System;

namespace Juniper.IO
{
    /// <summary>
    /// A small chunk of a progress meter, reporting its own progress
    /// up to a parent progress tracker.
    /// </summary>
    public class ProgressSubdivision : IProgress
    {
        /// <summary>
        /// The parent progress tracker that is aggregating progress
        /// from multiple ProgressSubdivisions.
        /// </summary>
        private readonly IProgress parent;

        /// <summary>
        /// The beginning of the mapped range of progress.
        /// </summary>
        private readonly float start;

        /// <summary>
        /// The length of hte mapped range of progress.
        /// </summary>
        private readonly float length;

        /// <summary>
        /// A prefix to add to the status update message;
        /// </summary>
        private readonly string prefix;

        /// <summary>
        /// Creates a progress subdivision.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="prefix"></param>
        public ProgressSubdivision(IProgress parent, float start, float length, string prefix = null)
            : this(parent, prefix)
        {
            this.start = Math.Max(0, start);
            this.length = length;
        }

        public ProgressSubdivision(IProgress parent, string prefix = null)
        {
            this.parent = parent;
            this.prefix = prefix;
        }

        /// <summary>
        /// Returns the current progress of the subdivision.
        /// </summary>
        public float Progress
        {
            get;
            private set;
        }

        public string Status
        {
            get;
            private set;
        }

        /// <summary>
        /// Receive a progress report.
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="status"></param>
        public void ReportWithStatus(float progress, string status)
        {
            Progress = progress;
            var prog = start + progress * length;
            if (prefix is object && status is object)
            {
                Status = prefix + " " + status;
            }
            else if (status is object)
            {
                Status = status;
            }
            else
            {
                Status = prefix;
            }

            parent.Report(prog, Status);
        }
    }
}