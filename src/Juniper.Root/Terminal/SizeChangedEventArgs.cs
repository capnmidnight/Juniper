namespace Juniper.Terminal;

public class SizeChangedEventArgs
{

    public SizeChangedEventArgs(int oldWidth, int oldHeight, int newWidth, int newHeight)
    {
        OldWidth = oldWidth;
        OldHeight = oldHeight;
        NewWidth = newWidth;
        NewHeight = newHeight;
    }

    public int OldWidth { get; }

    public int OldHeight { get; }

    public int NewWidth { get; }

    public int NewHeight { get; }
}