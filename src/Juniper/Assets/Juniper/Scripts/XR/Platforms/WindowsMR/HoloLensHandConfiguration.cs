#if UNITY_XR_WINDOWSMR_METRO && HOLOLENS
using Juniper.Input;

using UnityEngine.XR.WSA.Input;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Unity.Input.Pointers.Motion
{
    public class HoloLensHandConfiguration : AbstractHandTrackerConfiguration<InteractionSourceHandedness, WindowsMRButtons>
    {
        public HoloLensHandConfiguration()
        {
            AddButton(WindowsMRButtons.AirTap, InputButton.Left);
        }

        public override InteractionSourceHandedness? this[Hands hand]
        {
            get
            {
                if (hand == Hands.Right)
                {
                    return InteractionSourceHandedness.Unknown;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}

#endif