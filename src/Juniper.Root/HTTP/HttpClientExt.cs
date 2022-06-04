namespace Juniper.HTTP
{
    public static class HttpClientExt
    {
        public static void DownloadFile(this HttpClient client, string uri, string outPath)
        {
            using var stream = client.GetStreamAsync(uri).Result;
            using var outFile = new FileStream(outPath, FileMode.Create, FileAccess.Write);
            stream.CopyTo(outFile);
            stream.Close();
            outFile.Flush();
            outFile.Close();
        }
    }
}
