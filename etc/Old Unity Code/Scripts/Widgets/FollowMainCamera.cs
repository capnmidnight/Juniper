using Juniper.Display;

using UnityEngine;

namespace Juniper.Widgets
{
    public class FollowMainCamera : MonoBehaviour
    {
        [SerializeField]
        [HideInNormalInspector]
        private FollowObject f;

        public void OnValidate()
        {
            f = this.Ensure<FollowObject>();
        }

        public void Awake()
        {
            if (f == null)
            {
                OnValidate();
            }

            f.followObject = DisplayManager.MainCamera.transform;
        }

        public void OnEnable()
        {
            f.Skip();
        }
    }
}
