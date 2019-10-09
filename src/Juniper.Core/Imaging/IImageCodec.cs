using System;
using System.IO;
using System.Threading.Tasks;

using Juniper.IO;
using Juniper.Progress;

namespace Juniper.Imaging
{
    public interface IImageCodec<T> : IFactory<T>
    {
        MediaType.Image ContentType { get; }
    }
}