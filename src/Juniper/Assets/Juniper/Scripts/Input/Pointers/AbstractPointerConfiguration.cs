using System.Collections.Generic;

using Juniper.Events;

using UnityEngine;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Input.Pointers
{
    public abstract class AbstractPointerConfiguration<ButtonIDType>
        where ButtonIDType : struct
    {
        private readonly Dictionary<ButtonIDType, InputEventButton> nativeButtons = new Dictionary<ButtonIDType, InputEventButton>(5);

        protected void AddButton(ButtonIDType outButton, InputButton inButton)
        {
            nativeButtons.Add(outButton, (InputEventButton)inButton);
        }


        protected void AddButton(ButtonIDType outButton)
        {
            nativeButtons.Add(outButton, InputEventButton.None);
        }

        public void Install(ButtonMapper<ButtonIDType> mapper, GameObject eventParent, bool reset)
        {
            mapper.Install(eventParent, nativeButtons, reset);
        }

        public void Uninstall(GameObject eventParent)
        {
            foreach (var evt in eventParent.GetComponents<ButtonEvent>())
            {
                evt.DestroyImmediate();
            }
        }
    }
}
