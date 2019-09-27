using Juniper.HTTP;

namespace Juniper.Azure.CognitiveServices
{
    public sealed class OutputFormat
    {
        public static OutputFormat Raw24KHz16BitMonoPCM = new OutputFormat("raw-24khz-16bit-mono-pcm", MediaType.Audio.X_Wav);
        public static OutputFormat Raw16KHz16BitMonoPCM = new OutputFormat("raw-16khz-16bit-mono-pcm", MediaType.Audio.X_Wav);
        public static OutputFormat Raw8KHz8BitMonoMULAW = new OutputFormat("raw-8khz-8bit-mono-mulaw", MediaType.Audio.X_Wav);

        public static OutputFormat Riff24KHz16BitMonoPCM = new OutputFormat("riff-24khz-16bit-mono-pcm", MediaType.Audio.X_Wav);
        public static OutputFormat Riff16KHz16BitMonoPCM = new OutputFormat("riff-16khz-16bit-mono-pcm", MediaType.Audio.X_Wav);
        public static OutputFormat Riff8KHz8BitMonoALAW = new OutputFormat("riff-8khz-8bit-mono-alaw", MediaType.Audio.X_Wav);
        public static OutputFormat Riff8KHz8BitMonoMULAW = new OutputFormat("riff-8khz-8bit-mono-mulaw", MediaType.Audio.X_Wav);

        public static OutputFormat Audio16KHz128KbitrateMonoMP3 = new OutputFormat("audio-16khz-128kbitrate-mono-mp3", MediaType.Audio.Mpeg);
        public static OutputFormat Audio16KHz64KbitrateMonoMP3 = new OutputFormat("audio-16khz-64kbitrate-mono-mp3", MediaType.Audio.Mpeg);
        public static OutputFormat Audio16KHz32KbitrateMonoMP3 = new OutputFormat("audio-16khz-32kbitrate-mono-mp3", MediaType.Audio.Mpeg);

        public static OutputFormat Audio24KHz160KbitrateMonoMP3 = new OutputFormat("audio-24khz-160kbitrate-mono-mp3", MediaType.Audio.Mpeg);
        public static OutputFormat Audio24KHz94KbitrateMonoMP3 = new OutputFormat("audio-24khz-96kbitrate-mono-mp3", MediaType.Audio.Mpeg);
        public static OutputFormat Audio24KHz48KbitrateMonoMP3 = new OutputFormat("audio-24khz-48kbitrate-mono-mp3", MediaType.Audio.Mpeg);

        public readonly string Value;
        public readonly MediaType.Audio Format;

        private OutputFormat(string formatName, MediaType.Audio audioFormat)
        {
            Value = formatName;
            Format = audioFormat;
        }
    }
}
