using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Juniper.IO;
using Juniper.Progress;

using static System.Console;
using static Juniper.MediaType;

namespace Juniper.VideoDownloader
{
    public static class Program
    {
        private class ConsoleProgress : IProgress
        {
            public string Status { get; private set; }

            public float Progress { get; private set; }

            private string lastStatus = string.Empty;

            public void ReportWithStatus(float progress, string status)
            {
                var percent = Units.Converter.Label(progress, Units.UnitOfMeasure.Proportion, Units.UnitOfMeasure.Percent, 3);
                Progress = progress;
                Status = $"Progress {percent} {status ?? string.Empty}";
                if (Status != lastStatus)
                {
                    lastStatus = Status;
                    for (var i = 0; i < lastStatus.Length; ++i)
                    {
                        Write('\b');
                    }
                    Write(Status);
                }
            }
        }

        public static async Task Main()
        {
            Write("Enter URL:> ");
            var url = ReadLine();
            var uri = new Uri(url);
            var request = HttpWebRequestExt.Create(uri)
                .DoNotTrack()
                .Accept(Any);
            using var response = await request
                .GetAsync()
                .ConfigureAwait(false);
            var contentType = Lookup(response.ContentType);
            WriteLine($"Status {response.StatusCode}");
            WriteLine($"Content-Type {contentType.Value}");
            WriteLine($"Content-Length {response.ContentLength}");
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var fileName = Path.Combine(desktop, PathExt.FixPath(uri.PathAndQuery.Substring(1)));
            var fileExt = Path.GetExtension(fileName).Substring(1);
            if (contentType?.PrimaryExtension is object && !contentType.Extensions.Contains(fileExt))
            {
                fileName += "." + contentType.PrimaryExtension;
            }
            var file = new FileInfo(fileName);
            file.Directory.Create();
            using var outStream = file.Create();
            using var body = response.GetResponseStream();
            using var progStream = new ProgressStream(body, response.ContentLength, new ConsoleProgress(), false);
            await progStream.CopyToAsync(outStream)
                .ConfigureAwait(false);
        }
    }
}
