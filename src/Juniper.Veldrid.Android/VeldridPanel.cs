using System;

using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Veldrid;

namespace Juniper.VeldridIntegration.AndroidSupport
{
    public class VeldridPanel : SurfaceView, ISurfaceHolderCallback, IVeldridPanel

    {
        public SwapchainSource VeldridSwapchainSource { get; private set; }

        public event EventHandler Ready;
        public event EventHandler Resize;
        public event EventHandler Destroying;

        public uint RenderWidth => (uint)Width;
        public uint RenderHeight => (uint)Height;

        public VeldridPanel(Context context) :
            base(context)
        {
            Holder.AddCallback(this);
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            if (holder is null)
            {
                throw new ArgumentNullException(nameof(holder));
            }

            VeldridSwapchainSource = SwapchainSource.CreateAndroidSurface(holder.Surface.Handle, JNIEnv.Handle);
            Ready?.Invoke(this, EventArgs.Empty);
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            Resize?.Invoke(this, EventArgs.Empty);
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            Destroying?.Invoke(this, EventArgs.Empty);
        }
    }
}