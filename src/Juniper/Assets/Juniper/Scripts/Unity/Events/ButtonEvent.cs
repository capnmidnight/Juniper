using Juniper.Unity.Input.Pointers;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Juniper.Unity.Events
{
    [RequireComponent(typeof(IPointerDevice))]
    public class ButtonEvent : MonoBehaviour
    {
        public static string FormatKey(string type, string value)
        {
            return $"{type}::{value}";
        }

        public static string MakeKey<T>(T value)
            where T : struct
        {
            return FormatKey(typeof(T).FullName, value.ToString());
        }

        private static readonly Regex KeyPattern =
            new Regex(FormatKey("(\\w+(?:(?:\\.|\\+)\\w+)*)", "(\\w+)"), RegexOptions.Compiled);

        public IEnumerable<Type> GetSupportedButtonTypes()
        {
            yield return typeof(PointerEventData.InputButton);
            yield return GetComponent<IPointerDevice>().ButtonType;
            yield return typeof(Input.VirtualTouchPadButton);
            yield return typeof(Input.VirtualTriggerButton);
        }

        public string buttonTypeName;
        public string buttonValueName;

        public T? GetButtonValue<T>()
            where T : struct
        {
            var type = typeof(T);

            if (type.IsEnum)
            {
                return (T)Enum.Parse(type, buttonValueName);
            }
            else
            {
                throw new ArgumentException("Button type must be an enumeration", buttonTypeName);
            }
        }

        public string Key
        {
            get
            {
                return FormatKey(buttonTypeName, buttonValueName);
            }

            set
            {
                var match = KeyPattern.Match(value);
                if (match == null)
                {
                    var pattern = FormatKey("{TypeName}", "{ValueName}");
                    throw new ArgumentException($"Key does not match expected pattern {pattern}");
                }

                buttonTypeName = match.Groups[1].Value;
                buttonValueName = match.Groups[2].Value;
            }
        }

        public PointerEvent onClick;
        public PointerEvent onDoubleClick;
        public PointerEvent onLongPress;
        public PointerEvent onUp;
        public PointerEvent onDown;

        public event EventHandler<PointerEventData> Click;

        public event EventHandler<PointerEventData> DoubleClick;

        public event EventHandler<PointerEventData> LongPress;

        public event EventHandler<PointerEventData> Up;

        public event EventHandler<PointerEventData> Down;

        private bool wasPressed;

        public bool IsPressed
        {
            get; private set;
        }

        public void LateUpdate()
        {
            wasPressed = IsPressed;
        }

        public bool IsDown
        {
            get
            {
                return IsPressed && !wasPressed;
            }
        }

        public bool IsUp
        {
            get
            {
                return !IsPressed && wasPressed;
            }
        }

        public void OnDown(PointerEventData evt)
        {
            IsPressed = true;
            onDown?.Invoke(evt);
            Down?.Invoke(this, evt);
        }

        public void OnClick(PointerEventData evt)
        {
            onClick?.Invoke(evt);
            Click?.Invoke(this, evt);
            if (evt.clickCount == 2)
            {
                OnDoubleClick(evt);
            }
        }

        private void OnDoubleClick(PointerEventData evt)
        {
            onDoubleClick?.Invoke(evt);
            DoubleClick?.Invoke(this, evt);
        }

        public void OnLongPress(PointerEventData evt)
        {
            onLongPress?.Invoke(evt);
            LongPress?.Invoke(this, evt);
        }

        public void OnUp(PointerEventData evt)
        {
            IsPressed = false;
            onUp?.Invoke(evt);
            Up?.Invoke(this, evt);
        }
    }
}
