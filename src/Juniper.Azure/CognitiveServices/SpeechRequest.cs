using System.IO;
using System.Net;
using System.Threading.Tasks;
using Juniper.HTTP;

namespace Juniper.Azure.CognitiveServices
{
    public class SpeechRequest : AbstractAzureSpeechRequest
    {
        private readonly string resourceName;

        public string Text { get; set; }

        public string VoiceName { get; set; }

        public OutputFormat OutputFormat { get; set; }

        private string ssmlText;
        private int ssmlTextLength;

        public SpeechRequest(string region, string authToken, string resourceName, DirectoryInfo cacheLocation)
            : base(region, "speech/recognition/conversation/cognitiveservices/v1", authToken, cacheLocation)
        {
            this.resourceName = resourceName;
        }

        public SpeechRequest(string region, string authToken, string resourceName)
            : this(region, authToken, resourceName, null)
        { }

        protected override BodyInfo GetBodyInfo()
        {
            ssmlText =
$@"<speak version='1.0' xmlns='https://www.w3.org/2001/10/synthesis' xml:lang='en-US'>
              <voice name='{VoiceName}'>{Text}</voice>
</speak>";
            ssmlTextLength = System.Text.Encoding.UTF8.GetByteCount(ssmlText);
            return new BodyInfo(MediaType.Application.SsmlXml, ssmlTextLength);
        }

        protected override void WriteBody(Stream stream)
        {
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(ssmlText);
            }
        }

        protected override async Task ModifyRequest(HttpWebRequest request)
        {
            await base.ModifyRequest(request);
            request.KeepAlive()
                .UserAgent(resourceName)
                .Header("X-Microsoft-OutputFormat", OutputFormat.Value);
        }
    }
}
