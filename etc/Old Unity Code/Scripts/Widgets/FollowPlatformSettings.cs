using System.Linq;

using Juniper.XR;

using UnityEngine;

namespace Juniper.Widgets
{
    /// <summary>
    /// Make one object's pose follow the pose of another object.
    /// </summary>
    public class FollowPlatformSettings : AbstractFollowSettings
    {
        public PlatformType platform;

        [SerializeField]
        [HideInNormalInspector]
        private FollowObject f;

        public void OnValidate()
        {
            if (this.f == null)
            {
                var f = this.Ensure<FollowObject>();
                if (!f.IsNew)
                {
                    Copy(f);
                }

                this.f = f;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying &&
                GetComponents<FollowPlatformSettings>()
                .All(other => other.platform != PlatformType.None))
            {
                var none = gameObject.AddComponent<FollowPlatformSettings>();
                none.platform = PlatformType.None;
            }
#endif
        }

        public void Awake()
        {
            var isGood = true;
            if (platform != JuniperSystem.CurrentPlatform)
            {
                var isDefault = platform == PlatformType.None
                    && !GetComponents<FollowPlatformSettings>()
                        .Any(other => other.platform == JuniperSystem.CurrentPlatform);

                if (!isDefault)
                {
                    isGood = enabled = false;
                    this.Destroy();
                }
            }

            if (isGood)
            {
                if (f == null)
                {
                    OnValidate();
                }

                f.Copy(this);
            }
        }

#if UNITY_EDITOR
        public void Update()
        {
            Copy(f);
        }
#endif
    }
}
