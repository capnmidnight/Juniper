using System;

using UnityEngine;

namespace Juniper.Sound.Beats
{
    [Serializable]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "This is an object that is meant to be edited in the Unity Editor, so its values must be public fields, not properties.")]
    public class BandPass
    {
        [Range(0, 10)]
        public int LowN = 0;
        [Range(0, 10)]
        public int HighN = 10;
        [Range(1, 10)]
        public int Multiplier = 1;
        [Range(1, 10)]
        public int Divider = 1;

        public bool Visualize;

        [HideInInspector]
        [NonSerialized]
        public bool wasVisualized;

        public int Low => (int)Math.Pow(2, LowN);
        public int High => (int)Math.Pow(2, HighN);
    }
}