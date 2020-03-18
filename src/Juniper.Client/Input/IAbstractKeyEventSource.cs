using System;

namespace Juniper.Input
{
    public interface IKeyEventSource
    {
        event EventHandler<KeyChangeEvent> KeyChanged;
        event EventHandler<KeyEvent> KeyDown;
        event EventHandler<KeyEvent> KeyUp;

        bool IsDown(string name);
        float GetValue(string name);
        void DefineAxis(string name, string negative, string positive);
        float GetAxis(string name);
        void Start();
        void Join();
    }
}
