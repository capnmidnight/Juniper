using Juniper.Haptics;
using Juniper.Input.Pointers.Screen;
using UnityEngine;

namespace Juniper.Input.Pointers.Gaze
{
    public abstract class NosePointer<ButtonIDType, HapticsType, ConfigType> :
        AbstractScreenDevice<ButtonIDType, HapticsType, ConfigType>
        where ButtonIDType : struct
        where HapticsType : AbstractHapticDevice
        where ConfigType : AbstractPointerConfiguration<ButtonIDType>, new()
    {
        public override Vector2 ScreenPoint =>
            SCREEN_MIDPOINT;
    }
}
