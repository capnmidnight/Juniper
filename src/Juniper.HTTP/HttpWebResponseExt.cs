using System.IO;

namespace System.Net
{
    /// <summary>
    /// Perform HTTP queries
    /// </summary>
    public static class HttpWebResponseExt
    {
        public static string ReadBody(this HttpWebResponse response)
        {
            using (var stream = response.GetResponseStream())
            {
                return stream.ReadString();
            }
        }
    }
}
