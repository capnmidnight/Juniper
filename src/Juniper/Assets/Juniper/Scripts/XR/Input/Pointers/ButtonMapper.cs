using System;
using System.Collections.Generic;
using System.Linq;

using Juniper.Input;
using Juniper.Events;

using UnityEngine;
using UnityEngine.EventSystems;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Input.Pointers
{
    public class ButtonMapper<ButtonIDType>
        where ButtonIDType : struct
    {
        public event Action<Interaction> InteractionNeeded;

        public event Func<ButtonIDType, bool> ButtonDownNeeded;

        public event Func<ButtonIDType, bool> ButtonUpNeeded;

        public event Func<ButtonIDType, bool> ButtonPressedNeeded;

        public event Func<int, PointerEventData, PointerEventData> ClonedPointerEventNeeded;

        private readonly List<MappedButton<ButtonIDType>> buttons = new List<MappedButton<ButtonIDType>>(5);

        public bool IsDragging
        {
            get
            {
                return buttons.Any((btn) => btn.IsPressed && !btn.IsDown);
            }
        }

        public void Install(GameObject eventParent, Dictionary<ButtonIDType, InputEventButton> buttonMapping)
        {
            buttons.Clear();

            foreach (var pair in buttonMapping)
            {
                AddButton(eventParent, pair.Key, pair.Value);
            }

            foreach (var evt in eventParent.GetComponents<ButtonEvent>())
            {
                if (string.IsNullOrEmpty(evt.buttonTypeName)
                    || string.IsNullOrEmpty(evt.buttonValueName))
                {
                    evt.DestroyImmediate();
                }
            }
        }

        public void AddButton(GameObject eventParent, ButtonIDType buttonID, InputEventButton buttonValue)
        {
            var btn = new MappedButton<ButtonIDType>(buttonID, buttonValue, eventParent);
            btn.ButtonDownNeeded += OnButtonDownNeeded;
            btn.ButtonUpNeeded += OnButtonUpNeeded;
            btn.ButtonPressedNeeded += OnButtonPressedNeeded;
            btn.ClonedPointerEventNeeded += OnClonedPointerEventNeeded;
            btn.InteractionNeeded += OnInteractionNeeded;
            buttons.Add(btn);
        }

        public void RemoveButton(ButtonIDType buttonID)
        {
            var btn = buttons.FirstOrDefault(b => b.button.Equals(buttonID));
            if (btn != null)
            {
                buttons.Remove(btn);
                btn.ButtonDownNeeded -= OnButtonDownNeeded;
                btn.ButtonUpNeeded -= OnButtonUpNeeded;
                btn.ButtonPressedNeeded -= OnButtonPressedNeeded;
                btn.ClonedPointerEventNeeded += OnClonedPointerEventNeeded;
                btn.InteractionNeeded -= OnInteractionNeeded;
                btn.Destroy();
            }
        }

        private bool OnButtonDownNeeded(ButtonIDType button)
        {
            return ButtonDownNeeded?.Invoke(button) == true;
        }

        private bool OnButtonUpNeeded(ButtonIDType button)
        {
            return ButtonUpNeeded?.Invoke(button) == true;
        }

        private bool OnButtonPressedNeeded(ButtonIDType button)
        {
            return ButtonPressedNeeded?.Invoke(button) == true;
        }

        private PointerEventData OnClonedPointerEventNeeded(int pointerID, PointerEventData donor)
        {
            return ClonedPointerEventNeeded?.Invoke(pointerID, donor);
        }

        private void OnInteractionNeeded(Interaction action)
        {
            InteractionNeeded?.Invoke(action);
        }

        private MappedButton<ButtonIDType> FindMappedButton(InputButton inputBtn)
        {
            foreach (var button in buttons)
            {
                if (button.UnityInputButton == inputBtn)
                {
                    return button;
                }
            }
            return null;
        }

        public bool IsButtonDown(InputButton button)
        {
            return FindMappedButton(button)?.IsDown == true;
        }

        public bool IsButtonUp(InputButton button)
        {
            return FindMappedButton(button)?.IsUp == true;
        }

        public bool IsButtonPressed(InputButton button)
        {
            return FindMappedButton(button)?.IsPressed == true;
        }

        public IEventSystemHandler Process(PointerEventData eventData, float pixelDragThresholdSquared)
        {
            IEventSystemHandler eventTarget = null;
            foreach (var btn in buttons)
            {
                eventTarget = btn.Process(eventData, pixelDragThresholdSquared);
            }
            return eventTarget;
        }
    }
}
