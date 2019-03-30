using System;

using UnityEngine;

using Vector3 = UnityEngine.Vector3;

namespace Juniper.Unity.Statistics
{
    [Serializable]
    [CreateAssetMenu(fileName = "lerpMotionFilter", menuName = "Motion Filters/LERP")]
    public class LerpMotionFilter : AbstractMotionFilter
    {
        [Range(0, 1)]
        public float interpolationFactor;

        private float IF
        {
            get; set;
        }

        private float CompIF
        {
            get; set;
        }

        private Vector3 position;

        public override Vector3 PredictedPosition
        {
            get
            {
                return position;
            }
        }

        public override Vector3 Position
        {
            get
            {
                return position;
            }
        }

#if UNITY_EDITOR

        public override void Copy(AbstractMotionFilter filter)
        {
            if (filter is LerpMotionFilter)
            {
                var f = (LerpMotionFilter)filter;
                interpolationFactor = f.interpolationFactor;
            }
        }

#endif

        public override void UpdateState(Vector3 point)
        {
            interpolationFactor = Mathf.Clamp01(interpolationFactor);

            if (!Mathf.Approximately(IF, interpolationFactor))
            {
                IF = interpolationFactor;
                CompIF = 1 - IF;
            }

            position = (point * IF) + (position * CompIF);
        }
    }
}