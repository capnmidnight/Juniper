using System;

namespace Juniper.Primrose
{
    public interface IInterpreter
    {
        event EventHandler<string> Output;
        event EventHandler<Action<string>> Input;
        event EventHandler<RuntimeException> RuntimeError;
        event EventHandler ClearScreen;
        event EventHandler<Action<Func<string, byte[]>>> LoadFile;
        event EventHandler ContinueNext;
        event EventHandler Done;

        void Interpret();
    }
}