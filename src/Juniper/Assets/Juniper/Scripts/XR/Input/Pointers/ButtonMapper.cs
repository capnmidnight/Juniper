using Juniper.Input;
using Juniper.Unity.Events;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Unity.Input.Pointers
{
    public class ButtonMapper<ButtonIDType>
        where ButtonIDType : struct
    {
        public event Action<Interaction> InteractionNeeded;
        
        public event Func<ButtonIDType, bool> ButtonDownNeeded;

        public event Func<ButtonIDType, bool> ButtonUpNeeded;

        public event Func<ButtonIDType, bool> ButtonPressedNeeded;

        public event Func<int, PointerEventData, PointerEventData> ClonedPointerEventNeeded;

        private readonly List<MappedButton<ButtonIDType>> buttons = new List<MappedButton<ButtonIDType>>();

        public bool IsDragging
        {
            get { return buttons.Any((btn) => btn.IsDragging); }
        }

        public void Install(GameObject pointer, Dictionary<ButtonIDType, InputEventButton> buttonMapping)
        {
            buttons.Clear();

            foreach (var pair in buttonMapping)
            {
                var btn = new MappedButton<ButtonIDType>(pair.Key, pair.Value, pointer);
                btn.ButtonDownNeeded += OnButtonDownNeeded;
                btn.ButtonUpNeeded += OnButtonUpNeeded;
                btn.ButtonPressedNeeded += OnButtonPressedNeeded;
                btn.ClonedPointerEventNeeded += OnClonedPointerEventNeeded;
                btn.InteractionNeeded += InteractionNeeded;
                buttons.Add(btn);
            }

            foreach (var evt in pointer.GetComponents<ButtonEvent>())
            {
                if (string.IsNullOrEmpty(evt.buttonTypeName)
                    || string.IsNullOrEmpty(evt.buttonValueName))
                {
                    evt.Destroy();
                }
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

        private MappedButton<ButtonIDType> FindMappedButton(InputButton inputBtn)
        {
            foreach(var button in buttons)
            {
                if(button.UnityInputButton == inputBtn)
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
            foreach(var btn in buttons)
            {
                eventTarget = btn.Process(eventData, pixelDragThresholdSquared);
            }
            return eventTarget;
        }
    }
}
