using System.Collections;

namespace UnityEngine
{
    public static class AsyncOperationExt
    {
        public static IEnumerator AsCoroutine(this AsyncOperation op)
        {
            while (!op.isDone)
            {
                yield return null;
            }
        }
    }
}
