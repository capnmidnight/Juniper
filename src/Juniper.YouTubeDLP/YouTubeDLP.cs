
using Juniper.Processes;

using Newtonsoft.Json;

namespace Juniper
{
    public class YouTubeDLP : ShellCommand
    {
        private const string YT_DLP = "yt-dlp";

        public static bool IsAvailable => FindCommandPath(YT_DLP) is not null;
        public static IEnumerable<string> AttemptPaths => FindCommandPaths(YT_DLP);

        public record MediaEntry(string? ContentType, string Url);

        public record Result(MediaEntry? Video, MediaEntry? Audio);


        private YouTubeDLP(string cmd, string youtubeUrl)
            : base(YT_DLP, cmd, youtubeUrl)
        {
        }

        public static async Task<YTMetadata?> GetJSON(HttpClient http, string youtubUrl)
        {
            var cmd = new YouTubeDLP("-j", youtubUrl);
            var lines = await cmd.RunForStdOutAsync();
            var input = lines.Join(Environment.NewLine);
            var obj = JsonConvert.DeserializeObject<YTMetadata>(input);
            if (obj is not null)
            {
                await Task.WhenAll(obj.formats
                    .Cast<YTMetadataURL>()
                    .Union(obj.requested_formats ?? Array.Empty<YTMetadataFormat>())
                    .Union(obj.thumbnails ?? Array.Empty<YTMetadataThumbnail>())
                    .Select(v => AddContentType(http, v)));
            }

            return obj;
        }

        private static async Task AddContentType(HttpClient http, YTMetadataURL format)
        {
            format.content_type = await GetContentType(http, format.url);
        }

        public static async Task<string> GetJSONString(HttpClient http, string youtubUrl)
        {
            var obj = await GetJSON(http, youtubUrl);
            var output = JsonConvert.SerializeObject(obj);
            return output;
        }

        private static async Task<string?> GetContentType(HttpClient http, string url)
        {
            using var request = new HttpRequestMessage(HttpMethod.Head, url);
            using var response = await http.SendAsync(request);
            return response?.Content?.Headers?.ContentType?.MediaType;
        }
    }
}