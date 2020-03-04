using System;

using Valve.VR;

namespace Juniper
{
    public static class Program
    {
        public static void Main()
        {
            Console.WriteLine("Is Runtime Installed: {0}", OpenVR.IsRuntimeInstalled());
            var isHmdPresent = OpenVR.IsHmdPresent();
            Console.WriteLine("Is HMD Present: {0}", isHmdPresent);
            if (isHmdPresent)
            {
                try
                {
                    var err = EVRInitError.None;
                    var sys = OpenVR.Init(ref err, EVRApplicationType.VRApplication_Scene);
                    if (err != EVRInitError.None)
                    {
                        Console.WriteLine("Err {0}", err);
                    }
                    else
                    {
                        Console.WriteLine("Runtime version: {0}", sys.GetRuntimeVersion());
                        Console.WriteLine("Is display on desktop: {0}", sys.IsDisplayOnDesktop());
                        Console.WriteLine("Is input available: {0}", sys.IsInputAvailable());
                        Console.WriteLine("Is SteamVR drawing controllers: {0}", sys.IsSteamVRDrawingControllers());
                        Console.WriteLine("Should application pause: {0}", sys.ShouldApplicationPause());
                        Console.WriteLine("Should application reduce rendering work: {0}", sys.ShouldApplicationReduceRenderingWork());
                        sys.GetRecommendedRenderTargetSize(out var width, out var height);
                        Console.WriteLine("Recommended render target size: {0}x{1}", width, height);
                        Console.WriteLine("Button Name: {0}", sys.GetButtonIdNameFromEnum(EVRButtonId.k_EButton_A));
                    }
                }
                finally
                {
                    OpenVR.Shutdown();
                }
            }
        }
    }
}
