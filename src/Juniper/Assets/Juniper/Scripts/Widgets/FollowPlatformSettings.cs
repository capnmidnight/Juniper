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
            if(GetComponents<FollowPlatformSettings>()
                .All(other => other.platform != PlatformType.None))
            {
                var none = gameObject.AddComponent<FollowPlatformSettings>();
                none.platform = PlatformType.None;
            }
#endif
        }

        public void Awake()
        {
            bool isGood = true;
            if (platform != JuniperSystem.CurrentPlatform)
            {
                bool isDefault = platform == PlatformType.None
                    && !GetComponents<FollowPlatformSettings>()
                        .Any(other => other.platform == JuniperSystem.CurrentPlatform);

                if (!isDefault)
                {
                    isGood = enabled = false;
                    this.DestroyImmediate();
                }
            }
            
            if(isGood)
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
