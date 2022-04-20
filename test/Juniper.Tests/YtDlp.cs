using NUnit.Framework;

using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Juniper
{
    [TestFixture]
    public class YtDlpTests
    {
        private HttpClient http;

        [SetUp]
        public void Setup()
        {
            http = new(new HttpClientHandler
            {
                UseCookies = false
            });
        }

        [TearDown]
        public void TearDown()
        {
            http?.Dispose();
            http = null;
        }

        [Test]
        public async Task Parse1()
        {
            if(http is null)
            {
                throw new NullReferenceException("don't have an http client");
            }

            var urls = new[] {
                "https://www.youtube.com/watch?v=sPyAQQklc1s",
                "https://www.youtube.com/watch?v=MgJITGvVfR0",
                "https://www.youtube.com/watch?v=UUzQcPuK8uk",
                "https://www.youtube.com/watch?v=K6uGXtPCjEw",
                "https://www.youtube.com/watch?v=7NGExT9cPKA",
                "https://www.youtube.com/watch?v=PqpVB72lZa8"
            };

            foreach (var url in urls)
            {
                var obj = await YouTubeDLP.GetJSON(http, url);
            }
        }
    }
}