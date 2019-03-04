namespace Juniper.Progress
{
    public sealed class StaticProgress : IProgressReceiver
    {
        public static IProgress COMPLETE = new StaticProgress(1);

        private StaticProgress(float value)
        {
            SetProgress(value);
        }

        public StaticProgress()
        {
            SetProgress(0);
        }

        public float Progress
        {
            get; private set;
        }

        public void SetProgress(float progress, string status = null)
        {
            Progress = progress;
        }
    }
}
