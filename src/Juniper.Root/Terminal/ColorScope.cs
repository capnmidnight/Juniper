namespace Juniper.Terminal
{
    public class ColorScope : IDisposable
    {
        private readonly ConsoleColor background;
        private readonly ConsoleColor foreground;

        public ColorScope(ConsoleColor background, ConsoleColor foreground)
        {
            this.background = Console.BackgroundColor;
            this.foreground = Console.ForegroundColor;
            Console.BackgroundColor = background;
            Console.ForegroundColor = foreground;
        }

        public ColorScope(ConsoleColor foreground)
            : this(Console.BackgroundColor, foreground)
        { }

        protected virtual void Dispose(bool disposing)
        {
            Console.BackgroundColor = background;
            Console.ForegroundColor = foreground;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ColorScope()
        {
            Dispose(false);
        }
    }
}
