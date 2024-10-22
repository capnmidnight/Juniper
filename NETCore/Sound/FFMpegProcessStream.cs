using Juniper.Processes;

namespace Juniper.Sound;


public class FFMpegProcessStream : PipedProcessStream
{
    public MediaType MediaType { get; }

    public FFMpegProcessStream(MediaType mediaType, string format, string codec)
        : this(mediaType, $"-f {format} - codec:a ${codec}")
    {
        MediaType = mediaType;
    }

    public FFMpegProcessStream(MediaType mediaType, string command)
        : base("ffmpeg", $"-nostdin -hide_banner -i pipe:0 {command} pipe:1")
    {
        MediaType = mediaType;
    }
}
