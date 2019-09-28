using System.IO;

namespace Juniper.Azure.CognitiveServices
{
    public class VoiceListRequest : AbstractAzureSpeechRequest
    {
        public VoiceListRequest(string region, string authToken, DirectoryInfo cacheLocation)
            : base(region, "cognitiveservices/voices/list", authToken, cacheLocation)
        { }

        public VoiceListRequest(string region, string authToken)
            : this(region, authToken, null)
        { }
        public VoiceListRequest(string region, DirectoryInfo cacheLocation)
            : this(region, null, cacheLocation)
        { }
        public VoiceListRequest(string region)
            : this(region, null, null)
        { }
    }
}
