using Juniper.Unity.Haptics;
using Juniper.Unity.Input.Pointers.Screen;

using UnityEngine;

namespace Juniper.Unity.Input.Pointers.Gaze
{
    public abstract class NosePointer<ButtonIDType, HapticsType, ConfigType> :
        AbstractScreenDevice<ButtonIDType, HapticsType, ConfigType>
        where ButtonIDType : struct
        where HapticsType : AbstractHapticDevice
        where ConfigType : AbstractPointerConfiguration<ButtonIDType>, new()
    {
        public override Vector3 WorldPoint
        {
            get
            {
                return WorldFromViewport(VIEWPORT_MIDPOINT);
            }
        }
    }
}
