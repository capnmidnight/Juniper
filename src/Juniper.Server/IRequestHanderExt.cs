namespace Juniper.HTTP.Server
{
    public static class IRequestHanderExt
    {
        public static bool HasHttps(this IRequestHandler handler)
        {
            if (handler is null)
            {
                return false;
            }
            else
            {
                return (handler.Protocols & HttpProtocols.HTTPS) != 0;
            }
        }
    }
}