using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Juniper.Progress;
using Juniper.Streams;
using static System.Console;
using static Juniper.HTTP.MediaType;

namespace Juniper.VideoDownloader
{
    class Program
    {
        class ConsoleProgress : IProgress
        {
            public float Progress { get; private set; }

            private string lastStatus = string.Empty;

            public void Report(float progress, string status)
            {
                var percent = Units.Converter.Label(progress, Units.UnitOfMeasure.Proportion, Units.UnitOfMeasure.Percent, 3);
                var curStatus = $"Progress {percent} {status}";
                if (curStatus != lastStatus)
                {
                    lastStatus = curStatus;
                    for(int i = 0; i < lastStatus.Length; ++i)
                    {
                        Write('\b');
                    }
                    Write(curStatus);
                }
            }

            public void Report(float value)
            {
                Report(value, string.Empty);
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
                var fileName = Path.Combine(desktop, uri.PathAndQuery.Substring(1)).RemoveInvalidChars();
                var fileExt = Path.GetExtension(fileName).Substring(1);
                if (contentType?.PrimaryExtension != null && !contentType.Extensions.Contains(fileExt))
                {
                    fileName += "." + contentType.PrimaryExtension;
                }
                var file = new FileInfo(fileName);
                file.Directory.Create();
                using (var outStream = file.OpenWrite())
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
