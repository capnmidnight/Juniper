using Juniper.Processes;

namespace Juniper
{
    public class YouTubeDLP : ShellCommand
    {
        private const string YT_DLP = "yt-dlp";

        public static bool IsAvailable => FindCommandPath(YT_DLP) is not null;
        public static IEnumerable<string> AttemptPaths => FindCommandPaths(YT_DLP);

        private static readonly HttpClient http = new();
        public record Output(string? ContentType, string Url);

        private YouTubeDLP(string youtubeUrl)
            : base(YT_DLP, "--get-url", youtubeUrl)
        {

        }

        public static async Task<Output[]> GetURLs(string youtubeUrl)
        {
            var cmd = new YouTubeDLP(youtubeUrl);
            var urls = await cmd.RunForStdOutAsync();
            var output = await Task.WhenAll(urls.Select(GetURL));
            return output.ToArray();
        }

        private static async Task<Output> GetURL(string url)
        {
            using var request = new HttpRequestMessage(HttpMethod.Head, url);
            using var response = await http.SendAsync(request);
            return new Output(response?.Content?.Headers?.ContentType?.MediaType, url);
        }
    }
}