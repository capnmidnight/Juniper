using System;
using System.Collections.Generic;

using Juniper.Input;

using UnityEngine;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Unity.Input.Pointers.Motion
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

        protected void AddButton(VirtualTouchPadButton outButton, InputButton? inButton = null)
        {
            touchPadButtons.Add(outButton, inButton == null ? InputEventButton.None : (InputEventButton)inButton.Value);
        }

        protected void AddButton(VirtualTriggerButton outButton, InputButton? inButton = null)
        {
            triggerButtons.Add(outButton, inButton == null ? InputEventButton.None : (InputEventButton)inButton.Value);
        }

        public void Install(ButtonMapper<VirtualTouchPadButton> mapper, GameObject eventParent)
        {
            mapper.Install(eventParent, touchPadButtons);
        }

        public void Install(ButtonMapper<VirtualTriggerButton> mapper, GameObject eventParent)
        {
            mapper.Install(eventParent, triggerButtons);
        }
    }
}
