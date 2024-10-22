namespace Juniper.Terminal;

public class ConsoleBufferSubdivision : IConsoleBuffer
{
    private readonly IConsoleBuffer parent;
    private readonly int x;
    private readonly int y;

    public ConsoleBufferSubdivision(IConsoleBuffer parent, int x, int y, int width, int height)
    {
        this.parent = parent;
        this.x = x;
        this.y = y;

        Width = width;
        Height = height;
    }

    public int AbsoluteLeft => parent.AbsoluteLeft + x;

    public int AbsoluteRight => AbsoluteLeft + Width - 1;

    public int AbsoluteTop => parent.AbsoluteTop + y;

    public int AbsoluteBottom => AbsoluteTop + Height - 1;

    public int Width { get; private set; }

    public int Height { get; private set; }

    public void Draw(int x, int y, char c, ConsoleColor f, ConsoleColor b)
    {
        parent.Draw(this.x + x, this.y + y, c, f, b);
    }

    public ConsoleColor GetBackgroundColor(int x, int y)
    {
        return parent.GetBackgroundColor(this.x + x, this.y + y);
    }
}
