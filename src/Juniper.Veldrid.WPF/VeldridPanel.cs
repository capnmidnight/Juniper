using System;
using System.Runtime.InteropServices;
using System.Windows;

using Veldrid;

namespace Juniper.VeldridIntegration.WPFSupport
{
    public class VeldridPanel : Win32HwndControl, IVeldridPanel
    {
        public SwapchainSource VeldridSwapchainSource { get; private set; }

        public event EventHandler Ready;
        public event EventHandler Resize;
        public event EventHandler Destroying;

        public uint RenderWidth => (uint)RenderSize.Width;
        public uint RenderHeight => (uint)RenderSize.Height;

        protected sealed override void Initialize()
        {
            var mainModule = typeof(VeldridPanel).Module;
            var hinstance = Marshal.GetHINSTANCE(mainModule);
            VeldridSwapchainSource = SwapchainSource.CreateWin32(Hwnd, hinstance);
            Ready?.Invoke(this, EventArgs.Empty);
            var here = Parent as FrameworkElement;
            while (here != null)
            {
                if (here is Window window)
                {
                    window.Closing += Window_Closing;
                    here = null;
                }
                else
                {
                    here = here.Parent as FrameworkElement;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Destroying?.Invoke(this, EventArgs.Empty);
        }

        protected sealed override void Resized()
        {
            Resize?.Invoke(this, EventArgs.Empty);
        }

        protected sealed override void Uninitialize()
        {
        }
    }
}
