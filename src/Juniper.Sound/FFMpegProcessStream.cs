using Juniper.Processes;

namespace Juniper.Sound
{

    public class FFMpegProcessStream : PipedProcessStream
    {
        public FFMpegProcessStream(string args)
            : base("ffmpeg", "-nostdin -hide_banner " + args)
        {

        }
    }
}
