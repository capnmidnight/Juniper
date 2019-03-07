#if UNITY_XR_WINDOWSMR_METRO && WINDOWSMR
using UnityEngine.XR.WSA.Input;
using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;
using System.Linq;

#if UNITY_EDITOR
using HapticsType = Juniper.Haptics.NoHaptics;
#else
using HapticsType = Juniper.Haptics.WindowsMRHaptics;
#endif

namespace Juniper.Unity.Input.Pointers.Motion
{
    public class WindowsMRProbeConfiguration : AbstractProbeNameConfiguration<InteractionSourceHandedness>
    {
        public WindowsMRProbeConfiguration() :
            base(InteractionSourceHandedness.Left, InteractionSourceHandedness.Right) { }
    }

    /// <summary>
    /// A motion controller or hand-tracking. Currently only implements WindowsMR.
    /// </summary>
    public abstract class WindowsMRMotionController : AbstractWindowsMRDevice<WindowsMRProbeConfiguration, HapticsType>
    {
        private uint ControllerID;

        public override Hands Hand
        {
            get
            {
                return base.Hand;
            }

            set
            {
                base.Hand = value;
                ControllerID = (from state in InteractionManager.GetCurrentReading()
                                where state.source.handedness == NativeHandID
                                select state.source.id)
                            .FirstOrDefault();
            }
        }

        public override void Awake()
        {
            base.Awake();

#if !UNITY_EDITOR
            Haptics.ControllerID = ControllerID;
#endif

            AddButton(WindowsMRButtons.Select, InputButton.Left);
            AddButton(WindowsMRButtons.Touchpad, InputButton.Right);
            AddButton(WindowsMRButtons.App, InputButton.Middle);
        }
    }
}
#endif
