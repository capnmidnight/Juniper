using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Juniper.Collections
{
    public interface IRoute<ValueT> :
        IEquatable<IRoute<ValueT>>,
        IComparable,
        IComparable<IRoute<ValueT>>
    {
        float Cost { get; }

        int Count { get; }

        ValueT Start { get; }

        ValueT End { get; }

        IReadOnlyList<ValueT> Nodes { get; }
    }
}