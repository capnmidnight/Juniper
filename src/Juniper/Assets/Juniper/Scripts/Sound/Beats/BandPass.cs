using System;

using UnityEngine;

namespace Juniper.Sound.Beats
{
    [Serializable]
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

        public int Low
        {
            get
            {
                return (int)Math.Pow(2, LowN);
            }
        }
        public int High
        {
            get
            {
                return (int)Math.Pow(2, HighN);
            }
        }
    }
}