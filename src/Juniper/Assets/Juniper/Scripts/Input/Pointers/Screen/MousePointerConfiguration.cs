using UnityEngine;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Input.Pointers.Screen
{
    public class MousePointerConfiguration : AbstractPointerConfiguration<KeyCode>
    {
        public MousePointerConfiguration()
        {
            AddButton(KeyCode.Mouse0, InputButton.Left);
            AddButton(KeyCode.Mouse1, InputButton.Right);
            AddButton(KeyCode.Mouse2, InputButton.Middle);
        }
    }
}
