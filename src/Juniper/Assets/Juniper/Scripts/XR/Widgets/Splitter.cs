using UnityEngine;

namespace Juniper.Unity.Widgets
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
#if UNITY_MODULES_ANIMATION
            animator = GetComponent<Animator>();
#endif
            wasOpen = open;
        }

        public void Update()
        {
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