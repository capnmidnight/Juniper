using System;
using System.Collections.Generic;

using Juniper.Events;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Juniper.Input.Pointers
{
    public class ButtonMapper<ButtonIDType>
        where ButtonIDType : struct
    {
        public event Action<Interaction> InteractionNeeded;

        public event Func<ButtonIDType, bool> ButtonDownNeeded;

        public event Func<ButtonIDType, bool> ButtonUpNeeded;

        public event Func<ButtonIDType, bool> ButtonPressedNeeded;

        public event Func<int, JuniperPointerEventData, JuniperPointerEventData> ClonedPointerEventNeeded;

        private readonly List<MappedButton<ButtonIDType>> buttons = new List<MappedButton<ButtonIDType>>(5);

        public bool IsDragging
        {
            get
            {
                for(int i = 0; i < buttons.Count; ++i)
                {
                    var btn = buttons[i];
                    if(btn.IsDown && !btn.IsDown)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public void Install(GameObject eventParent, Dictionary<ButtonIDType, KeyCode> keyMapping, bool reset)
        {
            buttons.Clear();

            if (reset)
            {
                foreach(var pair in keyMapping)
                {
                    AddButton(new MappedButton<ButtonIDType>(pair.Key, pair.Value, eventParent));
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
            else
            {
                var buttonIDType = typeof(ButtonIDType);
                var buttonTypeName = buttonIDType.FullName;
                var existing = eventParent.GetComponentsInChildren<ButtonEvent>();
                foreach(var evt in existing)
                {
                    var seperatorIndex = evt.Key.IndexOf("::");
                    var typeName = evt.Key.Substring(0, seperatorIndex);
                    var valueName = evt.Key.Substring(seperatorIndex + 2);
                    if(typeName == buttonTypeName)
                    {
                        var value = (ButtonIDType)Enum.Parse(buttonIDType, valueName);
                        AddButton(new MappedButton<ButtonIDType>(value, evt));
                    }
                }
            }
        }

        private void AddButton(MappedButton<ButtonIDType> btn)
        {
            btn.ButtonDownNeeded += OnButtonDownNeeded;
            btn.ButtonUpNeeded += OnButtonUpNeeded;
            btn.ButtonPressedNeeded += OnButtonPressedNeeded;
            btn.ClonedPointerEventNeeded += OnClonedPointerEventNeeded;
            btn.InteractionNeeded += OnInteractionNeeded;
            buttons.Add(btn);
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

        private JuniperPointerEventData OnClonedPointerEventNeeded(int pointerID, JuniperPointerEventData donor)
        {
            return ClonedPointerEventNeeded?.Invoke(pointerID, donor);
        }

        private void OnInteractionNeeded(Interaction action)
        {
            InteractionNeeded?.Invoke(action);
        }

        private MappedButton<ButtonIDType> FindMappedButton(KeyCode inputKey)
        {
            foreach (var button in buttons)
            {
                if (button.UnityKeyCode == inputKey)
                {
                    return button;
                }
            }
            return null;
        }

        public bool IsButtonDown(KeyCode key)
        {
            return FindMappedButton(key)?.IsDown == true;
        }

        public bool IsButtonUp(KeyCode key)
        {
            return FindMappedButton(key)?.IsUp == true;
        }

        public bool IsButtonPressed(KeyCode key)
        {
            return FindMappedButton(key)?.IsPressed == true;
        }

        public IEventSystemHandler Process(JuniperPointerEventData eventData, float pixelDragThresholdSquared, List<KeyCode> keyPresses)
        {
            IEventSystemHandler eventTarget = null;
            foreach (var btn in buttons)
            {
                eventTarget = btn.Process(eventData, pixelDragThresholdSquared, keyPresses);
            }
            return eventTarget;
        }
    }
}
