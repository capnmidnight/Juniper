using System;

using UnityEngine.UI;

namespace Juniper.Widgets
{
    public sealed class UnitySliderAdapter : IValuedControl<float>
    {
        private readonly Slider element;

        private UnitySliderAdapter(Slider element)
        {
            this.element = element;
            this.element.onValueChanged.AddListener(f =>
                ValueChange?.Invoke(this, f));
        }

        public float value
        {
            get
            {
                return element.value;
            }
            set
            {
                element.value = value;
            }
        }

        public event EventHandler<float> ValueChange;

        public static implicit operator UnitySliderAdapter(Slider element)
        {
            if (element == null)
            {
                return null;
            }
            else
            {
                return new UnitySliderAdapter(element);
            }
        }
    }
}
