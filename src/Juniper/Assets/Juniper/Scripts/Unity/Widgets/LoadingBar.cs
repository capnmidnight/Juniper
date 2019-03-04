.0using Juniper.Progress;

using UnityEngine;

namespace Juniper.Widgets
{
    public class LoadingBar : MonoBehaviour, IProgressReceiver
    {
        public static PooledComponent<LoadingBar> Ensure(Transform parent)
        {
            return parent.EnsureTransform("LoadingBar/Center/Indicator", () =>
            {
                var indicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
                indicator.layer = LayerMask.NameToLayer("TransparentFX");
                return indicator;
            })
            .Value
            .parent
            .parent
            .EnsureComponent<LoadingBar>(
#if UNITY_MODULES_PHYSICS
                (loadingBar) =>
                {
                    foreach (var collider in loadingBar.GetComponentsInChildren<Collider>())
                    {
                        collider.Destroy();
                    }
                }
#endif
            );
        }

        public void SetProgress(float progress, string status = null)
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