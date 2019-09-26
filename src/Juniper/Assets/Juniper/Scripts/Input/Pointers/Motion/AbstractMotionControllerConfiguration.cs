using System;
using System.Collections.Generic;

using UnityEngine;

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

        private readonly Dictionary<VirtualTouchPadButton, KeyCode> touchPadKeys = new Dictionary<VirtualTouchPadButton, KeyCode>(11);

        protected void AddButton(VirtualTouchPadButton outButton, KeyCode inKey)
        {
            touchPadKeys.Add(outButton, inKey);
        }

        private readonly Dictionary<VirtualTriggerButton, KeyCode> triggerKeys = new Dictionary<VirtualTriggerButton, KeyCode>(3);

        protected void AddButton(VirtualTriggerButton outButton, KeyCode inKey)
        {
            triggerKeys.Add(outButton, inKey);
        }

        public void Install(ButtonMapper<VirtualTouchPadButton> mapper, GameObject eventParent, bool reset)
        {
            mapper.Install(eventParent, touchPadKeys, reset);
        }

        public void Install(ButtonMapper<VirtualTriggerButton> mapper, GameObject eventParent, bool reset)
        {
            mapper.Install(eventParent, triggerKeys, reset);
        }
    }
}
