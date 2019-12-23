using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Juniper.IO
{
    public interface ISaveable<T> : ISerializable
    { }
}
