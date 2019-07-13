using System.IO;
using Juniper.Progress;

namespace System.Net
{
    /// <summary>
    /// Perform HTTP queries
    /// </summary>
    public static class HttpWebResponseExt
    {
        public static string ReadBodyString(this HttpWebResponse response, IProgress prog = null)
        {
            using (var stream = response.GetResponseStream())
            {
                return stream.ReadString(response.ContentLength, prog);
            }
        }

        public static byte[] ReadBodyBytes(this HttpWebResponse response, IProgress prog = null)
        {
            using (var stream = response.GetResponseStream())
            {
                return stream.ReadBytes(response.ContentLength, prog);
            }
        }
    }
}
