using System;

namespace Juniper.Input
{
    [Flags]
    public enum InputMode
    {
        None,
        Auto = 1,
        Mouse = 2,
        Touch = 4,
        Gaze = 8,
        Motion = 16,
        Hands = 32,
        Voice = 64,
        Desktop = Mouse | Voice,
        Touchscreen = Touch | Gaze | Voice,
        SeatedVR = Mouse | Gaze | Motion | Voice,
        StandingVR = Gaze | Motion | Voice,
        HeadsetAR = Gaze | Motion | Hands | Voice
    }
}