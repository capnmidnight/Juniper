using System;
using System.Collections.Generic;

using Juniper.Input;

using UnityEngine;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Input.Pointers.Motion
{
    public abstract class AbstractMotionControllerConfiguration<HandIDType, ButtonIDType>
        : AbstractHandedPointerConfiguration<HandIDType, ButtonIDType>
        where HandIDType : struct, IComparable
        where ButtonIDType : struct
    {
        protected override string PointerNameStub
        {
            get
            {
                return "MotionController";
            }
        }

        private readonly Dictionary<VirtualTouchPadButton, InputEventButton> touchPadButtons = new Dictionary<VirtualTouchPadButton, InputEventButton>(11);
        private readonly Dictionary<VirtualTriggerButton, InputEventButton> triggerButtons = new Dictionary<VirtualTriggerButton, InputEventButton>(3);

        protected void AddButton(VirtualTouchPadButton outButton, InputButton inButton)
        {
            touchPadButtons.Add(outButton, (InputEventButton)inButton);
        }

        protected void AddButton(VirtualTouchPadButton outButton)
        {
            touchPadButtons.Add(outButton, InputEventButton.None);
        }

        protected void AddButton(VirtualTriggerButton outButton, InputButton inButton)
        {
            triggerButtons.Add(outButton, (InputEventButton)inButton);
        }

        protected void AddButton(VirtualTriggerButton outButton)
        {
            triggerButtons.Add(outButton, InputEventButton.None);
        }

        public void Install(ButtonMapper<VirtualTouchPadButton> mapper, GameObject eventParent, bool reset)
        {
            mapper.Install(eventParent, touchPadButtons, reset);
        }

        public void Install(ButtonMapper<VirtualTriggerButton> mapper, GameObject eventParent, bool reset)
        {
            mapper.Install(eventParent, triggerButtons, reset);
        }
    }
}
