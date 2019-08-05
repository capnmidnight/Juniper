using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace System
{
    public class TaskEnumerator<T> : IEnumerator<T>
    {
        private readonly Task<T> task;
        private bool valueCaptured;

        public TaskEnumerator(Task<T> task)
        {
            this.task = task;
        }

        public T Current
        {
            get;
            private set;
        }

        object IEnumerator.Current { get { return Current; } }

        public bool MoveNext()
        {
            if (!valueCaptured && task.IsCompleted)
            {
                Current = task.Result;
                valueCaptured = true;
            }

            return !(valueCaptured
                || task.IsCompleted
                || task.IsFaulted
                || task.IsCanceled);
        }

        public void Reset()
        {
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    task.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable Support
    }
}