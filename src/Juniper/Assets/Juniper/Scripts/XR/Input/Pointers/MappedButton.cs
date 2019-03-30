using System;

using Juniper.Input;
using Juniper.Unity.Events;

using UnityEngine;
using UnityEngine.EventSystems;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Unity.Input.Pointers
{
    public class MappedButton<ButtonIDType>
        where ButtonIDType : struct
    {
        private const float THRESHOLD_CLICK = 0.4f;
        private const float THRESHOLD_LONG_PRESS = 2f;

        public readonly ButtonIDType button;
        public readonly int buttonNumber;

        private float buttonDownTime;
        private float dragDistance;
        private bool mayLongPress;

        public event Action<Interaction> InteractionNeeded;

        public event Func<ButtonIDType, bool> ButtonDownNeeded;

        public event Func<ButtonIDType, bool> ButtonUpNeeded;

        public event Func<ButtonIDType, bool> ButtonPressedNeeded;

        public event Func<int, PointerEventData, PointerEventData> ClonedPointerEventNeeded;

        private readonly ButtonEvent buttonEvent;

        public InputButton? UnityInputButton
        {
            get
            {
                if (buttonEvent.inputButton == InputEventButton.None)
                {
                    return null;
                }
                else
                {
                    return (InputButton)buttonEvent.inputButton;
                }
            }
        }

        public MappedButton(ButtonIDType btn, InputEventButton inputBtn, GameObject pointer)
        {
            button = btn;
            buttonNumber = Convert.ToInt32(btn);
            var key = ButtonEvent.MakeKey(button);
            var btns = pointer.GetComponents<ButtonEvent>();
            buttonEvent = Array.Find(btns, e => e.Key == key)
                ?? Array.Find(btns, e => e.inputButton == inputBtn)
                ?? pointer.AddComponent<ButtonEvent>();

            buttonEvent.Key = key;
            buttonEvent.inputButton = inputBtn;
        }

        public bool IsDown
        {
            get; private set;
        }

        public bool IsUp
        {
            get; private set;
        }

        public bool IsPressed
        {
            get; private set;
        }

        public bool IsDragging
        {
            get; private set;
        }

        public IEventSystemHandler Process(PointerEventData eventData, float pixelDragThresholdSquared)
        {
            IsPressed = ButtonPressedNeeded?.Invoke(button) == true;
            var evtData = ClonedPointerEventNeeded?.Invoke(buttonEvent.PointerDataID, eventData) ?? eventData;
            evtData.button = (InputButton)buttonEvent.inputButton;

            TestUpDown(evtData);
            TestDrag(evtData, pixelDragThresholdSquared);

            return evtData.pointerEnter?.GetComponent<IEventSystemHandler>();
        }

        private void TestUpDown(PointerEventData evtData)
        {
            IsUp = ButtonUpNeeded?.Invoke(button) == true;
            IsDown = ButtonDownNeeded?.Invoke(button) == true;
            if (IsDown)
            {
                mayLongPress = true;
                buttonDownTime = Time.time;
                dragDistance = 0;
                evtData.rawPointerPress = evtData.pointerEnter;
                evtData.pressPosition = evtData.position;
                evtData.pointerPressRaycast = evtData.pointerCurrentRaycast;
                evtData.eligibleForClick = true;

                evtData.pointerDrag = ExecuteEvents.ExecuteHierarchy(evtData.pointerEnter, evtData, ExecuteEvents.initializePotentialDrag);
                evtData.pointerPress = ExecuteEvents.ExecuteHierarchy(evtData.pointerEnter, evtData, ExecuteEvents.pointerDownHandler);
                buttonEvent.OnDown(evtData);
                if (evtData.pointerPress != null)
                {
                    InteractionNeeded?.Invoke(Interaction.Pressed);
                }
            }

            var deltaTime = Time.time - buttonDownTime;
            evtData.eligibleForClick = deltaTime < THRESHOLD_CLICK;

            if (IsUp)
            {
                ExecuteEvents.ExecuteHierarchy(evtData.pointerPress, evtData, ExecuteEvents.pointerUpHandler);
                buttonEvent.OnUp(evtData);
                if (evtData.pointerPress != null)
                {
                    InteractionNeeded?.Invoke(Interaction.Released);
                }

                var target = evtData.pointerCurrentRaycast.gameObject;
                if (evtData.eligibleForClick)
                {
                    ++evtData.clickCount;
                    evtData.clickTime = Time.time;

                    evtData.selectedObject = ExecuteEvents.ExecuteHierarchy(evtData.pointerPress, evtData, ExecuteEvents.pointerClickHandler);
                    buttonEvent.OnClick(evtData);
                    if (evtData.pointerPress != null)
                    {
                        InteractionNeeded?.Invoke(Interaction.Clicked);
                    }
                }
                else if (evtData.pointerDrag != null)
                {
                    ExecuteEvents.ExecuteHierarchy(target, evtData, ExecuteEvents.dropHandler);
                }

                evtData.pointerPress = null;
                evtData.rawPointerPress = null;
            }
            else if (IsPressed && mayLongPress)
            {
                if (deltaTime < THRESHOLD_LONG_PRESS)
                {
                    ExecuteEvents.ExecuteHierarchy(evtData.pointerPress, evtData, LongPressEvents.longPressUpdateHandler);
                }
                else
                {
                    mayLongPress = false;
                    ExecuteEvents.ExecuteHierarchy(evtData.pointerPress, evtData, LongPressEvents.longPressHandler);
                    buttonEvent.OnLongPress(evtData);
                    if (evtData.pointerPress != null)
                    {
                        InteractionNeeded?.Invoke(Interaction.Clicked);
                    }
                }
            }
        }

        private void TestDrag(PointerEventData evtData, float pixelDragThresholdSquared)
        {
            if (evtData.pointerDrag != null && evtData.IsPointerMoving())
            {
                var wasDragging = evtData.dragging;
                if (!IsPressed)
                {
                    evtData.dragging = false;
                }
                else if (!evtData.useDragThreshold)
                {
                    evtData.dragging = true;
                }
                else
                {
                    dragDistance += evtData.delta.sqrMagnitude;
                    evtData.dragging = dragDistance > pixelDragThresholdSquared;
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
                        mayLongPress = false;
                        ExecuteEvents.ExecuteHierarchy(evtData.pointerDrag, evtData, ExecuteEvents.beginDragHandler);
                        InteractionNeeded?.Invoke(Interaction.DraggingStarted);
                        IsDragging = true;
                    }

                    evtData.pointerDrag = ExecuteEvents.ExecuteHierarchy(evtData.pointerDrag ?? evtData.pointerPress, evtData, ExecuteEvents.dragHandler);
                    InteractionNeeded?.Invoke(Interaction.Dragged);
                }
                else if (wasDragging)
                {
                    ExecuteEvents.ExecuteHierarchy(evtData.pointerDrag, evtData, ExecuteEvents.endDragHandler);
                    InteractionNeeded?.Invoke(Interaction.DraggingEnded);
                    evtData.pointerDrag = null;
                    IsDragging = false;
                }
            }
        }
    }
}