using System;
using UnityEngine;
using UnityEngine.UI;

namespace Juniper.Widgets
{
    public interface IValuedControl<T>
    {
        T value { get; set; }

        event EventHandler<T> ValueChange;
    }

    public static class IValuedControlExt
    {
        public static IValuedControl<float> GetSlider(this GameObject element) =>
            ((IValuedControl<float>)((UnitySliderAdapter)element.GetComponentInChildren<Slider>()))
            ?? element.GetComponentInChildren<Draggable>();
    }
}
