using System.Collections;
using System.Threading.Tasks;

using Juniper.IO;

namespace UnityEngine
{
    public static class AsyncOperationExt
    {
        public static Task AsTask(this AsyncOperation op, IProgress prog)
        {
            return Task.Run(() =>
            {
                prog.Report(op.progress);
                while (!op.isDone)
                {
                    Task.Yield();
                    prog.Report(op.progress);
                }
            });
        }

        public static Task AsTask(this AsyncOperation op)
        {
            return op.AsTask(null);
        }

        public static IEnumerator AsCoroutine(this AsyncOperation op, IProgress prog)
        {
            return op.AsTask(prog).AsCoroutine();
        }

        public static IEnumerator AsCoroutine(this AsyncOperation op)
        {
            return op.AsCoroutine(null);
        }
    }
}
