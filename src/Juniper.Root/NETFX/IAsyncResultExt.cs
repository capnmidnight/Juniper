namespace System
{
    public static class IAsyncResultExt
    {
        public static async Task<IAsyncResult> AsAsync(this IAsyncResult res)
        {
            if (res is null)
            {
                throw new ArgumentNullException(nameof(res));
            }

            while (!res.IsCompleted)
            {
                await Task.Yield();
            }

            return res;
        }
    }
}
