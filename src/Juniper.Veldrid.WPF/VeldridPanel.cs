using System;
using System.Runtime.InteropServices;

using Veldrid;

namespace Juniper.VeldridIntegration.WPFSupport
{
    public class VeldridPanel : Win32HwndControl
    {
        public SwapchainSource VeldridSwapchainSource { get; internal set; }
        public event EventHandler Ready;
        public event EventHandler Resize;

        protected sealed override void Initialize()
        {
            var mainModule = typeof(VeldridPanel).Module;
            var hinstance = Marshal.GetHINSTANCE(mainModule);
            VeldridSwapchainSource = SwapchainSource.CreateWin32(Hwnd, hinstance);
            Ready?.Invoke(this, EventArgs.Empty);
            //CompositionTarget.Rendering += OnCompositionTargetRendering;
        }

        protected sealed override void Uninitialize()
        {
            //CompositionTarget.Rendering -= OnCompositionTargetRendering;
        }

        protected sealed override void Resized()
        {
            Resize?.Invoke(this, EventArgs.Empty);
        }
    }
}
