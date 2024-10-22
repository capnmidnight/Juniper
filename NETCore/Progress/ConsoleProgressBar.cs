namespace Juniper.Progress;

using System.Text;

public class ConsoleProgressBar : IProgress
{
    public string? Status { get; private set; }
    public float Progress { get; private set; }

    private readonly int originX;
    private readonly int originY;

    private readonly StringBuilder sb = new();

    public ConsoleProgressBar()
    {
        (originX, originY) = Console.GetCursorPosition();
    }

    public ConsoleProgressBar(int x, int y)
    {
        originX = x;
        originY = y;
    }

    public void Report(float progress, string? status = null)
    {
        lock (this)
        {
            Progress = progress;
            Status = status;

            Clear();
            Write();
        }
    }

    private void Write()
    {
        sb.Append('[');
        for (var p = 0f; p <= 1; p += 0.1f)
        {
            sb.Append(p <= Progress ? "o" : "-");
        }
        sb.AppendFormat("] {0:P0} ", Progress);
        sb.Append(Status);

        Dump();
    }

    private void Clear()
    {
        for (var i = 0; i < sb.Length; ++i)
        {
            sb[i] = ' ';
        }

        Dump();

        sb.Clear();
    }

    private void Dump()
    {
        var (oldX, oldY) = Console.GetCursorPosition();
        if(oldY == originY && oldY < Console.BufferHeight - 1)
        {
            oldY += 1;
        }

        Console.SetCursorPosition(originX, originY);
        Console.Write(sb.ToString());
        Console.SetCursorPosition(oldX, oldY);
    }
}
