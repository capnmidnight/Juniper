using System.Text.RegularExpressions;

namespace Juniper.AppShell;

public class AppShellOptions
{
    public const string AppShell = "AppShell";
    public string? SplashScreenPath { get; set; } = null;

    public AppShellWindowOptions? Window { get; set; } = null;
}

public class AppShellWindowOptions
{
    public string? Title { get; set; } = null;
    public bool? Maximized { get; set; } = null;
    public AppShellWindowSizeOption? Size { get; set; } = null;
}

public partial class AppShellWindowSizeOption
{
    public int? Width { get; set; } = null;
    public int? Height { get; set; } = null;


    [GeneratedRegex("^\\d+x\\d+$", RegexOptions.Compiled)]
    private static partial Regex GetSizePattern();

    private static readonly Regex sizePattern = GetSizePattern();

    public static implicit operator AppShellWindowSizeOption(string expr)
    {
        if (sizePattern.IsMatch(expr))
        {
            throw new ArgumentException("Invalid size format");
        }

        var parts = expr.Split('x');
        var dims = parts.Select(int.Parse).ToArray();
        return new AppShellWindowSizeOption
        {
            Width = dims[0],
            Height = dims[1]
        };
    }
}