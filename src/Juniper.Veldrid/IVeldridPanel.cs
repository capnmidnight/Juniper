using System;

using Veldrid;

namespace Juniper.VeldridIntegration
{
    public interface IVeldridPanel
    {
        SwapchainSource VeldridSwapchainSource { get; }

        event EventHandler Ready;
        event EventHandler Resize;
        event EventHandler Destroying;

        uint RenderWidth { get; }
        uint RenderHeight { get; }
    }
}
