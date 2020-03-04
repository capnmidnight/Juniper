using Intel.RealSense;

namespace Juniper
{
    public static class Program
    {
        const int fisheySensorIdx = 1;

        public static void Main()
        {
            using var pipe = new Pipeline();
            using var cfg = new Config();

            cfg.EnableStream(Stream.Pose);
            cfg.EnableStream(Stream.Fisheye, 1);
            cfg.EnableStream(Stream.Fisheye, 2);

            var pipeProfile = pipe.Start(cfg);

            using var fisheyeStream = pipeProfile.GetStream(Stream.Fisheye, fisheySensorIdx);
            using var poseStream = pipeProfile.GetStream(Stream.Pose);
            var intrinsics = fisheyeStream
                .As<VideoStreamProfile>()
                .GetIntrinsics();
            var poseToFisheyeExtrinsics = poseStream.GetExtrinsicsTo(fisheyeStream);

        }
    }
}
