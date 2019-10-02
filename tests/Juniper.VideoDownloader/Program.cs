using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Juniper.Progress;

using static System.Console;
using static Juniper.HTTP.MediaType;

namespace Juniper.VideoDownloader
{
    class Program
    {
        class ConsoleProgress : IProgress
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
                    for(int i = 0; i < lastStatus.Length; ++i)
                    {
                        Write('\b');
                    }
                    Write(Status);
                }
            }
        }

        static async Task Main(string[] args)
        {
            Write("Enter URL:> ");
            var url = ReadLine();
            var uri = new Uri(url);
            var request = HttpWebRequestExt.Create(uri)
                .DoNotTrack()
                .Accept(Any);
            using (var response = await request.Get())
            {
                var contentType = Lookup(response.ContentType);
                WriteLine($"Status {response.StatusCode}");
                WriteLine($"Content-Type {contentType.Value}");
                WriteLine($"Content-Length {response.ContentLength}");
                var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var fileName = Path.Combine(desktop, PathExt.FixPath(uri.PathAndQuery.Substring(1)));
                var fileExt = Path.GetExtension(fileName).Substring(1);
                if (contentType?.PrimaryExtension != null && !contentType.Extensions.Contains(fileExt))
                {
                    fileName += "." + contentType.PrimaryExtension;
                }
                var file = new FileInfo(fileName);
                file.Directory.Create();
                using (var outStream = file.Create())
                using (var body = response.GetResponseStream())
                {
                    var inStream = body;
                    if (response.ContentLength > 0)
                    {
                        inStream = new ProgressStream(body, response.ContentLength, new ConsoleProgress());
                    }
                    inStream.CopyTo(outStream);
                }
            }
        }
    }
}
