namespace Juniper.Input
{
    public class KeyChangeEvent : KeyEvent
    {
        public bool State { get; }

        public KeyChangeEvent(string name, bool state)
            : base(name)
        {
            State = state;
        }
    }
}
