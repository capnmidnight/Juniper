using System.Collections;
using Juniper.Progress;

namespace UnityEngine
{
    public static class AsyncOperationExt
    {
        public static IEnumerator AsCoroutine(this AsyncOperation op)
        {
            return op.AsCoroutine(null);
        }

        public static IEnumerator AsCoroutine(this AsyncOperation op, IProgress prog)
        {
            prog.Report(op.progress);
            while (!op.isDone)
            {
                yield return null;
                prog.Report(op.progress);
            }
        }
    }
}
