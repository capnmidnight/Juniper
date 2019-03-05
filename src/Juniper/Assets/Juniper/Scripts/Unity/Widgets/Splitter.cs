#if UNITY_MODULES_ANIMATION

using UnityEngine;

namespace Juniper.Unity.Widgets
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Animator))]
    public class Splitter : MonoBehaviour
    {
        public bool open;

        public void Awake()
        {
            animator = GetComponent<Animator>();
            wasOpen = open;
        }

        public void Update()
        {
            if (open != wasOpen)
            {
                wasOpen = open;
                animator.Play(open ? "Open" : "Closed");
            }
        }

        public void Toggle()
        {
            open = !open;
        }

        private Animator animator;
        private bool wasOpen;
    }
}

#endif