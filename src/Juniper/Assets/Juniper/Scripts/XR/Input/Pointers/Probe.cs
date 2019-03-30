using System.Linq;

using Juniper.Unity.Display;
using Juniper.Unity.Input.Pointers.Screen;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Juniper.Unity.Input.Pointers
{
    /// <summary>
    /// AKA a Cursor, but I don't like the word Cursor (reminds me too much of SQL).
    /// </summary>
    public class Probe : MonoBehaviour, IInstallable, IProbe
    {
        /// <summary>
        /// The size of the probe ring when the <see cref="State"/> is <see cref="Appearance.None"/>.
        /// </summary>
        public Vector3 ringStartScale;

        /// <summary>
        /// The size of the probe center when the <see cref="State"/> is <see cref="Appearance.None"/>.
        /// </summary>
        public Vector3 centerStartScale;

        /// <summary>
        /// The size the whole cursor should reach when <see cref="State"/> is <see cref="Appearance.None"/>.
        /// </summary>
        public Vector2 SizeOnNone = Vector2.one;

        /// <summary>
        /// The size the whole cursor should reach when <see cref="State"/> is <see cref="Appearance.HitObject"/>
        /// </summary>
        public Vector2 SizeOnHit = 1.1f * Vector2.one;

        /// <summary>
        /// The size the whole cursor should reach when <see cref="State"/> is <see cref="Appearance.Pressing"/>.
        /// </summary>
        public Vector2 SizeOnPress = 0.9f * Vector2.one;

        /// <summary>
        /// The amount of time, in seconds, it should take to transition between appearance states.
        /// </summary>
        public float TransitionSeconds = 0.25f;

        /// <summary>
        /// Set to true if this probe is capable of press gestures.
        /// </summary>
        public bool canPress = true;

        /// <summary>
        /// Set to true if this probe is capable of gaze gestures.
        /// </summary>
        public bool canGaze;

        public bool CanGaze
        {
            get
            {
                return canGaze;
            }
            set
            {
                canGaze = value;
            }
        }

        /// <summary>
        /// The animation states of the probe, for indicating selections and potential interactions.
        /// </summary>
        public enum Appearance
        {
            /// <summary>
            /// Nothing is under the pointer
            /// </summary>
            None,

            /// <summary>
            /// The pointer is indicating an object
            /// </summary>
            HitObject,

            /// <summary>
            /// The pointer is indicating an object and the pimary interaction button has been pressed.
            /// </summary>
            Pressing
        }

        /// <summary>
        /// The current target shape for the probe.
        /// </summary>
        private Appearance State;

        /// <summary>
        /// The target shape for the probe from the last frame.
        /// </summary>
        private Appearance? lastState;

        /// <summary>
        /// The size the probe should grow/shrink to.
        /// </summary>
        private Vector2 targetScale;

        /// <summary>
        /// The size the probe ring should grow/shrink to.
        /// </summary>
        private Vector3 ringTargetScale;

        /// <summary>
        /// The size the probe center should grow/shrink to.
        /// </summary>
        private Vector3 centerTargetScale;

        /// <summary>
        /// The amount of time left, in seconds, in the appearance transition.
        /// </summary>
        private float timeLeft;

        public Transform Cursor
        {
            get; private set;
        }

        /// <summary>
        /// The outer portion of the probe.
        /// </summary>
        private Transform ring;

        /// <summary>
        /// The inner portion of the probe.
        /// </summary>
        private Transform center;

        /// <summary>
        /// The chunks that show the progress of the gaze gesture.
        /// </summary>
        private Transform[] pips;

        private LineRenderer line;

        public Material laserPointerMaterial;

        public Material LaserPointerMaterial
        {
            get
            {
                return laserPointerMaterial;
            }
            set
            {
                laserPointerMaterial = value;
            }
        }

        private Material lastLaserPointerMaterial;
        private Transform touchpoint;

        public PhysicsRaycaster Raycaster
        {
            get; private set;
        }

        public Camera EventCamera
        {
            get
            {
                return Raycaster?.eventCamera;
            }
        }

        public Vector2 TouchPoint
        {
            get
            {
                return new Vector2(
                    touchpoint.localPosition.x,
                    touchpoint.localPosition.y);
            }

            set
            {
                touchpoint.localPosition = new Vector3(value.x, 0, value.y);
            }
        }

        /// <summary>
        /// Adds a probe to the scene.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name">  </param>
        /// <returns></returns>
        public static Probe Ensure(Transform parent, string name)
        {
            name += "-probe";

            var probe = ComponentExt
                .FindAll<Probe>(p => p.name == name)
                .FirstOrDefault();

            if (probe == null)
            {
                var config = ComponentExt.FindAny<UnifiedInputModule>();
                if (config.pointerPrefab == null)
                {
                    probe = new GameObject(name).AddComponent<Probe>();
                }
                else
                {
                    probe = Instantiate(config.pointerPrefab).Ensure<Probe>();
                    probe.name = name;
                }
            }

            probe.transform.SetParent(parent, false);

            return probe;
        }

        /// <summary>
        /// Set up the probe, getting its transform, ring, center, progress pips, and their various scales.
        /// </summary>
        public void Awake()
        {
            Install(false);

            SetGaze(0);
        }

        public virtual void Reinstall()
        {
            Install(true);
        }

#if UNITY_EDITOR

        public void Reset()
        {
            Reinstall();
        }

#endif

        public bool Install(bool reset)
        {
            Cursor = transform.Ensure<Transform>("cursor");

            var pipsContainer = Cursor.Find("offset/pips");
            if (pipsContainer != null)
            {
                pips = pipsContainer.Children().ToArray();
                ring = Cursor.Find("offset/ring");
                center = Cursor.Find("offset/center");

                if (ringStartScale == Vector3.zero)
                {
                    ringStartScale = ring.localScale;
                }

                if (centerStartScale == Vector3.zero)
                {
                    centerStartScale = center.localScale;
                }
            }

            touchpoint = this.Ensure<Transform>("touchpad/touchpointTrack/touchpoint");

            return true;
        }

        public void Start()
        {
            var pointer = GetComponentInParent<IScreenDevice>();
            if (pointer != null)
            {
                // Screen Devices need to use the Main Camera as the Event Camera.
                Raycaster = DisplayManager.MainCamera.Ensure<PhysicsRaycaster>();
            }
            else
            {
                // Any other type of pointer uses its own raycaster.
                Raycaster = this.Ensure<PhysicsRaycaster>();
            }

            if (EventCamera != DisplayManager.MainCamera)
            {
                EventCamera.clearFlags = CameraClearFlags.SolidColor;
                EventCamera.backgroundColor = ColorExt.TransparentBlack;
                EventCamera.nearClipPlane = DisplayManager.MainCamera.nearClipPlane;
                EventCamera.depth = DisplayManager.MainCamera.depth - 1;
                EventCamera.allowHDR = false;
                EventCamera.allowMSAA = false;
                EventCamera.enabled = false;
            }
        }

        public void Uninstall()
        {
        }

        public void AlignProbe(Vector3 dir, Vector3 up, float maxDistance)
        {
            transform.rotation = Quaternion.LookRotation(dir, up);
            if (EventCamera != DisplayManager.MainCamera)
            {
                EventCamera.farClipPlane = maxDistance;
            }
        }

        /// <summary>
        /// Set the input state of the pointer.
        /// </summary>
        /// <param name="targeted">      whether or not we are pointing at a valid target</param>
        /// <param name="pressed">       whether or not the primary selection button has been pressed</param>
        /// <param name="targetPosition">the location at which the pointer found a target</param>
        /// <param name="targetForward"> the surface normal rotation for the pointer's target</param>
        public void SetCursor(bool targeted, bool pressed, Vector3 targetPosition, Vector3 targetForward)
        {
            Cursor.position = targetPosition;

            if (!Mathf.Approximately(targetForward.sqrMagnitude, 0))
            {
                Cursor.rotation = Quaternion.LookRotation(targetForward);
            }

            var distance = Mathf.Max(0.1f, Vector3.Distance(Cursor.position, transform.position));
            Cursor.localScale = distance * Vector3.one;

            if (!targeted)
            {
                State = Appearance.None;
            }
            else if (pressed)
            {
                State = Appearance.Pressing;
            }
            else
            {
                State = Appearance.HitObject;
            }

            if (State != lastState)
            {
                switch (State)
                {
                    case Appearance.None:
                    targetScale = SizeOnNone;
                    break;

                    case Appearance.HitObject:
                    targetScale = SizeOnHit;
                    break;

                    case Appearance.Pressing:
                    targetScale = SizeOnPress;
                    break;

                    default:
                    targetScale = Vector3.one;
                    break;
                }

                ringTargetScale = targetScale.x * ringStartScale;
                centerTargetScale = targetScale.y * centerStartScale;
                ringTargetScale.y = ringStartScale.y;
                centerTargetScale.y = centerStartScale.y;

                timeLeft = TransitionSeconds;

                lastState = State;
            }

            var p = 1 - (timeLeft / TransitionSeconds);
            ring?.SetScale(Vector3.Lerp(ringStartScale, ringTargetScale, p));
            center?.SetScale(Vector3.Lerp(centerStartScale, centerTargetScale, p));

            timeLeft = Mathf.Max(0, timeLeft - Time.deltaTime);

            if (line == null && (LaserPointerMaterial != null || LaserPointerMaterial != lastLaserPointerMaterial))
            {
                line = this.Ensure<LineRenderer>();
                line.alignment = LineAlignment.View;
                line.positionCount = 2;
                line.receiveShadows = false;
                line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

                line.widthCurve = new AnimationCurve(
                    new Keyframe(0, 1),
                    new Keyframe(1, 0));

                line.widthMultiplier = 0.01f;
                line.SetMaterial(LaserPointerMaterial);
            }
            else if (line != null && LaserPointerMaterial == null)
            {
                line.Destroy();
                line = null;
            }

            if (line != null)
            {
                line.SetPosition(0, transform.position);
                var delta = targetPosition - transform.position;
                line.SetPosition(1, transform.position + (0.333f * delta));
            }

            lastLaserPointerMaterial = LaserPointerMaterial;
        }

        /// <summary>
        /// update the progress of the gaze gesture indicator.
        /// </summary>
        /// <param name="gazeProgress">
        /// the proportion of a gaze process through which the pointer has progressed
        /// </param>
        public void SetGaze(float gazeProgress)
        {
            if (pips != null)
            {
                var numPips = Mathf.RoundToInt(gazeProgress * pips.Length);
                for (var i = 0; i < pips.Length; ++i)
                {
                    pips[i].SetActive(canGaze && i < numPips);
                }
            }
        }

        public void Destroy()
        {
            gameObject.Destroy();
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}