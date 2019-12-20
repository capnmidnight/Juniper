using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

using Line = System.Collections.Generic.List<Juniper.Primrose.Token>;

namespace Juniper.Primrose
{
    public interface IInterpreter
    {
        event EventHandler<string> Output;
        event EventHandler<Action<string>> Input;
        event EventHandler<RuntimeException> Error;
        event EventHandler ClearScreen;
        event EventHandler<Action<Func<string, byte[]>>> LoadFile;
        event EventHandler Next;
        event EventHandler Done;

        void Interpret();
    }
}