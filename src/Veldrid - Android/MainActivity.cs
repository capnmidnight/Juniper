using System;
using System.Threading;
using System.Threading.Tasks;

using Android.App;
using Android.Content.PM;
using Android.OS;

using Juniper.VeldridIntegration;
using Juniper.VeldridIntegration.AndroidSupport;

using Veldrid;

namespace Juniper
{
    [Activity(
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize,
        Label = "@string/app_name")]
    public class MainActivity : Activity
    {
        private static CancellationTokenSource canceller;
        private static VeldridPanel panel;
        private static VeldridDemoProgram demo;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            canceller = new CancellationTokenSource();

            panel = new VeldridPanel(this);
            panel.Ready += Panel_Ready;
            panel.Destroying += Panel_Destroying;
            panel.Touch += Panel_Touch;

            SetContentView(panel);
        }

        private void Panel_Touch(object sender, Android.Views.View.TouchEventArgs e)
        {
            if (e.Event.Action == Android.Views.MotionEventActions.Move
                && e.Event.HistorySize > 0)
            {
                var lastX = e.Event.GetHistoricalX(0);
                var lastY = e.Event.GetHistoricalY(0);
                var x = e.Event.GetX();
                var y = e.Event.GetY();
                var dx = x - lastX;
                var dy = y - lastY;
                demo?.SetMouseRotate((int)dx, (int)dy);
            }
        }

        private static void Panel_Ready(object sender, EventArgs e)
        {
            demo = new VeldridDemoProgram(
                new GraphicsDeviceOptions
                {
                    PreferDepthRangeZeroToOne = true,
                    PreferStandardClipSpaceYDirection = true,
                    ResourceBindingModel = ResourceBindingModel.Improved,
                    SwapchainDepthFormat = (PixelFormat)SwapchainDepthFormat.R16_UNorm,
                    SwapchainSrgbFormat = false,
                    SyncToVerticalBlank = true,
                    HasMainSwapchain = false
                },
                panel,
                canceller.Token);
            //demo.Error += form.SetError;
            demo.Update += Demo_Update;
            demo.Start();
        }

        private static void Demo_Update(float dt)
        {
            //demo.SetVelocity(keys.GetAxis("horizontal"), keys.GetAxis("forward"));
        }

        private static void Panel_Destroying(object sender, EventArgs e)
        {
            canceller.Cancel();
            demo.Quit();
            demo.Dispose();
        }
    }
}