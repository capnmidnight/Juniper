namespace System.Threading.Tasks;

public static class TaskExtensions
{
    public static Task OnError(this Task task, Action<Exception> resume)
    {
        return task?.ContinueWith(
            t => resume(t.Exception),
            CancellationToken.None,
            TaskContinuationOptions.OnlyOnFaulted,
            TaskScheduler.Default);
    }
}