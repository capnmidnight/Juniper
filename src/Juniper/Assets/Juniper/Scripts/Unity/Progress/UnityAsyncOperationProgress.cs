using Juniper.Progress;
using UnityEngine;

namespace Juniper.Unity.Progress
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
