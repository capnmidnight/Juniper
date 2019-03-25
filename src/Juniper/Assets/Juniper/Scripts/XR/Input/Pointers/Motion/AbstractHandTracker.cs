using Juniper.Unity.Haptics;

using System;

namespace Juniper.Unity.Input.Pointers.Motion
{
    public abstract class AbstractHandTrackerConfiguration<HandIDType, ButtonIDType>
        : AbstractHandedPointerConfiguration<HandIDType, ButtonIDType>
        where HandIDType : struct, IComparable
        where ButtonIDType : struct
    {
        protected override string PointerNameStub
        {
            get
            {
                return "Hand";
            }
        }
    }

    public abstract class AbstractHandTracker<HandIDType, ButtonIDType, ConfigType> :
        AbstractHandedPointer<HandIDType, ButtonIDType, ConfigType, NoHaptics>
        where HandIDType : struct, IComparable
        where ButtonIDType : struct
        where ConfigType : AbstractHandTrackerConfiguration<HandIDType, ButtonIDType>, new()
    {
    }
}
