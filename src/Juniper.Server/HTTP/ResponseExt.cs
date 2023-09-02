namespace Microsoft.AspNetCore.Http;

public static class ResponseExt
{
    public static void RegisterForDispose(this HttpResponse response, IEnumerable<IDisposable> disposables)
    {
        foreach (var obj in disposables)
        {
            if (obj is not null)
            {
                response.RegisterForDispose(obj);
            }
        }
    }
}