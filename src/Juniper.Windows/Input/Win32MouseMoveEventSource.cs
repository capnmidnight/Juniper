using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Juniper.Input
{
    public sealed class Win32MouseMoveEventSource
        : IMouseMotionEventSource
    {
        private readonly MouseMovedEventArgs args;
        private readonly Thread poller;
        private readonly CancellationToken canceller;

        public event EventHandler<MouseMovedEventArgs> Moved;

        private int lastX, lastY;

        public Win32MouseMoveEventSource(CancellationToken token)
        {
            args = new MouseMovedEventArgs();
            var threadStart = new ThreadStart(Update);
            poller = new Thread(threadStart);
            poller.SetApartmentState(ApartmentState.STA);
            canceller = token;
        }

        public void Start()
        {
            poller.Start();
        }

        public void Quit()
        {
            poller.Join();
        }

        private void Update()
        {
            while (!canceller.IsCancellationRequested)
            {
                if(NativeMethods.GetCursorPos(out var point))
                {
                    if(lastX != 0 || lastY != 0)
                    {
                        args.DX = point.X - lastX;
                        args.DY = point.Y - lastY;
                        args.X = point.X;
                        args.Y = point.Y;
                        Moved?.Invoke(this, args);
                    }

                    lastX = point.X;
                    lastY = point.Y;
                }
            }
        }

        private static class NativeMethods
        {
            [DllImport("user32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
            internal static extern bool GetCursorPos(out POINT point);

            [StructLayout(LayoutKind.Sequential)]
            internal struct POINT
            {
                internal int X;
                internal int Y;
            }
        }
    }

}