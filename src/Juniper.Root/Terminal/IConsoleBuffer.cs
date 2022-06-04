namespace Juniper.Terminal
{
    public interface IConsoleBuffer
    {
        int AbsoluteLeft { get; }
        int AbsoluteRight { get; }
        int AbsoluteTop { get; }
        int AbsoluteBottom { get; }
        int Height { get; }
        int Width { get; }
        void Draw(int x, int y, char c, ConsoleColor f, ConsoleColor b);
        ConsoleColor GetBackgroundColor(int x, int y);
    }
}