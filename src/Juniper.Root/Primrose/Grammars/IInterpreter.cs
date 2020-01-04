using System;

namespace Juniper.Primrose
{
    public interface IInterpreter
    {
        event EventHandler<Action<Func<string, byte[]>>> LoadFile;
        event EventHandler<Action<string>> Input;

        event EventHandler<StringEventArgs> Output;
        event EventHandler<ErrorEventArgs> RuntimeError;
        event EventHandler ClearScreen;
        event EventHandler ContinueNext;
        event EventHandler Done;

        void Interpret();
    }
}