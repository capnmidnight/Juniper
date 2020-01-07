using System.Collections;
using System.Runtime.CompilerServices;

namespace System.Threading.Tasks
{
    public static class TaskEnumeratorExt
    {
        public static bool IsRunning(this Task task)
        {
            return task?.IsCompleted == false;
        }

        public static bool IsSuccessful(this Task task)
        {
            return task?.Status == TaskStatus.RanToCompletion;
        }

        public static IEnumerator AsCoroutine(this Task task)
        {
            while (task.IsRunning())
            {
                yield return null;
            }
        }

        public static IEnumerator AsCoroutine(this ConfiguredTaskAwaitable task)
        {
            var awaiter = task.GetAwaiter();
            while (!awaiter.IsCompleted)
            {
                yield return null;
            }
        }
    }
}