using System;

namespace Juniper.Progress
{
    public class ProgressEventer : IProgress
    {
        public event EventHandler<EventArgs> ProgressUpdated;

        public string Status
        {
            get;
            private set;
        }

        public float Progress
        {
            get;
            private set;
        }

        public void ReportWithStatus(float progress, string status)
        {
            Status = status;
            Progress = progress;
            ProgressUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}