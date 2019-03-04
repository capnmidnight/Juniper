using Juniper.Input.Pointers;

using System;

namespace Juniper.Input
{
    /// <summary>
    /// An event argument type for passing on the pointer that was found.
    /// </summary>
    public class PointerFoundEventArgs : EventArgs
    {
        public readonly IPointerDevice device;

        public PointerFoundEventArgs(IPointerDevice device)
        {
            this.device = device;
        }
    }
}
