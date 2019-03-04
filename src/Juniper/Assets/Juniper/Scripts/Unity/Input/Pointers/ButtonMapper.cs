using Juniper.Events;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Input.Pointers
{
    public class ButtonMapper<ButtonIDType>
        where ButtonIDType : struct
    {
        private const float THRESHOLD_CLICK = 0.4f;
        private const float THRESHOLD_LONG_PRESS = 2f;

        private readonly Dictionary<ButtonIDType, InputButton> toInputButton = new Dictionary<ButtonIDType, InputButton>();
        private readonly Dictionary<InputButton, ButtonIDType> fromInputButton = new Dictionary<InputButton, ButtonIDType>();
        private readonly Dictionary<ButtonIDType, int> toInteger = new Dictionary<ButtonIDType, int>();
        private readonly Dictionary<ButtonIDType, float> buttonDownTime = new Dictionary<ButtonIDType, float>();
        private readonly Dictionary<ButtonIDType, bool> mayLongPress = new Dictionary<ButtonIDType, bool>();
        private readonly Dictionary<InputButton, float> dragDistances = new Dictionary<InputButton, float>();

        public event Func<ButtonIDType, bool> ButtonDownNeeded;

        public event Func<ButtonIDType, bool> ButtonUpNeeded;

        public event Func<ButtonIDType, bool> ButtonPressedNeeded;

        public event Func<PointerEventData, ButtonIDType, PointerEventData> ClonedPointerEventNeeded;

        public event Action<Interaction> InteractionNeeded;

        public readonly Dictionary<ButtonIDType, ButtonEvent> buttonEvents = new Dictionary<ButtonIDType, ButtonEvent>();

        public readonly Dictionary<InputButton, ButtonEvent> inputButtonEvents = new Dictionary<InputButton, ButtonEvent>();

        public IEnumerable<ButtonIDType> Buttons
        {
            get
            {
                return toInputButton.Keys;
            }
        }

        public bool AnyDragging
        {
            get; private set;
        }

        public void Install(GameObject pointer, Dictionary<ButtonIDType, InputButton> buttons)
        {
            buttonEvents.Clear();
            inputButtonEvents.Clear();

            foreach (var pair in buttons)
            {
                MakeEventHandler(pointer, pair.Key);
                MakeEventHandler(pointer, pair.Value);

                toInputButton.Add(pair.Key, pair.Value);
                toInteger.Add(pair.Key, Convert.ToInt32(pair.Key));
            }

            foreach (var evt in pointer.GetComponents<ButtonEvent>())
            {
                if (string.IsNullOrEmpty(evt.buttonTypeName)
                    || string.IsNullOrEmpty(evt.buttonValueName))
                {
                    evt.Destroy();
                }
            }

            GetEventHandlers(pointer, buttonEvents);
            GetEventHandlers(pointer, inputButtonEvents);
        }

        private static void MakeEventHandler<T>(GameObject pointer, T button)
            where T : struct
        {
            var key = ButtonEvent.MakeKey(button);
            var evt = Array.Find(pointer.GetComponents<ButtonEvent>(), e => e.Key == key)
                ?? pointer.AddComponent<ButtonEvent>();

            evt.Key = key;
        }

        private static void GetEventHandlers<T>(GameObject pointer, Dictionary<T, ButtonEvent> storage)
            where T : struct
        {
            var type = typeof(T);
            var events = pointer.GetComponents<ButtonEvent>()
                .Where(e => e.buttonTypeName == type.FullName);

            foreach (var evt in events)
            {
                var value = evt.GetButtonValue<T>();
                if (value != null)
                {
                    storage.Add(value.Value, evt);
                }
            }
        }

        public bool IsButtonDown(InputButton button)
        {
            return fromInputButton.ContainsKey(button)
                && ButtonDownNeeded?.Invoke(fromInputButton[button]) == true;
        }

        public bool IsButtonUp(InputButton button)
        {
            return fromInputButton.ContainsKey(button)
                && ButtonUpNeeded?.Invoke(fromInputButton[button]) == true;
        }

        public bool IsButtonPressed(InputButton button)
        {
            return fromInputButton.ContainsKey(button)
                && ButtonPressedNeeded?.Invoke(fromInputButton[button]) == true;
        }

        public int ToInt32(ButtonIDType button)
        {
            return toInteger.Get(button);
        }

        public IEventSystemHandler Process(PointerEventData eventData, float pixelDragThresholdSquared)
        {
            IEventSystemHandler eventTarget = null;
            AnyDragging = false;
            foreach (var button in Buttons)
            {
                var pressed = ButtonPressedNeeded?.Invoke(button) == true;
                var evtData = ClonedPointerEventNeeded?.Invoke(eventData, button) ?? eventData;
                evtData.button = toInputButton[button];

                TestUpDown(evtData, button, pressed);
                TestDrag(evtData, pressed, pixelDragThresholdSquared);

                eventTarget = evtData.pointerEnter?.GetComponent<IEventSystemHandler>() ?? eventTarget;
            }

            return eventTarget;
        }

        private void TestUpDown(PointerEventData evtData, ButtonIDType button, bool pressed)
        {
            var up = ButtonUpNeeded?.Invoke(button) == true;
            var down = ButtonDownNeeded?.Invoke(button) == true;
            if (down)
            {
                mayLongPress[button] = true;
                buttonDownTime[button] = Time.time;
                dragDistances[evtData.button] = 0;
                evtData.rawPointerPress = evtData.pointerEnter;
                evtData.pressPosition = evtData.position;
                evtData.pointerPressRaycast = evtData.pointerCurrentRaycast;
                evtData.eligibleForClick = true;

                evtData.pointerDrag = ExecuteEvents.ExecuteHierarchy(evtData.pointerEnter, evtData, ExecuteEvents.initializePotentialDrag);
                evtData.pointerPress = ExecuteEvents.ExecuteHierarchy(evtData.pointerEnter, evtData, ExecuteEvents.pointerDownHandler);
                buttonEvents[button].OnDown(evtData);
                inputButtonEvents[evtData.button]?.OnDown(evtData);
                InteractionNeeded?.Invoke(Interaction.Pressed);
            }

            var deltaTime = buttonDownTime.ContainsKey(button)
                ? Time.time - buttonDownTime[button]
                : 0;

            evtData.eligibleForClick = buttonDownTime.ContainsKey(button)
                && deltaTime < THRESHOLD_CLICK;

            if (up)
            {
                ExecuteEvents.ExecuteHierarchy(evtData.pointerPress, evtData, ExecuteEvents.pointerUpHandler);
                buttonEvents[button].OnUp(evtData);
                inputButtonEvents[evtData.button]?.OnUp(evtData);
                InteractionNeeded?.Invoke(Interaction.Released);

                var target = evtData.pointerCurrentRaycast.gameObject;
                if (evtData.eligibleForClick)
                {
                    ++evtData.clickCount;
                    evtData.clickTime = Time.time;

                    evtData.selectedObject = ExecuteEvents.ExecuteHierarchy(evtData.pointerPress, evtData, ExecuteEvents.pointerClickHandler);
                    buttonEvents[button].OnClick(evtData);
                    inputButtonEvents[evtData.button]?.OnClick(evtData);
                    InteractionNeeded?.Invoke(Interaction.Clicked);
                }
                else if (evtData.pointerDrag != null)
                {
                    ExecuteEvents.ExecuteHierarchy(target, evtData, ExecuteEvents.dropHandler);
                }

                evtData.pointerPress = null;
                evtData.rawPointerPress = null;
            }
            else if (pressed
                && mayLongPress[button])
            {
                if (deltaTime < THRESHOLD_LONG_PRESS)
                {
                    ExecuteEvents.ExecuteHierarchy(evtData.pointerPress, evtData, LongPressEvents.longPressUpdateHandler);
                }
                else
                {
                    mayLongPress[button] = false;
                    ExecuteEvents.ExecuteHierarchy(evtData.pointerPress, evtData, LongPressEvents.longPressHandler);
                    buttonEvents[button].OnLongPress(evtData);
                    inputButtonEvents[evtData.button]?.OnLongPress(evtData);
                    InteractionNeeded?.Invoke(Interaction.Clicked);
                }
            }
        }

        private void TestDrag(PointerEventData evtData, bool pressed, float pixelDragThresholdSquared)
        {
            AnyDragging |= evtData.pointerDrag != null;
            if (evtData.pointerDrag != null && evtData.IsPointerMoving())
            {
                var wasDragging = evtData.dragging;
                if (!pressed)
                {
                    evtData.dragging = false;
                }
                else if (!evtData.useDragThreshold)
                {
                    evtData.dragging = true;
                }
                else
                {
                    dragDistances[evtData.button] += evtData.delta.sqrMagnitude;
                    evtData.dragging = dragDistances[evtData.button] > pixelDragThresholdSquared;
                }

                if (evtData.dragging)
                {
                    if (evtData.pointerPress != null && evtData.pointerPress != evtData.pointerDrag)
                    {
                        ExecuteEvents.Execute(evtData.pointerPress, evtData, ExecuteEvents.pointerUpHandler);
                        InteractionNeeded?.Invoke(Interaction.Released);

                        evtData.eligibleForClick = false;
                        evtData.pointerPress = null;
                        evtData.rawPointerPress = null;
                    }

                    if (!wasDragging)
                    {
                        ExecuteEvents.ExecuteHierarchy(evtData.pointerDrag, evtData, ExecuteEvents.beginDragHandler);
                        InteractionNeeded?.Invoke(Interaction.Dragged);
                    }

                    evtData.pointerDrag = ExecuteEvents.ExecuteHierarchy(evtData.pointerDrag ?? evtData.pointerPress, evtData, ExecuteEvents.dragHandler);
                    InteractionNeeded?.Invoke(Interaction.Dragged);
                }
                else if (wasDragging)
                {
                    ExecuteEvents.ExecuteHierarchy(evtData.pointerDrag, evtData, ExecuteEvents.endDragHandler);
                    InteractionNeeded?.Invoke(Interaction.Dragged);
                    evtData.pointerDrag = null;
                }
            }
        }
    }
}
