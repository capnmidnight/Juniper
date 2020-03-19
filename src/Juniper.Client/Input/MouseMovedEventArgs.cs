using System;

namespace Juniper.Input
{
    public class MouseMovedEventArgs : EventArgs
    {
        public int X { get; }
        public int Y { get; }

        public int DX { get; }
        public int DY { get; }

        public MouseMovedEventArgs(int x, int y, int dx, int dy)
        {
            X = x;
            Y = y;
            DX = dx;
            DY = dy;
        }
    }
}
