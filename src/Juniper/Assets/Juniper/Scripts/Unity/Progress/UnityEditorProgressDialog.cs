#if UNITY_EDITOR

using UnityEditor;

namespace Juniper.Progress
{
    public class UnityEditorProgressDialog : IProgress
    {
        private readonly string title;

        private string lastStatus;

        public UnityEditorProgressDialog(string title)
        {
            this.title = title;
        }

        public float Progress
        {
            get; private set;
        }

        public void Report(float progress)
        {
            Report(progress, null);
        }

        public void Report(float progress, string status)
        {
            Progress = progress;
            var progText = progress.ToString("P1");
            if (status == null)
            {
                status = progText;
            }
            else
            {
                status = $"{status} {progText}";
            }

            if (status != lastStatus)
            {
                lastStatus = status;
                if (EditorUtility.DisplayCancelableProgressBar(title, status, progress))
                {
                    throw new System.OperationCanceledException();
                }
            }
        }
    }
}

#endif