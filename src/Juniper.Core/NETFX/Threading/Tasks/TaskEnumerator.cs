using System.Collections;
using System.Collections.Generic;

namespace System.Threading.Tasks
{
    public static class TaskEnumeratorExt
    {
        public static IEnumerator AsCoroutine(this Task task)
        {
            while (task.IsRunning())
            {
                yield return null;
            }
        }

        public static IEnumerator<T> AsCoroutine<T>(this Task<T> task)
        {
            while (task.IsRunning())
            {
                yield return default;
            }

            if (task.Status == TaskStatus.RanToCompletion)
            {
                yield return task.Result;
            }
        }
    }
}