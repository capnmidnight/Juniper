using System;
using System.Collections.Generic;
using Juniper.Haptics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Juniper.Input.Pointers.Motion
{
    public class MotionController :
#if UNITY_XR_OCULUS
        OculusMotionController
#elif UNITY_XR_GOOGLEVR_ANDROID
        DaydreamMotionController
#elif WAVEVR
        ViveFocusMotionController
#elif PICO
        PicoMotionController
#elif UNITY_XR_WINDOWSMR_METRO
        WindowsMRMotionController
#elif UNITY_XR_MAGICLEAP
        MagicLeapMotionController
#else
        NoMotionController
#endif
    {
        [ContextMenu("Reinstall")]
        public override void Reinstall()
        {
            base.Reinstall();
        }
    }

    public abstract class AbstractMotionController<HandIDType, ButtonIDType, ConfigType, HapticType> :
        AbstractHandedPointer<HandIDType, ButtonIDType, ConfigType, HapticType>,
        IMotionController
        where HandIDType : struct, IComparable
        where ButtonIDType : struct
        where ConfigType : AbstractMotionControllerConfiguration<HandIDType, ButtonIDType>, new()
        where HapticType : AbstractHapticDevice
    {
        private const float BUTTON_RADIUS = 0.25f;
        private const float DIM = 0.75f;

        private static readonly Dictionary<VirtualTouchPadButtons, Vector2> BUTTON_CENTERS = new Dictionary<VirtualTouchPadButtons, Vector2>(9)
        {
            { VirtualTouchPadButtons.Center, new Vector2(0, 0) },

            { VirtualTouchPadButtons.Top, new Vector2(0, DIM) },
            { VirtualTouchPadButtons.Right, new Vector2(DIM, 0) },
            { VirtualTouchPadButtons.Bottom, new Vector2(0, -DIM) },
            { VirtualTouchPadButtons.Left, new Vector2(-DIM, 0) },

            { VirtualTouchPadButtons.TopLeft, new Vector2(DIM, -DIM) },
            { VirtualTouchPadButtons.TopRight, new Vector2(DIM, DIM) },
            { VirtualTouchPadButtons.BottomLeft, new Vector2(-DIM, -DIM) },
            { VirtualTouchPadButtons.BottomRight, new Vector2(DIM, -DIM) },
        };

        private static readonly Dictionary<VirtualTriggerButton, float> TRIGGER_THRESHOLDS = new Dictionary<VirtualTriggerButton, float>(3)
        {
            { VirtualTriggerButton.Off, 0.1f },
            { VirtualTriggerButton.On, 0.2f },
            { VirtualTriggerButton.Full, 0.8f }
        };

        protected readonly ButtonMapper<VirtualTouchPadButtons> touchPadButtons = new ButtonMapper<VirtualTouchPadButtons>();
        protected readonly ButtonMapper<VirtualTriggerButton> triggerButtons = new ButtonMapper<VirtualTriggerButton>();

        public override bool IsDragging
        {
            get
            {
                return base.IsDragging
                    || EventTarget != null && touchPadButtons.IsDragging
                    || EventTarget != null && triggerButtons.IsDragging;
            }
        }

        public override void Awake()
        {
            base.Awake();

            touchPadButtons.ButtonDownNeeded += IsButtonDown;
            touchPadButtons.ButtonUpNeeded += IsButtonUp;
            touchPadButtons.ButtonPressedNeeded += IsButtonPressed;
            touchPadButtons.ClonedPointerEventNeeded += Clone;
            touchPadButtons.InteractionNeeded += PlayInteraction;

            triggerButtons.ButtonDownNeeded += IsButtonDown;
            triggerButtons.ButtonUpNeeded += IsButtonUp;
            triggerButtons.ButtonPressedNeeded += IsButtonPressed;
            triggerButtons.ClonedPointerEventNeeded += Clone;
            triggerButtons.InteractionNeeded += PlayInteraction;
        }

        public override void Install(bool reset)
        {
            base.Install(reset);

            PointerConfig.Install(touchPadButtons, gameObject, reset);
            PointerConfig.Install(triggerButtons, gameObject, reset);

#if UNITY_EDITOR
            if (LaserPointerMaterial == null)
            {
                LaserPointerMaterial = ResourceExt.EditorLoadAsset<Material>("Assets/Juniper/Materials/LaserPointer.mat");
            }
#endif
        }

        public abstract float Trigger
        {
            get;
        }

        private float lastTrigger;

        public abstract Vector2 SquareTouchPoint
        {
            get;
        }

        public abstract Vector2 RoundTouchPoint
        {
            get;
        }

        private Vector2? lastTouchPosition;

        public override bool IsButtonPressed(VirtualTouchPadButtons button)
        {
            return VirtualButtonInBounds(button) && TouchPadPressed;
        }

        public override bool IsButtonDown(VirtualTouchPadButtons button)
        {
            return VirtualButtonInBounds(button) && TouchPadPressedDown;
        }

        public override bool IsButtonUp(VirtualTouchPadButtons button)
        {
            return VirtualButtonInBounds(button) && TouchPadPressedUp;
        }

        public override bool IsButtonPressed(VirtualTriggerButton button)
        {
            return VirtualTriggerButtonThreshold(button, Trigger);
        }

        public override bool IsButtonDown(VirtualTriggerButton button)
        {
            return VirtualTriggerButtonThreshold(button, Trigger)
                && !VirtualTriggerButtonThreshold(button, lastTrigger);
        }

        public override bool IsButtonUp(VirtualTriggerButton button)
        {
            return !VirtualTriggerButtonThreshold(button, Trigger)
                && VirtualTriggerButtonThreshold(button, lastTrigger);
        }

        public override bool AnyButtonPressed
        {
            get
            {
                return base.AnyButtonPressed
                    || IsButtonPressed(VirtualTouchPadButtons.Any)
                    || IsButtonPressed(VirtualTriggerButton.On)
                    || IsButtonPressed(VirtualTriggerButton.Full);
            }
        }

        protected abstract bool TouchPadTouched
        {
            get;
        }

        protected abstract bool TouchPadTouchedDown
        {
            get;
        }

        protected abstract bool TouchPadTouchedUp
        {
            get;
        }

        protected abstract bool TouchPadPressed
        {
            get;
        }

        protected abstract bool TouchPadPressedDown
        {
            get;
        }

        protected abstract bool TouchPadPressedUp
        {
            get;
        }

        protected override void InternalUpdate()
        {
            if (TouchPadTouched)
            {
                var touchPos = SquareTouchPoint;
                if (lastTouchPosition != null)
                {
                    ScrollDelta = touchPos - lastTouchPosition.Value;
                }
                lastTouchPosition = touchPos;
            }
            else
            {
                ScrollDelta = Vector2.zero;
                lastTouchPosition = null;
            }

            lastTrigger = Trigger;

            if (Probe != null)
            {
                Probe.TouchPoint = RoundTouchPoint;
            }
        }

        protected override IEventSystemHandler ProcessButtons(JuniperPointerEventData evtData, float pixelDragThresholdSquared, List<KeyCode> keyPresses)
        {
            var a = base.ProcessButtons(evtData, pixelDragThresholdSquared, keyPresses);
            var b = touchPadButtons.Process(evtData, pixelDragThresholdSquared, keyPresses);
            var c = triggerButtons.Process(evtData, pixelDragThresholdSquared, keyPresses);

            return a ?? b ?? c;
        }

        public override bool IsButtonPressed(KeyCode key)
        {
            return base.IsButtonPressed(key)
                || touchPadButtons.IsButtonPressed(key)
                || triggerButtons.IsButtonPressed(key);
        }

        public override bool IsButtonUp(KeyCode key)
        {
            return base.IsButtonUp(key)
                || touchPadButtons.IsButtonUp(key)
                || triggerButtons.IsButtonUp(key);
        }

        public override bool IsButtonDown(KeyCode key)
        {
            return base.IsButtonDown(key)
                || touchPadButtons.IsButtonDown(key)
                || triggerButtons.IsButtonDown(key);
        }

        private bool VirtualButtonInBounds(VirtualTouchPadButtons button)
        {
            return button == VirtualTouchPadButtons.Any
                || (BUTTON_CENTERS.ContainsKey(button)
                    && (SquareTouchPoint - BUTTON_CENTERS[button]).magnitude <= BUTTON_RADIUS);
        }

        private bool VirtualTriggerButtonThreshold(VirtualTriggerButton button, float triggerValue)
        {
            return TRIGGER_THRESHOLDS.ContainsKey(button)
                && triggerValue > TRIGGER_THRESHOLDS[button];
        }
    }
}
