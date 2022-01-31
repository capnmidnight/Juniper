using Juniper.Processes;

namespace Juniper
{
    public class YouTubeDLP : ShellCommand
    {
        private const string YT_DLP = "yt-dlp";

        public static bool IsAvailable => FindCommandPath(YT_DLP) is not null;
        public static IEnumerable<string> AttemptPaths => FindCommandPaths(YT_DLP);

        private static readonly HttpClient http = new();

        public record MediaEntry(string? ContentType, string Url);

        public record Result(MediaEntry? Video, MediaEntry? Audio);


        private YouTubeDLP(string youtubeUrl)
            : base(YT_DLP, "--get-url", youtubeUrl)
        {

        }

        public static async Task<Result> GetURLs(string youtubeUrl)
        {
            var cmd = new YouTubeDLP(youtubeUrl);
            var urls = await cmd.RunForStdOutAsync();
            var output = await Task.WhenAll(urls.Select(GetURL));
            var vid = output.FirstOrDefault(v => MediaType.Video.AnyVideo.Matches(v.ContentType));
            var aud = output.FirstOrDefault(v => MediaType.Audio.AnyAudio.Matches(v.ContentType));
            return new Result(vid, aud);
        }

        private static async Task<MediaEntry> GetURL(string url)
        {
            using var request = new HttpRequestMessage(HttpMethod.Head, url);
            using var response = await http.SendAsync(request);
            return new MediaEntry(response?.Content?.Headers?.ContentType?.MediaType, url);
        }
    }
}