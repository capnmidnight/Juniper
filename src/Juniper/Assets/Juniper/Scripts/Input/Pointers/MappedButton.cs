using System;
using System.Collections.Generic;

using Juniper.Events;

using UnityEngine;
using UnityEngine.EventSystems;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Input.Pointers
{
    public class MappedButton<ButtonIDType>
        where ButtonIDType : struct
    {
        private const float THRESHOLD_CLICK = 0.4f;
        private const float THRESHOLD_LONG_PRESS = 2f;

        public ButtonIDType button;
        public int buttonNumber;
        private string buttonName;

        private float buttonDownTime;
        private float dragDistance;
        private bool mayLongPress;

        public event Action<Interaction> InteractionNeeded;

        public event Func<ButtonIDType, bool> ButtonDownNeeded;

        public event Func<ButtonIDType, bool> ButtonUpNeeded;

        public event Func<ButtonIDType, bool> ButtonPressedNeeded;

        public event Func<int, JuniperPointerEventData, JuniperPointerEventData> ClonedPointerEventNeeded;

        private readonly ButtonEvent buttonEvent;

        public KeyCode UnityKeyCode
        {
            get
            {
                return buttonEvent.inputKey;
            }
        }

        public MappedButton(ButtonIDType btn, KeyCode inputKey, GameObject eventParent)
        {
            SetButton(btn);
            var key = ButtonEvent.MakeKey(button);
            var btns = eventParent.GetComponents<ButtonEvent>();
            buttonEvent = Array.Find(btns, e => e.Key == key)
                ?? Array.Find(btns, e => e.inputKey == inputKey)
                ?? eventParent.AddComponent<ButtonEvent>();
            buttonEvent.Key = key;
            buttonEvent.inputKey = inputKey;
        }

        public MappedButton(ButtonIDType btn, ButtonEvent evt)
        {
            SetButton(btn);
            buttonEvent = evt;
        }

        public void Destroy()
        {
            buttonEvent.DestroyImmediate();
        }

        private void SetButton(ButtonIDType value)
        {
            button = value;
            buttonName = value.ToString();
            buttonNumber = Convert.ToInt32(value);
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

        public IEventSystemHandler Process(JuniperPointerEventData eventData, float pixelDragThresholdSquared, List<KeyCode> keyPresses)
        {
            if (buttonEvent.buttonValueName != buttonName)
            {
                SetButton(buttonEvent.GetButtonValue<ButtonIDType>());
            }

            IsPressed = ButtonPressedNeeded(button);
            var evtData = ClonedPointerEventNeeded(buttonEvent.GetInstanceID(), eventData);
            evtData.keyCode = buttonEvent.inputKey;
            switch (evtData.keyCode)
            {
                case KeyCode.Mouse0: evtData.button = InputButton.Left; break;
                case KeyCode.Mouse1: evtData.button = InputButton.Right; break;
                case KeyCode.Mouse2: evtData.button = InputButton.Middle; break;
                default: evtData.button = (InputButton)(-1); break;
            }

            TestUpDown(evtData, keyPresses);
            TestDrag(evtData, pixelDragThresholdSquared);

            return evtData.pointerEnter?.GetComponent<IEventSystemHandler>();
        }

        private void TestUpDown(JuniperPointerEventData evtData, List<KeyCode> keyPresses)
        {
            IsUp = ButtonUpNeeded(button);
            IsDown = ButtonDownNeeded(button);
            if (IsDown)
            {
                mayLongPress = true;
                buttonDownTime = Time.unscaledTime;
                dragDistance = 0;
                evtData.rawPointerPress = evtData.pointerEnter;
                evtData.pressPosition = evtData.position;
                evtData.pointerPressRaycast = evtData.pointerCurrentRaycast;
                evtData.eligibleForClick = true;
                evtData.pointerPress = ExecuteEvents.ExecuteHierarchy(evtData.pointerEnter, evtData, ExecuteEvents.pointerDownHandler);
                evtData.pointerDrag = ExecuteEvents.ExecuteHierarchy(evtData.pointerEnter, evtData, ExecuteEvents.initializePotentialDrag);
                buttonEvent.OnDown(evtData);
                if (evtData.pointerPress != null)
                {
                    InteractionNeeded(Interaction.Pressed);
                }
            }

            var deltaTime = Time.unscaledTime - buttonDownTime;
            evtData.eligibleForClick = deltaTime < THRESHOLD_CLICK;

            if (IsUp)
            {
                ExecuteEvents.ExecuteHierarchy(evtData.pointerPress, evtData, ExecuteEvents.pointerUpHandler);
                buttonEvent.OnUp(evtData);
                if (evtData.pointerPress != null)
                {
                    InteractionNeeded(Interaction.Released);
                }

                var target = evtData.pointerCurrentRaycast.gameObject;
                if (evtData.eligibleForClick)
                {
                    ++evtData.clickCount;
                    evtData.clickTime = Time.unscaledTime;

                    evtData.selectedObject = ExecuteEvents.ExecuteHierarchy(evtData.pointerPress, evtData, ExecuteEvents.pointerClickHandler);
                    buttonEvent.OnClick(evtData);
                    if (evtData.pointerPress != null)
                    {
                        InteractionNeeded(Interaction.Clicked);
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
                        InteractionNeeded(Interaction.Clicked);
                    }
                    keyPresses.MaybeAdd(evtData.keyCode);
                }
            }
        }

        private void TestDrag(JuniperPointerEventData evtData, float pixelDragThresholdSquared)
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
                        InteractionNeeded(Interaction.Released);

                        evtData.eligibleForClick = false;
                        evtData.pointerPress = null;
                        evtData.rawPointerPress = null;
                    }

                    if (!wasDragging)
                    {
                        mayLongPress = false;
                        ExecuteEvents.ExecuteHierarchy(evtData.pointerDrag, evtData, ExecuteEvents.beginDragHandler);
                        InteractionNeeded(Interaction.DraggingStarted);
                        IsDragging = true;
                    }

                    evtData.pointerDrag = ExecuteEvents.ExecuteHierarchy(evtData.pointerDrag ?? evtData.pointerPress, evtData, ExecuteEvents.dragHandler);
                    InteractionNeeded(Interaction.Dragged);
                }
                else if (wasDragging && !IsPressed)
                {
                    ExecuteEvents.ExecuteHierarchy(evtData.pointerDrag, evtData, ExecuteEvents.endDragHandler);
                    InteractionNeeded(Interaction.DraggingEnded);
                    evtData.pointerDrag = null;
                    IsDragging = false;
                }
            }
        }
    }
}
