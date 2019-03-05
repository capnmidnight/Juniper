using Juniper.Unity.Input.Pointers;

using System;

namespace Juniper.Unity.Input
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
