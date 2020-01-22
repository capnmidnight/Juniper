using System.Collections.Generic;

namespace Juniper
{
    public class BufferEventArgs : EventArgs<IReadOnlyCollection<byte>>
    {
        public BufferEventArgs(byte[] args)
            : base(args)
        { }
    }
}