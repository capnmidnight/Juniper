using System;
using System.Collections;

using Juniper.Animation;
using Juniper.Display;
using Juniper.Input;

using UnityEngine;

#if UNITY_MODULES_AUDIO

using Juniper.Audio;

#endif

namespace Juniper.Widgets
{
    /// <summary>
    /// Shows a pointer that directs the user to where to look next.
    /// </summary>
    [RequireComponent(typeof(AbstractAnimator))]
    public class AttentionDirector : MonoBehaviour
    {
        /// <summary>
        /// How quickly the object should vanish away
        /// </summary>
        public float shrinkSpeed = 0.5f;

        /// <summary>
        /// The threshold at which to consider an object "in view"
        /// </summary>
        [Range(0, 0.1f)]
        public float minDistance = 0.01f;

        /// <summary>
        /// The point we are trying to find with the attention director.
        /// </summary>
        public Vector3 Target
        {
            get
            {
                if (targetTrans != lastTargetTrans || targetTrans != null && targetTrans.position != lastTargetPosition)
                {
                    if (targetTrans == null)
                    {
                        lastTargetPosition = targetPos = Vector3.zero;
                        showPointer = false;
                    }
                    else
                    {
                        lastTargetPosition = targetTrans.position;
                        var tt = targetTrans.GetComponentInChildren<Tooltip>();
                        targetPos = targetTrans.gameObject.Center(tt);
                    }

                    lastTargetTrans = targetTrans;
                }

                return targetPos;
            }
        }

        /// <summary>
        /// Play one of the animations on the attention director.
        /// </summary>
        /// <param name="anim">Animation.</param>
        public void Play(string anim)
        {
#if UNITY_MODULES_ANIMATION
            animator?.Play(anim);
#endif
        }

        /// <summary>
        /// Get the pointer graphic objects, the main camera, the animator, and the main view system.
        /// Disable the graphics for the pointer.
        /// </summary>
        public void Awake()
        {
            pointer = transform.Find("Pointer");
            arrow = pointer.Find("Tag");
            tagRange = arrow.localPosition;
            image = transform.Find("Image");
#if UNITY_MODULES_ANIMATION
            animator = this.Ensure<UnityAnimator>();
#endif

            pointer.SetActive(false);
            image.SetActive(false);
        }

        /// <summary>
        /// Executes an animation that tells the user that they should be searching for a target.
        /// </summary>
        public void Scan(Action onFound = null)
        {
            StartCoroutine(ScanCoroutine(onFound));
        }

        /// <summary>
        /// Executes an animation that tells the user that they should be searching for a target.
        /// </summary>
        public IEnumerator ScanCoroutine(Action onFound = null)
        {
            this.Activate();
            SetTarget(null, onFound);
#if UNITY_MODULES_ANIMATION
            return animator.PlayCoroutine("Scan");
#else
            return null;
#endif
        }

        /// <summary>
        /// Executes an animation and a sound indicatingn to the user that the target has been found.
        /// </summary>
        public void Found()
        {
            StartCoroutine(FoundCoroutine());
        }

        /// <summary>
        /// Executes an animation and a sound indicating to the user that the target has been found.
        /// </summary>
        public IEnumerator FoundCoroutine()
        {
            this.Activate();
#if UNITY_MODULES_AUDIO
            InteractionAudio.Play(Interaction.Success, transform, null);
#endif
#if UNITY_MODULES_ANIMATION
            return animator.PlayCoroutine("Found");
#else
            return null;
#endif
        }

        /// <summary>
        /// Executes an animation and a sound indicating to the user that the target has been lost.
        /// </summary>
        public void Lost()
        {
            StartCoroutine(LostCoroutine());
        }

        /// <summary>
        /// Executes an animation and a sound indicating to the user that the target has been lost.
        /// </summary>
        /// <returns>The coroutine.</returns>
        public IEnumerator LostCoroutine()
        {
            this.Activate();
#if UNITY_MODULES_AUDIO
            InteractionAudio.Play(Interaction.Error, transform, null);
#endif
#if UNITY_MODULES_ANIMATION
            return animator.PlayCoroutine("Lost");
#else
            return null;
#endif
        }

        /// <summary>
        /// Set the attenion director to targeting a specific object, withouth showing the optional
        /// center image.
        /// </summary>
        /// <returns>The target coroutine.</returns>
        /// <param name="target">Target.</param>
        public IEnumerator SetTargetCoroutine(Component target)
        {
            var found = false;
            SetTarget(target, () => found = true);
            return new WaitUntil(() => found);
        }

        /// <summary>
        /// Set the attenion director to targeting a specific object, withouth showing the optional
        /// center image.
        /// </summary>
        /// <param name="target">       Target.</param>
        /// <param name="onTargetFound">On target found.</param>
        public void SetTarget(Component target, Action onTargetFound)
        {
            this.Activate();
            if (target != null)
            {
                targetTrans = target.transform;
                showImage = false;
                showPointer = true;
            }
            else
            {
                showImage = true;
                showPointer = false;
            }

            this.onTargetFound = onTargetFound;
        }

        /// <summary>
        /// Updates the pointer graphics if the option to show them has changed, and checks to see
        /// how close we are to the target, updating the location of the arrow graphic if we haven't
        /// found it yet, or activating the callback function if we have.
        /// </summary>
        public void Update()
        {
            if (showImage != wasShowImage)
            {
                image.SetActive(showImage);
            }

            if (showPointer != wasShowPointer)
            {
                pointer.SetActive(showPointer);
            }

            var origin = transform.position;
            var fwd = transform.forward;

            if (!targetTrans.IsActivated())
            {
                targetTrans = null;
            }

            if (Target != Vector3.zero)
            {
                var dir = (Target - origin).normalized;
                var right = Vector3.Dot(dir, transform.right) * transform.right;
                var up = Vector3.Dot(dir, transform.up) * transform.up;
                var point = up + right;
                pointer.rotation = Quaternion.LookRotation(point, transform.forward);

                var toTarget = DisplayManager.MainCamera.WorldToViewportPoint(Target);
                toTarget.z = 0;
                var targetDistance = toTarget.magnitude / 2;
                arrow.localPosition = Mathf.Clamp01(targetDistance) * tagRange;

                var distance = 1 - Vector3.Dot(fwd, dir);
                if (distance < minDistance && onTargetFound != null)
                {
                    // The callback could call AttentionDirector::Target, so if we call onTargetFound
                    // before nulling it, we'll break the callback.
                    var cb = onTargetFound;
                    onTargetFound = null;
                    cb();
                }
            }

            if (Application.isEditor && UnityEngine.Input.GetKeyDown(KeyCode.F))
            {
                var mouse = ComponentExt.FindAny<RunningMovement>();
                if (mouse != null)
                {
                    mouse.transform.LookAt(Target, Vector3.up);
                }
            }

            wasShowImage = showImage;
            wasShowPointer = showPointer;
        }

        /// <summary>
        /// The callback to execute when the target is found.
        /// </summary>
        private Action onTargetFound;

        /// <summary>
        /// The optional object we are targeting.
        /// </summary>
        private Transform targetTrans;

        /// <summary>
        /// The object we were targeting in the last frame.
        /// </summary>
        private Transform lastTargetTrans;

        /// <summary>
        /// The position in space that we are targeting, if there is no distinct target object.
        /// </summary>
        private Vector3 targetPos;

        /// <summary>
        /// The position in space that we were targeting last frame.
        /// </summary>
        private Vector3 lastTargetPosition;

        /// <summary>
        /// The pointer graphic.
        /// </summary>
        private Transform pointer;

        /// <summary>
        /// The arrow on the edge of the pointer graphic, pointer the user to the direction to turn.
        /// </summary>
        private Transform arrow;

        /// <summary>
        /// The offset from the center at which the arrow is located.
        /// </summary>
        private Vector3 tagRange;

        /// <summary>
        /// An optional image to display in the center of the attention director. Maybe use it for
        /// displaying instructions to the user the first time they see the attention director.
        /// </summary>
        private Transform image;

        /// <summary>
        /// Whether or not to show the optional center image.
        /// </summary>
        private bool showImage;

        /// <summary>
        /// Whether or not to show the pointer graphic at all.
        /// </summary>
        private bool showPointer;

        /// <summary>
        /// Whether or not the optional center image was visible in the last frame.
        /// </summary>
        private bool wasShowImage;

        /// <summary>
        /// Whether or not the pointer graphic was visible in the last frame.
        /// </summary>
        private bool wasShowPointer;

        /// <summary>
        /// An optional animation object to show different attention director states.
        /// </summary>
#if UNITY_MODULES_ANIMATION
        private AbstractAnimator animator;
#endif
    }
}
