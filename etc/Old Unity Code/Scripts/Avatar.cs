using System;

using Juniper.Anchoring;
using Juniper.Display;
using Juniper.Input;
using Juniper.IO;
using Juniper.Units;

using UnityEngine;
using UnityEngine.Rendering;

namespace Juniper
{
    [DisallowMultipleComponent]
    public class Avatar : MonoBehaviour, IInstallable
    {
        public static void Ensure()
        {
            if (Find.Any(out JuniperSystem sys))
            {
                var sysT = sys.transform;
                var head = DisplayManager.MainCamera.transform;

                var pivot = head.EnsureParent("Pivot", sysT);
                var neck = pivot.EnsureParent("Neck", sysT);
                var avatar = neck.EnsureParent("Avatar", sysT);
                var stage = avatar.EnsureParent("Stage", sysT);
                stage.Ensure<Avatar>();
                stage.transform.SetParent(sysT, false);
            }
        }

        /// <summary>
        /// When running on systems that do not understand the relationship between the camera and
        /// the ground (marker-tracking AR, 3DOF VR), this is the height that is used for the camera
        /// off of the ground.
        /// </summary>
        [Tooltip("1.75 meters is about 5 feet 9 inches.")]
        public float defaultAvatarHeight = (5f + 9f.Convert(UnitOfMeasure.Inches, UnitOfMeasure.Feet))
            .Convert(UnitOfMeasure.Feet, UnitOfMeasure.Meters);

        public Vector3 UserTop
        {
            get
            {
                return Head.position;
            }
        }

        public Vector3 UserBottom
        {
            get
            {
                return Head.position - FullUp;
            }
        }

        private Vector3 LocalUserTop
        {
            get
            {
                return UserTop - transform.position;
            }
        }

        /// <summary>
        /// Tells us where to put the stage. This will be the <see cref="defaultAvatarHeight"/>
        /// on 3DOF systems, and 0 on 6DOF systems;
        /// </summary>
        private float AvatarHeight
        {
            get
            {
                return LocalUserTop.y;
            }
        }

        private Vector3 FullUp
        {
            get
            {
                return AvatarHeight * Vector3.up;
            }
        }

        private float HalfHeight
        {
            get
            {
                return 0.5f * AvatarHeight;
            }
        }

        private Vector3 HalfUp
        {
            get
            {
                return HalfHeight * Vector3.up;
            }
        }

        private Vector3 LocalUserCenter
        {
            get
            {
                return LocalUserTop - HalfUp;
            }
        }

        public Vector3 UserCenter
        {
            get
            {
                return UserTop - HalfUp;
            }
        }

#if UNITY_MODULES_PHYSICS

        /// <summary>
        /// A friction material for the player's avatar.
        /// </summary>
        public PhysicMaterial shoes;

        /// <summary>
        /// Physics for the avatar's body.
        /// </summary>
        [HideInNormalInspector]
        public Rigidbody BodyPhysics;

        [HideInNormalInspector]
        [SerializeField]
        private CapsuleCollider bodyShape;

#endif

        [HideInNormalInspector]
        [SerializeField]
        private Transform avatar;

        [HideInNormalInspector]
        [SerializeField]
        public Transform GroundPlane;

        [HideInNormalInspector]
        [SerializeField]
        private Transform shoulders;

        [HideInNormalInspector]
        [SerializeField]
        private Transform neck;

        [HideInNormalInspector]
        [SerializeField]
        private Transform pivot;

        [HideInNormalInspector]
        [SerializeField]
        private Transform headShadow;

        [HideInNormalInspector]
        [SerializeField]
        private Transform body;

        public Transform Head
        {
            get; private set;
        }

        public Transform Hands
        {
            get; private set;
        }

        private bool isIndependentHead;

        public bool IndependentHead
        {
            get
            {
                return isIndependentHead;
            }
            set
            {
                isIndependentHead = value;
                body.localPosition = -HalfUp;
            }
        }

        private Grounded grounder;

        private Vector3 velocity;

        public virtual void Awake()
        {
            Install(false);

            SetCasters(headShadow.GetComponentsInChildren<Renderer>());
            SetCasters(body.GetComponentsInChildren<Renderer>());
        }

        private static void SetCasters(Renderer[] casters)
        {
            foreach (var caster in casters)
            {
                caster.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }
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

        private static GameObject MakeShadowCaster(PrimitiveType type, Vector3? scale)
        {
            var caster = GameObject.CreatePrimitive(type);
            caster.transform.localScale = scale ?? Vector3.one;

            var casterRenderer = caster.GetComponent<Renderer>();
            casterRenderer.lightProbeUsage = LightProbeUsage.Off;
            casterRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            casterRenderer.shadowCastingMode = ShadowCastingMode.On;
            casterRenderer.receiveShadows = false;
            casterRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;

#if UNITY_MODULES_PHYSICS
            caster.Remove<Collider>();
#endif
            return caster;
        }

        public void Install(bool reset)
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            GroundPlane = transform.Find("GroundPlane");

            if(GroundPlane != null 
                && !GroundPlane.HasComponent<Collider>())
            {
                GroundPlane.gameObject.DestroyImmediate();
                GroundPlane = null;
            }

            if(GroundPlane == null)
            {
                GroundPlane = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;
                GroundPlane.parent = transform;
                GroundPlane.name = "GroundPlane";
                GroundPlane.localScale = new Vector3(100, 100, 1);
                GroundPlane.localRotation = Quaternion.Euler(90, 0, 0);
            }

            GroundPlane.Remove<Renderer>();
            GroundPlane.Deactivate();

            Head = DisplayManager.MainCamera.transform;
            {
                Head.localPosition = Vector3.zero;

                if (Head.parent == null)
                {
                    Head.parent = new GameObject("Pivot").transform;
                }
                pivot = Head.parent;
                pivot.localPosition = Vector3.zero;

                if (pivot.parent == null)
                {
                    pivot.parent = new GameObject("Neck").transform;
                }
                neck = pivot.parent;
                neck.localPosition = defaultAvatarHeight * Vector3.up;

                if (neck.parent == null)
                {
                    neck.parent = new GameObject("Avatar").transform;
                }
                avatar = neck.parent;
                avatar.localPosition = Vector3.zero;

                if (avatar.parent != transform)
                {
                    avatar.parent = transform;
                }

                headShadow = Head.Ensure<Transform>("HeadShadow", new Func<GameObject>(MakeHead));
                var goggles = headShadow.Ensure<Transform>("Goggles", new Func<GameObject>(MakeGoogles));
                goggles.Value.localPosition = new Vector3(0, 0, 0.311f);

                shoulders = avatar.Ensure<Transform>("Shoulders");
                {
                    Hands = shoulders.Ensure<Transform>("Hands");
                    body = shoulders.Ensure<Transform>("Body", new Func<GameObject>(MakeBody));

#if UNITY_MODULES_PHYSICS
                    var bs = shoulders.Ensure<CapsuleCollider>();
                    if (bs.IsNew)
                    {
                        bs.Value.SetMaterial(shoes);
                        bs.Value.height = AvatarHeight;
                        bs.Value.radius = 0.25f;
                        bs.Value.direction = (int)CartesianAxis.Y;
                    }
                    bodyShape = bs;
#endif
                }
            }

#if UNITY_MODULES_PHYSICS
            var bp = avatar.Ensure<Rigidbody>();
            if (bp.IsNew)
            {
                bp.Value.mass = 80;
                bp.Value.constraints = RigidbodyConstraints.FreezeRotation;
            }
            BodyPhysics = bp;
            BodyPhysics.useGravity = false;
            BodyPhysics.isKinematic = true;
            BodyPhysics.velocity = Vector3.zero;
            BodyPhysics.Ensure<RunningMovement>();

            grounder = avatar.Ensure<Grounded>();
            grounder.GroundFound += DestroyGrounder;
#endif

            SetBodyPositionAndShape();
        }

        private GameObject MakeBody()
        {
            return MakeShadowCaster(PrimitiveType.Capsule,
                new Vector3(0.5f, HalfHeight, 0.5f));
        }

        private GameObject MakeGoogles()
        {
            return MakeShadowCaster(PrimitiveType.Cube,
                new Vector3(HalfHeight, 0.5f, 0.5f));
        }

        private static GameObject MakeHead()
        {
            return MakeShadowCaster(PrimitiveType.Sphere,
                new Vector3(0.384284f, 0.3163f, 0.3831071f));
        }

        private void DestroyGrounder()
        {
            grounder.GroundFound -= DestroyGrounder;
            grounder.DestroyImmediate();
            grounder = null;

#if UNITY_MODULES_PHYSICS
            BodyPhysics.useGravity = true;
            BodyPhysics.isKinematic = false;
            BodyPhysics.velocity = Vector3.zero;
#endif
        }

        private void SetBodyPositionAndShape()
        {
            body.localScale = new Vector3(0.5f, HalfHeight, 0.5f);
#if UNITY_MODULES_PHYSICS
            var center = LocalUserCenter;
            center.y -= AvatarHeight;
            bodyShape.center = Quaternion.Inverse(transform.rotation) * center;
            bodyShape.height = AvatarHeight;
#endif
        }

        public void Uninstall()
        {
        }

        public void RotateView(Quaternion dQuat, float minX = -180, float maxX = 180)
        {
            SetViewRotation(Head.rotation * dQuat, minX, maxX);
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetViewRotation(Quaternion quat, float minX = -180, float maxX = 180)
        {
            var eul = quat.eulerAngles;

            avatar.localRotation = Quaternion.AngleAxis(eul.y, Vector3.up);

            var x = eul.x;
            if (x > 180)
            {
                x -= 360;
            }

            x = Mathf.Max(minX, Mathf.Min(maxX, x));

            pivot.localRotation = Quaternion.AngleAxis(x, Vector3.right);
        }

        public Vector3 Velocity
        {
            get
            {
                return velocity;
            }

            set
            {
#if UNITY_MODULES_PHYSICS
                var falling = BodyPhysics.velocity.y;
#endif
                velocity = value;

#if UNITY_MODULES_PHYSICS
                velocity.y = falling;
#endif
            }
        }

        public void FixedUpdate()
        {
#if UNITY_MODULES_PHYSICS
            var acceleration = (velocity - BodyPhysics.velocity) / Time.fixedDeltaTime;
            BodyPhysics.AddForce(acceleration, ForceMode.Acceleration);
#else
            avatar.position += velocity * Time.fixedDeltaTime;
#endif

            SetBodyPositionAndShape();
        }
    }
}