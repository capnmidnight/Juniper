namespace Juniper.Progress
{
    public class ProgressSubdivision : IProgress
    {
        private readonly IProgress parent;
        private readonly float start, length;
        private readonly string prefix;

        public ProgressSubdivision(IProgress parent, float start, float length, string prefix = null)
        {
            this.parent = parent;
            this.start = System.Math.Max(0, start);
            this.length = length;
            this.prefix = prefix;
        }

        public float Progress
        {
            get;
            private set;
        }

        public void Report(float progress)
        {
            Report(progress, null);
        }

        public void Report(float progress, string status)
        {
            Progress = progress;
            var prog = start + (progress * length);
            if (prefix == null)
            {
                parent?.Report(prog, status);
            }
            else
            {
                parent?.Report(prog, prefix + " " + status);
            }
        }
    }
}
