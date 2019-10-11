using System.Collections.Generic;

using Juniper.Progress;

using UnityEngine;

namespace Juniper.Widgets
{
    public class LoadingBar : MonoBehaviour, IProgress
    {
        private Transform indicator;
        private TextComponentWrapper text;

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
                    collider.DestroyImmediate();
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

        public void OnEnable()
        {
            ResetValues();
        }

        public void OnDisable()
        {
            ResetValues();
        }

        private void ResetValues()
        {
            TargetValue = 0;
            Progress = 0;
            Status = string.Empty;
        }

        public void ReportWithStatus(float progress, string status)
        {
            TargetValue = progress;
            Status = status ?? string.Empty;
        }

        public void Awake()
        {
            indicator = this.Query("Center/Indicator");
            var t = transform.Find("LoadingStatus");
            if (t != null)
            {
                text = t.Ensure<TextComponentWrapper>();
            }
        }

        public void Update()
        {
            Progress = Mathf.Lerp(Progress, TargetValue, 0.2f);
            var s = indicator.transform.localScale;
            s.x = Progress;
            indicator.transform.localScale = s;
            indicator.transform.localPosition = new Vector3(0.5f * (Progress - 1), 0, 0);
            text.Text = Status;
        }

        public string Status
        {
            get; private set;
        }

        public float Progress
        {
            get; private set;
        }

        public float TargetValue
        {
            get; set;
        }
    }
}