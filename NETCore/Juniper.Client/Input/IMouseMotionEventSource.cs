using System;

namespace Juniper.Input
{
    public interface IMouseMotionEventSource
    {
        event EventHandler<MouseMovedEventArgs> Moved;

        void Start();
        void Quit();
    }
}
