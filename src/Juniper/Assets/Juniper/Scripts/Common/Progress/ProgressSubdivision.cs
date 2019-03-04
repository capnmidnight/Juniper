namespace Juniper.Progress
{
    public class ProgressSubdivision : IProgressReceiver
    {
        private readonly IProgressReceiver parent;
        private readonly float start, length;
        private readonly string prefix;
        private float localProgress;

        public ProgressSubdivision(IProgressReceiver parent, float start, float length, string prefix = null)
        {
            this.parent = parent;
            this.start = UnityEngine.Mathf.Max(0, start);
            this.length = length;
            this.prefix = prefix;
        }

        public float Progress
        {
            get
            {
                return localProgress;
            }
        }

        public void SetProgress(float progress, string status = null)
        {
            localProgress = progress;
            var prog = start + (progress * length);
            if (prefix == null)
            {
                parent?.SetProgress(prog, status);
            }
            else
            {
                parent?.SetProgress(prog, prefix + " " + status);
            }
        }
    }
}