using UnityEngine;

namespace Juniper.Input.Pointers.Screen
{
    public class MousePointerConfiguration : AbstractPointerConfiguration<MouseButton>
    {
        public MousePointerConfiguration()
        {
            AddButton(MouseButton.Mouse0, KeyCode.Mouse0);
            AddButton(MouseButton.Mouse1, KeyCode.Mouse1);
            AddButton(MouseButton.Mouse2, KeyCode.Mouse2);
            AddButton(MouseButton.Mouse3, KeyCode.Mouse3);
            AddButton(MouseButton.Mouse4, KeyCode.Mouse4);
            AddButton(MouseButton.Mouse5, KeyCode.Mouse5);
            AddButton(MouseButton.Mouse6, KeyCode.Mouse6);
        }
    }
}
