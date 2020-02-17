using System;

namespace Juniper.Input
{
    public class KeyEvent : EventArgs
    {
        public string Name { get; }
        public KeyEvent(string name)
        {
            Name = name;
        }
    }
}
