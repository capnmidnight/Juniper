namespace Juniper.Input;

[Flags]
public enum VirtualTouchPadButtons
{
    None = 0,
    Center = ~None,
    Any = Center - 1,

    Top = 0x01,
    Right = 0x02,
    Bottom = 0x04,
    Left = 0x08,

    TopLeft = Top | Left,
    TopRight = Top | Right,
    BottomLeft = Bottom | Left,
    BottomRight = Bottom | Right
}
