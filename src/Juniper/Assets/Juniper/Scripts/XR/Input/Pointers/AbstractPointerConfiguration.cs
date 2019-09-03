using System.Collections.Generic;

using Juniper.Input;
using Juniper.Events;

using UnityEngine;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Input.Pointers
{
    public abstract class AbstractPointerConfiguration<ButtonIDType>
        where ButtonIDType : struct
    {
        private readonly Dictionary<ButtonIDType, InputEventButton> nativeButtons = new Dictionary<ButtonIDType, InputEventButton>(5);

        protected void AddButton(ButtonIDType outButton, InputButton? inButton = null)
        {
            nativeButtons.Add(outButton, inButton == null ? InputEventButton.None : (InputEventButton)inButton.Value);
        }

        public void Install(ButtonMapper<ButtonIDType> mapper, GameObject eventParent)
        {
            mapper.Install(eventParent, nativeButtons);
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
