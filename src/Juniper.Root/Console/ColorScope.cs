using System;

using C = System.Console;

namespace Juniper.Console
{
    public class ColorScope : IDisposable
    {
        private readonly ConsoleColor background;
        private readonly ConsoleColor foreground;

        public ColorScope(ConsoleColor background, ConsoleColor foreground)
        {
            this.background = C.BackgroundColor;
            this.foreground = C.ForegroundColor;
            C.BackgroundColor = background;
            C.ForegroundColor = foreground;
        }

        public ColorScope(ConsoleColor foreground)
            : this(C.BackgroundColor, foreground)
        { }

        protected virtual void Dispose(bool disposing)
        {
            C.BackgroundColor = background;
            C.ForegroundColor = foreground;
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
