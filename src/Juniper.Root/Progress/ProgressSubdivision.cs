using static System.Math;

namespace Juniper.Progress
{
    /// <summary>
    /// A small chunk of a progress meter, reporting its own progress
    /// up to a parent progress tracker. It maps its own [0, 1] range
    /// of progress onto a range on the parent that covers [<see cref="start"/>, <see cref="start"/> + <see cref="length"/>].
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
        public ProgressSubdivision(IProgress parent, float start, float length, string prefix)
        {
            this.parent = parent;
            this.start = Max(0, start);
            this.length = length;
            this.prefix = prefix;
        }

        public ProgressSubdivision(IProgress parent, float start, float length)
            : this(parent, start, length, null) { }

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
            var prog = start + (progress * length);
            if (prefix != null && status != null)
            {
                Status = prefix + " " + status;
            }
            else if (status != null)
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