using UnityEngine;

namespace Juniper.Progress
{
    public class UnityAsyncOperationProgress : IProgress
    {
        private readonly AsyncOperation op;

        public UnityAsyncOperationProgress(AsyncOperation op)
        {
            this.op = op;
        }

        public float Progress
        {
            get
            {
                return op.progress;
            }
        }
    }
}