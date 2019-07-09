using System.IO;

namespace System.Net
{
    /// <summary>
    /// Perform HTTP queries
    /// </summary>
    public static class HttpWebResponseExt
    {
        public static string ReadBodyString(this HttpWebResponse response)
        {
            using (var stream = response.GetResponseStream())
            {
                return stream.ReadString();
            }
        }

        public static byte[] ReadBodyBytes(this HttpWebResponse response)
        {
            using (var stream = response.GetResponseStream())
            {
                return stream.ReadBytes();
            }
        }
    }
}
