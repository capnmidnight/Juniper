using System;
using System.Numerics;
using System.Threading;
using System.Windows.Forms;

using Intel.RealSense;

using static System.Console;

using Pose = Juniper.Mathematics.Pose;

namespace Juniper
{

    public static class Program
    {
        const int fisheySensorIdx = 1;

        private static Pose objectPoseInWorld, devicePoseInWorld;

        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;

            WriteLine("Waiting for device...");

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

            WriteLine("Device found. Streaming data...");

            using var form = new MainForm();
            form.ClientSize = new System.Drawing.Size(intrinsics.width, intrinsics.height);
            form.KeyUp += Form_KeyUp;

            using var graphics = form.CreateGraphics();

            Application.Run(form);
        }

        private static void Form_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                objectPoseInWorld = ResetObjectPose(devicePoseInWorld);
                break;
            }
        }

        private static Pose DEFAULT_POSE = new Pose
        {
            Position = new Vector3(0, 0, -0.5f),
            Orientation = Quaternion.Identity
        };

        private static Pose ResetObjectPose(Pose devicePoseInWorld)
        {
            return devicePoseInWorld * DEFAULT_POSE;
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Error.WriteLine(e.Exception.Unroll());
        }
    }
}
