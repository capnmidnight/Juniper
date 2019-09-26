using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Juniper.Input;
using Juniper.Input.Pointers;

using UnityEngine;
using UnityEngine.EventSystems;

#if !NET_4_6 && !NET_3_5
using System.Reflection;
#endif

namespace Juniper.Events
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
            yield return GetComponent<IPointerDevice>().ButtonType;
            yield return typeof(VirtualTouchPadButton);
            yield return typeof(VirtualTriggerButton);
        }

        public string buttonTypeName;
        public string buttonValueName;

        public KeyCode inputKey = KeyCode.None;

        public T GetButtonValue<T>()
            where T : struct
        {
            var type = typeof(T);
#if NET_4_6 || NET_3_5
            var typeInfo = type;
#else
            var typeInfo = typeof(T).GetTypeInfo();
#endif

            if (typeInfo.IsEnum)
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

        public JuniperPointerEvent onClick;
        public JuniperPointerEvent onDoubleClick;
        public JuniperPointerEvent onLongPress;
        public JuniperPointerEvent onUp;
        public JuniperPointerEvent onDown;

        public event EventHandler<JuniperPointerEventData> Click;

        public event EventHandler<JuniperPointerEventData> DoubleClick;

        public event EventHandler<JuniperPointerEventData> LongPress;

        public event EventHandler<JuniperPointerEventData> Up;

        public event EventHandler<JuniperPointerEventData> Down;

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

        public void OnDown(JuniperPointerEventData evt)
        {
            IsPressed = true;
            onDown?.Invoke(evt);
            Down?.Invoke(this, evt);
        }

        public void OnClick(JuniperPointerEventData evt)
        {
            onClick?.Invoke(evt);
            Click?.Invoke(this, evt);
            if (evt.clickCount == 2)
            {
                OnDoubleClick(evt);
            }
        }

        private void OnDoubleClick(JuniperPointerEventData evt)
        {
            onDoubleClick?.Invoke(evt);
            DoubleClick?.Invoke(this, evt);
        }

        public void OnLongPress(JuniperPointerEventData evt)
        {
            onLongPress?.Invoke(evt);
            LongPress?.Invoke(this, evt);
        }

        public void OnUp(JuniperPointerEventData evt)
        {
            IsPressed = false;
            onUp?.Invoke(evt);
            Up?.Invoke(this, evt);
        }
    }
}
