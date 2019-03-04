using Juniper.Anchoring;
using Juniper.Input;
using Juniper.Input.Pointers;
using Juniper.Input.Pointers.Gaze;
using Juniper.Input.Pointers.Motion;
using Juniper.Input.Pointers.Screen;
using Juniper.Widgets;

using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_MODULES_UI

using UnityEngine.UI;

#endif

namespace Juniper
{
    [DisallowMultipleComponent]
    public class StageExtensions : MonoBehaviour, IInstallable
    {
        /// <summary>
        /// When running on systems that do not understand the relationship between the camera and
        /// the ground (marker-tracking AR, 3DOF VR), this is the height that is used for the camera
        /// off of the ground.
        /// </summary>
        [Tooltip("1.75 meters is about 5 feet 9 inches.")]
        public float defaultAvatarHeight = 1.75f;

#if UNITY_MODULES_PHYSICS

        /// <summary>
        /// When set to true, colliders and rigid bodies will be added to the stage and camera to
        /// simulate a "body" that can be affected by gravity.
        /// </summary>
        public bool usePhysicsBasedMovement = true;

        /// <summary>
        /// A friction material for the player's avatar.
        /// </summary>
        public PhysicMaterial shoes;

        /// <summary>
        /// Physics for the avatar's body.
        /// </summary>
        [HideInInspector]
        public Rigidbody BodyPhysics;

        private CapsuleCollider BodyShape;

        public bool useGravity = true;
#endif
        private const string ENABLE_GAZE_KEY = "GazePointer";
        private const string ENABLE_MOUSE_KEY = "MousePointer";
        private const string ENABLE_TOUCH_KEY = "TouchPointers";
        private const string ENABLE_HANDS_KEY = "HandPointers";
        private const string ENABLE_CONTROLLERS_KEY = "MotionControllers";

#if UNITY_MODULES_UI
        public Toggle enableControllersToggle;
        public Toggle enableMouseToggle;
        public Toggle enableTouchToggle;
        public Toggle enableHandsToggle;
        public Toggle enableGazeToggle;
#endif

        private GazePointer gazePointer;
        private Mouse mouse;
        private TouchPoint[] touches;
        private MotionController[] motionControllers;
        private HandTracker[] handTrackers;
        private Transform Head;
        private Transform Hands;
        private Transform Body;

        public virtual void Awake()
        {
            Install(false);

#if UNITY_MODULES_UI
            if (enableGazeToggle != null)
            {
                enableGazeToggle.onValueChanged.AddListener(EnableGaze);
                enableGazeToggle.isOn = PlayerPrefs.GetInt(ENABLE_GAZE_KEY, 0) == 1;
                gazePointer.SetActive(enableGazeToggle.isOn);
            }

            if (enableHandsToggle != null)
            {
                enableHandsToggle.onValueChanged.AddListener(EnableHands);
                enableHandsToggle.isOn = PlayerPrefs.GetInt(ENABLE_HANDS_KEY, 0) == 1;
                foreach (var handTracker in handTrackers)
                {
                    handTracker.SetActive(enableHandsToggle.isOn);
                }
            }

            if (enableControllersToggle != null)
            {
                enableControllersToggle.onValueChanged.AddListener(EnableControllers);
                enableControllersToggle.isOn = PlayerPrefs.GetInt(ENABLE_CONTROLLERS_KEY, 1) == 1;
                foreach (var motionController in motionControllers)
                {
                    motionController.SetActive(enableControllersToggle.isOn);
                }
            }

            if (enableMouseToggle != null)
            {
                enableMouseToggle.onValueChanged.AddListener(EnableMouse);
                enableMouseToggle.isOn = PlayerPrefs.GetInt(ENABLE_MOUSE_KEY, 1) == 1;
                mouse.SetActive(enableMouseToggle.isOn);
            }

            if (enableTouchToggle != null)
            {
                enableTouchToggle.onValueChanged.AddListener(EnableTouch);
                enableTouchToggle.isOn = PlayerPrefs.GetInt(ENABLE_TOUCH_KEY, Application.isMobilePlatform ? 1 : 0) == 1;
                foreach (var touch in touches)
                {
                    touch.SetActive(enableTouchToggle.isOn);
                }
            }
#endif

#if ARKIT || ARCORE || HOLOLENS || MAGIC_LEAP
            usePhysicsBasedMovement = false;
#endif

#if UNITY_MODULES_PHYSICS
            BodyShape.enabled = usePhysicsBasedMovement;
#endif
        }

        private void EnableMouse(bool value)
        {
            mouse.SetActive(value);
            PlayerPrefs.SetInt(ENABLE_MOUSE_KEY, value ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void EnableGaze(bool value)
        {
            gazePointer.SetActive(value);
            PlayerPrefs.SetInt(ENABLE_GAZE_KEY, value ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void EnableTouch(bool value)
        {
            foreach (var touch in touches)
            {
                touch.SetActive(value);
            }
            PlayerPrefs.SetInt(ENABLE_TOUCH_KEY, value ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void EnableHands(bool value)
        {
            foreach (var handTracker in handTrackers)
            {
                handTracker.SetActive(value);
            }
            PlayerPrefs.SetInt(ENABLE_HANDS_KEY, value ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void EnableControllers(bool value)
        {
            foreach (var motionController in motionControllers)
            {
                motionController.SetActive(value);
            }
            PlayerPrefs.SetInt(ENABLE_CONTROLLERS_KEY, value ? 1 : 0);
            PlayerPrefs.Save();
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

        private static GameObject MakeShadowCaster(PrimitiveType type, Vector3? scale, Vector3? position = null)
        {
            var caster = GameObject.CreatePrimitive(type);
            caster.transform.localScale = scale ?? Vector3.one;
            caster.transform.localPosition = position ?? Vector3.zero;

            var casterRenderer = caster.GetComponent<Renderer>();
            casterRenderer.lightProbeUsage = LightProbeUsage.Off;
            casterRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            casterRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            casterRenderer.receiveShadows = false;
            casterRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;

#if UNITY_MODULES_PHYSICS
            caster.GetComponent<Collider>()?.Destroy();
#endif
            return caster;
        }

        public void Install(bool reset)
        {
            reset &= Application.isEditor;

            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            Head = this.EnsureTransform("Head (Camera)");

            var headShadow = Head.EnsureTransform("HeadShadow", () =>
                MakeShadowCaster(
                    PrimitiveType.Sphere,
                    new Vector3(0.384284f, 0.3163f, 0.3831071f)));

            headShadow.Value.EnsureTransform("Goggles", () =>
                MakeShadowCaster(
                    PrimitiveType.Cube,
                    new Vector3(0.85f, 0.5f, 0.5f),
                    new Vector3(0, 0, 0.311f)));

            Hands = this.EnsureTransform("Hands");

            Body = this.EnsureTransform("Body", () =>
                MakeShadowCaster(
                    PrimitiveType.Capsule,
                    new Vector3(0.5f, 0.5f * defaultAvatarHeight, 0.5f)));

#if UNITY_MODULES_PHYSICS
            BodyShape = this.EnsureComponent<CapsuleCollider>((bodyShape) =>
            {
                bodyShape.SetMaterial(shoes);
                bodyShape.height = defaultAvatarHeight;
                bodyShape.radius = 0.25f;
                bodyShape.direction = (int)CartesianAxis.Y;
            });

            BodyPhysics = this.EnsureComponent<Rigidbody>((phys) =>
            {
                phys.mass = 80;
                phys.constraints = RigidbodyConstraints.FreezeRotation;
            });

            BodyPhysics.useGravity = false;
            var grounder = BodyPhysics.EnsureComponent<Grounded>().Value;
            grounder.WhenGrounded(() =>
            {
                BodyPhysics.useGravity = useGravity;
                grounder.Destroy();
            });
#endif

            this.WithLock(() =>
            {
                gazePointer = MakePointer<GazePointer>(Head, "GazePointer");
                mouse = MakePointer<Mouse>(Head, "Mouse");

                touches = new TouchPoint[10];
                for (var i = 0; i < touches.Length; ++i)
                {
                    touches[i] = MakePointer<TouchPoint>(Head, "Touches/TouchPoint" + i);
                    touches[i].fingerID = i;
                }

                motionControllers = MotionController.MakeMotionControllers(name =>
                    MakePointer<MotionController>(Hands, name));

                handTrackers = HandTracker.MakeHandTrackers(name =>
                    MakePointer<HandTracker>(Hands, name));
            });

            this.RemoveComponent<AbstractVelocityLocomotion>();
            gameObject.AddComponent<DefaultLocomotion>();
        }

        public void Uninstall()
        {
        }

        public T MakePointer<T>(Transform parent, string path)
            where T : Component, IPointerDevice
        {
            var trans = parent.EnsureTransform(path).Value;
            return trans.EnsureComponent<T>().Value;
        }

        public void RotateView(Quaternion dQuat, float minX, float maxX)
        {
            var quat = Head.rotation * dQuat;
            var eul = quat.eulerAngles;

            transform.localRotation = Quaternion.AngleAxis(eul.y, Vector3.up);

            var x = eul.x;
            if (x > 180)
            {
                x -= 360;
            }

            x = Mathf.Max(minX, Mathf.Min(maxX, x));

            Head.localRotation = Quaternion.AngleAxis(x, Vector3.right);
        }

        private Vector3 velocity;

        public void SetVelocity(Vector3 v)
        {
            velocity = Body.rotation * v;

#if UNITY_MODULES_PHYSICS
            if (usePhysicsBasedMovement)
            {
                velocity.y = BodyPhysics.velocity.y;
            }
#endif
        }

        public void FixedUpdate()
        {
#if UNITY_MODULES_PHYSICS
            if (usePhysicsBasedMovement)
            {
                var acceleration = (velocity - BodyPhysics.velocity) / Time.fixedDeltaTime;
                BodyPhysics.AddForce(acceleration, ForceMode.Acceleration);
            }
            else
            {
#endif
                transform.position += velocity * Time.fixedDeltaTime;
#if UNITY_MODULES_PHYSICS
            }
#endif

            var userCenter = Head.position - transform.position;

#if UNITY_MODULES_PHYSICS
            if (usePhysicsBasedMovement)
            {
                var center = userCenter;
                center.y = -0.5f * defaultAvatarHeight;

                BodyShape.center = Quaternion.Inverse(transform.rotation) * center;
                // Make the hands follow the camera position, but not the rotation. On Daydream
                // systems, the system can figure out the right orientation for the controller as
                // you rotate your body in place, but it can't figure out the position relative
                // to the 6DOF tracking of the headset.
                Body.position = BodyShape.transform.position + userCenter;
            }
#else
            Body.position = userCenter;
#endif

#if !MAGIC_LEAP
            Hands.position = Body.position;
#endif
        }
    }
}