using System;
using System.Collections.Generic;

using Juniper.Haptics;

using UnityEngine;
using UnityEngine.EventSystems;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Input.Pointers.Motion
{
    public abstract class AbstractMotionControllerConfiguration<HandIDType, ButtonIDType>
        : AbstractHandedPointerConfiguration<HandIDType, ButtonIDType>
        where HandIDType : struct, IComparable
        where ButtonIDType : struct
    {
        protected override string PointerNameStub => "MotionController";

        private readonly Dictionary<VirtualTouchPadButton, InputButton> touchPadButtons = new Dictionary<VirtualTouchPadButton, InputButton>();
        private readonly Dictionary<VirtualTriggerButton, InputButton> triggerButtons = new Dictionary<VirtualTriggerButton, InputButton>();

        protected void AddButton(VirtualTouchPadButton outButton, InputButton inButton) =>
            touchPadButtons.Add(outButton, inButton);

        protected void AddButton(VirtualTriggerButton outButton, InputButton inButton) =>
            triggerButtons.Add(outButton, inButton);

        public void Install(ButtonMapper<VirtualTouchPadButton> mapper, GameObject eventParent) =>
            mapper.Install(eventParent, touchPadButtons);

        public void Install(ButtonMapper<VirtualTriggerButton> mapper, GameObject eventParent) =>
            mapper.Install(eventParent, triggerButtons);
    }

    public abstract class AbstractMotionController<HandIDType, ButtonIDType, ConfigType, HapticsType> :
        AbstractHandedPointer<HandIDType, ButtonIDType, ConfigType, HapticsType>,
        IMotionController
        where HandIDType : struct, IComparable
        where ButtonIDType : struct
        where ConfigType : AbstractMotionControllerConfiguration<HandIDType, ButtonIDType>, new()
        where HapticsType : AbstractHapticDevice
    {
        private const float BUTTON_RADIUS = 0.25f;
        private const float DIM = 0.75f;

        private static readonly Dictionary<VirtualTouchPadButton, Vector2> BUTTON_CENTERS = new Dictionary<VirtualTouchPadButton, Vector2>
        {
            { VirtualTouchPadButton.Center, new Vector2(0, 0) },

            { VirtualTouchPadButton.Top, new Vector2(0, DIM) },
            { VirtualTouchPadButton.Right, new Vector2(DIM, 0) },
            { VirtualTouchPadButton.Bottom, new Vector2(0, -DIM) },
            { VirtualTouchPadButton.Left, new Vector2(-DIM, 0) },

            { VirtualTouchPadButton.TopLeft, new Vector2(DIM, -DIM) },
            { VirtualTouchPadButton.TopRight, new Vector2(DIM, DIM) },
            { VirtualTouchPadButton.BottomLeft, new Vector2(-DIM, -DIM) },
            { VirtualTouchPadButton.BottomRight, new Vector2(DIM, -DIM) },
        };

        private static readonly Dictionary<VirtualTriggerButton, float> TRIGGER_THRESHOLDS = new Dictionary<VirtualTriggerButton, float>
        {
            { VirtualTriggerButton.Off, 0.1f },
            { VirtualTriggerButton.On, 0.2f },
            { VirtualTriggerButton.Full, 0.8f }
        };

        protected readonly ButtonMapper<VirtualTouchPadButton> touchPadButtons = new ButtonMapper<VirtualTouchPadButton>();
        protected readonly ButtonMapper<VirtualTriggerButton> triggerButtons = new ButtonMapper<VirtualTriggerButton>();

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
            reset &= Application.isEditor;

            base.Install(reset);

            PointerConfig.Install(touchPadButtons, gameObject);
            PointerConfig.Install(triggerButtons, gameObject);

#if UNITY_EDITOR
            if (LaserPointerMaterial == null)
            {
                LaserPointerMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(
                    System.IO.PathExt.FixPath("Assets/Juniper/Materials/LaserPointer.mat"));
            }
#endif
        }

        private PointerEventData Clone(PointerEventData evtData, VirtualTouchPadButton button) =>
            InputModule?.Clone(evtData, touchPadButtons.ToInt32(button) + 100);

        private PointerEventData Clone(PointerEventData evtData, VirtualTriggerButton button) =>
            InputModule?.Clone(evtData, triggerButtons.ToInt32(button) + 200);

        public abstract float Trigger { get; }

        private float lastTrigger;

        public abstract Vector2 SquareTouchPoint { get; }

        public abstract Vector2 RoundTouchPoint { get; }

        private Vector2? lastTouchPosition;

        public bool IsButtonPressed(VirtualTouchPadButton button) =>
            VirtualButtonInBounds(button) && TouchPadPressed;

        public bool IsButtonDown(VirtualTouchPadButton button) =>
            VirtualButtonInBounds(button) && TouchPadPressedDown;

        public bool IsButtonUp(VirtualTouchPadButton button) =>
            VirtualButtonInBounds(button) && TouchPadPressedUp;

        public bool IsButtonPressed(VirtualTriggerButton button) =>
            VirtualTriggerButtonThreshold(button, Trigger);

        public bool IsButtonDown(VirtualTriggerButton button) =>
            VirtualTriggerButtonThreshold(button, Trigger)
            && !VirtualTriggerButtonThreshold(button, lastTrigger);

        public bool IsButtonUp(VirtualTriggerButton button) =>
            !VirtualTriggerButtonThreshold(button, Trigger)
            && VirtualTriggerButtonThreshold(button, lastTrigger);

        public override bool AnyButtonPressed =>
            base.AnyButtonPressed
            || IsButtonPressed(VirtualTouchPadButton.Any)
            || IsButtonPressed(VirtualTriggerButton.On)
            || IsButtonPressed(VirtualTriggerButton.Full);

        protected abstract bool TouchPadTouched { get; }

        protected abstract bool TouchPadTouchedDown { get; }

        protected abstract bool TouchPadTouchedUp { get; }

        protected abstract bool TouchPadPressed { get; }

        protected abstract bool TouchPadPressedDown { get; }

        protected abstract bool TouchPadPressedUp { get; }

        protected static Vector2 Round2Square(Vector2 point)
        {
            var pLen = point.magnitude;
            if (pLen > 0)
            {
                var onUnitCircle = point / pLen;
                float mx = Mathf.Abs(onUnitCircle.x),
                    my = Mathf.Abs(onUnitCircle.y);

                var onUnitSquare = onUnitCircle;
                if (mx > my)
                {
                    onUnitSquare /= mx;
                }
                else if (my > 0)
                {
                    onUnitSquare /= my;
                }

                point *= onUnitSquare.magnitude;
            }
            return point;
        }

        protected static Vector2 Square2Round(Vector2 point)
        {
            var pLen = point.magnitude;
            if (pLen > 0)
            {
                var onUnitCircle = point / pLen;
                var onUnitSquare = Round2Square(onUnitCircle);
                point /= onUnitSquare.magnitude;
            }
            return point;
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

            if (probe != null)
            {
                probe.TouchPoint = RoundTouchPoint;
            }
        }

        protected override IEventSystemHandler ProcessButtons(PointerEventData evtData, float pixelDragThresholdSquared)
        {
            var a = base.ProcessButtons(evtData, pixelDragThresholdSquared);
            var b = touchPadButtons.Process(evtData, pixelDragThresholdSquared);
            var c = triggerButtons.Process(evtData, pixelDragThresholdSquared);

            return a ?? b ?? c;
        }

        public override bool IsDragging =>
            base.IsDragging
            || touchPadButtons.AnyDragging
            || triggerButtons.AnyDragging;

        public override bool IsButtonPressed(InputButton button) =>
            base.IsButtonPressed(button)
            || touchPadButtons.IsButtonPressed(button)
            || triggerButtons.IsButtonPressed(button);

        public override bool IsButtonUp(InputButton button) =>
            base.IsButtonUp(button)
            || touchPadButtons.IsButtonUp(button)
            || triggerButtons.IsButtonUp(button);

        public override bool IsButtonDown(InputButton button) =>
            base.IsButtonDown(button)
            || touchPadButtons.IsButtonDown(button)
            || triggerButtons.IsButtonDown(button);

        private bool VirtualButtonInBounds(VirtualTouchPadButton button) =>
            button == VirtualTouchPadButton.Any
            || (BUTTON_CENTERS.ContainsKey(button)
                && (SquareTouchPoint - BUTTON_CENTERS[button]).magnitude <= BUTTON_RADIUS);

        private bool VirtualTriggerButtonThreshold(VirtualTriggerButton button, float triggerValue) =>
            TRIGGER_THRESHOLDS.ContainsKey(button)
            && triggerValue > TRIGGER_THRESHOLDS[button];
    }
}
