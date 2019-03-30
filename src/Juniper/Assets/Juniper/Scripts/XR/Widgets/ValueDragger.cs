using System;
using System.Globalization;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Juniper.Unity.Widgets
{
    public class ValueDragger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public InputField field;
        public float min = 0;
        public float max = 100;
        public float delta = 1;
        public FloatEvent onValueChanged;

        public void Awake()
        {
            field.onValueChanged.AddListener(OnTextChanged);
        }

        public void OnTextChanged(string text)
        {
            if (enabled)
            {
                float value;
                if (float.TryParse(text, out value))
                {
                    onValueChanged?.Invoke(value);
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            locked = true;
            eventData.Use();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            locked = false;
        }

        public void Update()
        {
            if (field != null)
            {
                if (locked)
                {
                    Vector2 end = UnityEngine.Input.mousePosition;

                    if (!wasLocked)
                    {
                        drag = Vector2.zero;
                    }
                    else
                    {
                        drag += end - start;
                        if (Mathf.Abs(drag.x) > 10)
                        {
                            ChangeValue(Mathf.Sign(drag.x) * delta);
                            drag = Vector2.zero;
                        }
                    }

                    start = end;
                }
                else if (field.text != lastText)
                {
                    ChangeValue(0);
                }

                lastText = field.text;
            }

            wasLocked = locked;
        }

        private Vector2 start, drag;
        private bool locked, wasLocked;
        private string lastText;

        private void ChangeValue(float v)
        {
            float value;
            var withDecimal = field.text.EndsWith(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, StringComparison.CurrentCulture);
            if (float.TryParse(field.text, out value))
            {
                value = Mathf.Clamp(value + v, min, max);

                if (field.characterValidation == InputField.CharacterValidation.Integer)
                {
                    field.text = Mathf.RoundToInt(value).ToString();
                }
                else
                {
                    field.text = (Mathf.Round(value * 100) / 100).ToString();
                }

                if (withDecimal)
                {
                    field.text += CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                }
            }
        }
    }
}