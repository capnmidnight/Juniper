namespace System.Threading.Tasks
{
    public static class TaskExt
    {
        public static bool IsRunning(this Task task)
        {
            return !(task == null || task.IsCompleted || task.IsCanceled || task.IsFaulted);
        }
    }
}