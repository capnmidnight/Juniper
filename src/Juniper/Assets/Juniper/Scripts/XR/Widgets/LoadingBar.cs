using System.Collections.Generic;

using Juniper.Progress;

using UnityEngine;

namespace Juniper.Unity.Widgets
{
    public class LoadingBar : MonoBehaviour, IProgress
    {
        public static PooledComponent<LoadingBar> Ensure(Transform parent, int transparentLayer)
        {
            var bar = parent.Ensure<Transform>("LoadingBar/Center/Indicator", () =>
                GameObject.CreatePrimitive(PrimitiveType.Cube))
                .Value
                .parent
                .parent
                .Ensure<LoadingBar>();

#if UNITY_MODULES_PHYSICS
            if (bar.IsNew)
            {
                foreach (var collider in bar.Value.GetComponentsInChildren<Collider>())
                {
                    collider.Destroy();
                }
            }
#endif

            var queue = new Queue<Transform>
            {
                bar.Value.transform
            };

            while (queue.Count > 0)
            {
                var here = queue.Dequeue();
                queue.AddRange(here.Children());
                here.gameObject.layer = transparentLayer;
            }

            return bar;
        }

        public void Report(float progress)
        {
            Report(progress, null);
        }

        public void Report(float progress, string status)
        {
            TargetValue = progress;
        }

        public void Awake()
        {
            indicator = this.Query("Center/Indicator");
        }

        public void Update()
        {
            Progress = Mathf.Lerp(Progress, TargetValue, 0.1f);
            var s = indicator.transform.localScale;
            s.x = Progress;
            indicator.transform.localScale = s;
            indicator.transform.localPosition = new Vector3(0.5f * (Progress - 1), 0, 0);
        }

        private Transform indicator;

        public float Progress
        {
            get; private set;
        }

        public float TargetValue
        {
            get; set;
        }

        private WaitUntil _waiter;

        public WaitUntil Waiter
        {
            get
            {
                if (_waiter == null)
                {
                    _waiter = new WaitUntil(this.IsComplete);
                }
                else
                {
                    _waiter.Reset();
                }
                return _waiter;
            }
        }
    }
}