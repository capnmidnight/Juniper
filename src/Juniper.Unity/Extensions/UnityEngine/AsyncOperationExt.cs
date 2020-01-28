using System.Collections;
using System.Threading.Tasks;

using Juniper.IO;

namespace UnityEngine
{
    public static class AsyncOperationExt
    {
        public static Task AsTaskAsync(this AsyncOperation op, IProgress prog)
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

        public static Task AsTaskAsync(this AsyncOperation op)
        {
            return op.AsTaskAsync(null);
        }

        public static IEnumerator AsCoroutine(this AsyncOperation op, IProgress prog)
        {
            return op.AsTaskAsync(prog).AsCoroutine();
        }

        public static IEnumerator AsCoroutine(this AsyncOperation op)
        {
            return op.AsCoroutine(null);
        }
    }
}
