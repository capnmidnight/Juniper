namespace Juniper.Input;

public class MouseMovedEventArgs : EventArgs
{
    public int X { get; set; }
    public int Y { get; set; }

    public int DX { get; set; }
    public int DY { get; set; }
}
