#if UNITY_EDITOR

using System;

using UnityEditor;

namespace Juniper.Progress
{
    public class UnityEditorProgressDialog : IProgress, IDisposable
    {
        private readonly string title;

        private string lastStatus;

        private bool canceled;

        public UnityEditorProgressDialog(string title)
        {
            this.title = title;
            EditorApplication.update += ShowStatus;
        }

        public string Status
        {
            get; private set;
        }

        public float Progress
        {
            get; private set;
        }

        public void ReportWithStatus(float progress, string status)
        {
            if (canceled)
            {
                throw new OperationCanceledException();
            }

            Progress = progress;
            var progText = progress.ToString("P1");
            if (status == null)
            {
                Status = progText;
            }
            else
            {
                Status = $"{status} {progText}";
            }
        }

        private void ShowStatus()
        {
            if (Status != lastStatus)
            {
                lastStatus = Status;
                canceled = EditorUtility.DisplayCancelableProgressBar(title, Status, Progress);
            }
        }

        public void Close()
        {
            EditorApplication.update -= ShowStatus;
            EditorUtility.ClearProgressBar();
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Close();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        #endregion IDisposable Support
    }
}

#endif