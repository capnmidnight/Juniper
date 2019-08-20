using System.Collections;

namespace System.Threading.Tasks
{
    public static class TaskExt
    {
        public static bool IsRunning(this Task task)
        {
            return task?.IsCompleted == false;
        }

        public static bool IsSuccessful(this Task task)
        {
            return task.Status == TaskStatus.RanToCompletion;
        }

        public static IEnumerator Waiter(this Task task)
        {
            while (task.IsRunning())
            {
                yield return null;
            }
        }
    }
}