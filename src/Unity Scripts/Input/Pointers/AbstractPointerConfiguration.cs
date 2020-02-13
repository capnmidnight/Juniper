using System.Collections.Generic;

using Juniper.Events;

using UnityEngine;

namespace Juniper.Input.Pointers
{
    public abstract class AbstractPointerConfiguration<ButtonIDType>
        where ButtonIDType : struct
    {
        private readonly Dictionary<ButtonIDType, KeyCode> nativeKeys = new Dictionary<ButtonIDType, KeyCode>(5);

        protected void AddButton(ButtonIDType outButton, KeyCode inKey)
        {
            nativeKeys.Add(outButton, inKey);
        }

        public void Install(ButtonMapper<ButtonIDType> mapper, GameObject eventParent, bool reset)
        {
            mapper.Install(eventParent, nativeKeys, reset);
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
