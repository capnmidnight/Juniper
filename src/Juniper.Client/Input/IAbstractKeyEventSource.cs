using System;

namespace Juniper.Input
{
    public interface IKeyEventSource
    {
        event EventHandler<KeyChangeEvent> KeyChanged;
        event EventHandler<KeyEvent> KeyDown;
        event EventHandler<KeyEvent> KeyUp;

        bool IsDown(string name);
        void Start();
        void Quit();
    }
}
