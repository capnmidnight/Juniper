#if UNITY_EDITOR

using Juniper.Progress;

using UnityEditor;

namespace Juniper.Unity.Progress
{
    public class UnityEditorProgressDialog : IProgressReceiver
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

        public void SetProgress(float progress, string status = null)
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