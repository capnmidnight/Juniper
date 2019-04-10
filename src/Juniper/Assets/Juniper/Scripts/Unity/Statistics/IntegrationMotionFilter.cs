using System;

using UnityEngine;

using Vector3 = UnityEngine.Vector3;

namespace Juniper.Unity.Statistics
{
    [Serializable]
    [CreateAssetMenu(fileName = "integrationMotionFilter", menuName = "Motion Filters/Integration")]
    public class IntegrationMotionFilter : AbstractMotionFilter
    {
        [Range(0, 1)]
        public float interpolationFactor;

        private float IF, compIF;

        public uint componentCount;
        private Vector3[] components;

        private uint saturation;

        private float lastTime;

        public override Vector3 PredictedPosition
        {
            get
            {
                var deltaTime = Time.unscaledTime - lastTime;
                if (components.Length > 1 && deltaTime > 0)
                {
                    for (var i = components.Length - 2; i >= 0; --i)
                    {
                        components[i] += (components[i + 1] * deltaTime * IF) + (components[i] * compIF);
                    }

                    lastTime = Time.unscaledTime;
                }
                return Position;
            }
        }

        public override Vector3 Position
        {
            get
            {
                return components[0];
            }
        }

#if UNITY_EDITOR

        public override void Copy(AbstractMotionFilter filter)
        {
            if (filter is IntegrationMotionFilter)
            {
                var f = (IntegrationMotionFilter)filter;
                componentCount = f.componentCount;
                interpolationFactor = f.interpolationFactor;
            }
        }

#endif

        public override void UpdateState(Vector3 point)
        {
            componentCount = Math.Max(1, componentCount);

            if (componentCount != components?.Length)
            {
                components = new Vector3[componentCount];
                saturation = 0;
            }

            interpolationFactor = Mathf.Clamp01(interpolationFactor);
            if (!Mathf.Approximately(IF, interpolationFactor))
            {
                IF = interpolationFactor;
                compIF = 1 - IF;
            }

            var deltaTime = Time.unscaledTime - lastTime;
            if (deltaTime > 0)
            {
                Vector3 last;
                for (uint i = 0; i < saturation; ++i)
                {
                    last = components[i];
                    components[i] = (point * IF) + (last * compIF);
                    point = (components[i] - last) / deltaTime;
                }

                if (saturation < components.Length)
                {
                    ++saturation;
                }

                lastTime = Time.unscaledTime;
            }
        }
    }
}
