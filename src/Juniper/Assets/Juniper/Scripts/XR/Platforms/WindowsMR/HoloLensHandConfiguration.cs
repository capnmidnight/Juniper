#if UNITY_XR_WINDOWSMR_METRO && HOLOLENS
using UnityEngine;
using UnityEngine.XR.WSA.Input;
using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Unity.Input.Pointers.Motion
{
    public class HoloLensHandConfiguration : AbstractHandTrackerConfiguration<InteractionSourceHandedness>
    {
        public HoloLensHandConfiguration() :
            base(InteractionSourceHandedness.Unknown, InteractionSourceHandedness.Unknown) { }
    }
}

#endif