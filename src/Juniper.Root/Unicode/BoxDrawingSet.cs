using static Juniper.Unicode.BoxDrawingCharacters;

namespace Juniper.Unicode
{
    public class BoxDrawingSet
    {
        public static readonly BoxDrawingSet Light = new(
            LightVertical,
            LightHorizontal,
            LightDownAndRight,
            LightDownAndLeft,
            LightUpAndRight,
            LightUpAndLeft);

        public static readonly BoxDrawingSet Heavy = new(
            HeavyVertical,
            HeavyHorizontal,
            HeavyDownAndRight,
            HeavyDownAndLeft,
            HeavyUpAndRight,
            HeavyUpAndLeft);

        public static readonly BoxDrawingSet DoubleLight = new(
            DoubleVertical,
            DoubleHorizontal,
            DoubleDownAndRight,
            DoubleDownAndLeft,
            DoubleUpAndRight,
            DoubleUpAndLeft);

        public char Vertical { get; }
        public char Horizontal { get; }
        public char UpperLeft { get; }
        public char UpperRight { get; }
        public char LowerLeft { get; }
        public char LowerRight { get; }

        private BoxDrawingSet(char vert, char horiz, char ul, char ur, char ll, char lr)
        {
            Vertical = vert;
            Horizontal = horiz;
            UpperLeft = ul;
            UpperRight = ur;
            LowerLeft = ll;
            LowerRight = lr;
        }
    }
}
