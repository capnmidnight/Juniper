using System;

using UnityEngine;

using Vector3 = UnityEngine.Vector3;

namespace Juniper.Statistics
{
    [Serializable]
    public abstract class AbstractMotionFilter : ScriptableObject
    {
        public abstract Vector3 PredictedPosition
        {
            get;
        }

        public abstract Vector3 Position
        {
            get;
        }

        public abstract void UpdateState(Vector3 point);

#if UNITY_EDITOR

        public abstract void Copy(AbstractMotionFilter filter);

#endif
    }
}