using UnityEngine;

namespace Juniper.Widgets
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Animator))]
    public class Splitter : MonoBehaviour
    {
        public bool open;
        private bool wasOpen;

#if UNITY_MODULES_ANIMATION
        private Animator animator;
#endif

        public void Awake()
        {
            wasOpen = open;
        }

        public void Update()
        {
#if UNITY_MODULES_ANIMATION
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
#endif
            if (open != wasOpen)
            {
                wasOpen = open;
#if UNITY_MODULES_ANIMATION
                animator.Play(open ? "Open" : "Closed");
#endif
            }
        }

        public void Toggle()
        {
            open = !open;
        }
    }
}
