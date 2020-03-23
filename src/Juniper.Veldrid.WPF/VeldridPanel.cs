using System;
using System.Runtime.InteropServices;

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
            //CompositionTarget.Rendering += OnCompositionTargetRendering;
        }

        protected sealed override void Resized()
        {
            Resize?.Invoke(this, EventArgs.Empty);
        }

        protected sealed override void Uninitialize()
        {
            Destroying?.Invoke(this, EventArgs.Empty);
            //CompositionTarget.Rendering -= OnCompositionTargetRendering;
        }
    }
}
