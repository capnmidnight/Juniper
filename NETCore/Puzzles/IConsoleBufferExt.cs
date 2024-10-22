using Juniper.Puzzles;

namespace Juniper.Terminal;

public static class IConsoleBufferExt
{
    public static void DrawPuzzle(this IConsoleBuffer buf, int x, int y, Puzzle p)
    {
        if (buf is null)
        {
            throw new ArgumentNullException(nameof(buf));
        }

        if (p is null)
        {
            throw new ArgumentNullException(nameof(p));
        }

        for (var dx = 0; dx < p.Width; ++dx)
        {
            for (var dy = 0; dy < p.Height; ++dy)
            {
                if (p[dx, dy] != Puzzle.EmptyTile)
                {
                    buf.Draw(x + dx, y + dy, '#', (ConsoleColor)(p[dx, dy] + 8), (ConsoleColor)p[dx, dy]);
                }
            }
        }
    }
}