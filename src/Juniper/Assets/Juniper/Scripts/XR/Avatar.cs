using System.Linq;
using Juniper.Unity.Anchoring;
using Juniper.Unity.Display;
using Juniper.Unity.Input;
using Juniper.Unity.Widgets;

using UnityEngine;
using UnityEngine.Rendering;

namespace Juniper.Unity
{
    [DisallowMultipleComponent]
    public class Avatar : MonoBehaviour, IInstallable
    {
        /// <summary>
        /// When running on systems that do not understand the relationship between the camera and
        /// the ground (marker-tracking AR, 3DOF VR), this is the height that is used for the camera
        /// off of the ground.
        /// </summary>
        [Tooltip("1.75 meters is about 5 feet 9 inches.")]
        public float defaultAvatarHeight = (5f + 9f.Convert(UnitOfMeasure.Inches, UnitOfMeasure.Feet))
            .Convert(UnitOfMeasure.Feet, UnitOfMeasure.Meters);

        private Vector3 LocalUserTop
        {
            get
            {
                return Head.localPosition;
            }
        }

        public Vector3 UserTop
        {
            get
            {
                return Head.position;
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
        [HideInInspector]
        public Rigidbody BodyPhysics;

        private CapsuleCollider BodyShape;
#endif

        public Transform Shoulders
        {
            get; private set;
        }

        public Transform Head
        {
            get; private set;
        }

        private Transform HeadShadow;

        public Transform Hands
        {
            get; private set;
        }

        public Transform Body
        {
            get; private set;
        }

        private Grounded grounder;

        private Vector3 velocity;

        public virtual void Awake()
        {
            Install(false);

            var casters = HeadShadow.GetComponentsInChildren<Renderer>()
                .Union(Body.GetComponentsInChildren<Renderer>());
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
            caster.GetComponent<Collider>()?.Destroy();
#endif
            return caster;
        }

        public bool Install(bool reset)
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            Head = DisplayManager.MainCamera.transform;

            Shoulders = this.Ensure<Transform>("Shoulders");

            var _ = DisplayManager.EventCamera; // force the event camera to setup

            HeadShadow = Head.Ensure<Transform>("HeadShadow", () =>
                MakeShadowCaster(
                    PrimitiveType.Sphere,
                    new Vector3(0.384284f, 0.3163f, 0.3831071f)));

            var goggles = HeadShadow.Ensure<Transform>("Goggles", () =>
                MakeShadowCaster(
                    PrimitiveType.Cube,
                    new Vector3(HalfHeight, 0.5f, 0.5f)));
            goggles.Value.localPosition = new Vector3(0, 0, 0.311f);

            Hands = Shoulders.Ensure<Transform>("Hands");

            Body = Shoulders.Ensure<Transform>("Body", () =>
                MakeShadowCaster(
                    PrimitiveType.Capsule,
                    new Vector3(0.5f, HalfHeight, 0.5f)));

#if UNITY_MODULES_PHYSICS
            var bs = Shoulders.Ensure<CapsuleCollider>();
            if (bs.IsNew)
            {
                bs.Value.SetMaterial(shoes);
                bs.Value.height = AvatarHeight;
                bs.Value.radius = 0.25f;
                bs.Value.direction = (int)CartesianAxis.Y;
            }
            BodyShape = bs;

            var bp = this.Ensure<Rigidbody>();
            if (bp.IsNew)
            {
                bp.Value.mass = 80;
                bp.Value.constraints = RigidbodyConstraints.FreezeRotation;
            }
            BodyPhysics = bp;
            BodyPhysics.useGravity = false;
            BodyPhysics.isKinematic = true;
            BodyPhysics.velocity = Vector3.zero;

            grounder = this.Ensure<Grounded>();
            grounder.WhenGrounded(() =>
            {
                grounder.Destroy();
                grounder = null;
                BodyPhysics.useGravity = true;
                BodyPhysics.isKinematic = false;
                BodyPhysics.velocity = Vector3.zero;
            });
#endif

            BodyPhysics.Ensure<DefaultLocomotion>();

            SetBodyPositionAndShape();

            return true;
        }

        private void SetBodyPositionAndShape()
        {
            Shoulders.localPosition = LocalUserTop;
            Body.localScale = new Vector3(0.5f, HalfHeight, 0.5f);
#if UNITY_MODULES_PHYSICS
            var center = LocalUserCenter;
            center.y = -HalfHeight;
            BodyShape.center = Quaternion.Inverse(transform.rotation) * center;
#endif
        }

        public void Uninstall()
        {
        }

        public void SetIndependentHead(bool isIndendent)
        {
            if (!isIndendent)
            {
                Head.localPosition = defaultAvatarHeight * Vector3.up;
            }
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

        public void SetVelocity(Vector3 v)
        {
#if UNITY_MODULES_PHYSICS
            var falling = BodyPhysics.velocity.y;
#endif

            velocity = v;

#if UNITY_MODULES_PHYSICS
            velocity.y = falling;
#endif
        }

        public void FixedUpdate()
        {
#if UNITY_MODULES_PHYSICS
            var acceleration = (velocity - BodyPhysics.velocity) / Time.fixedDeltaTime;
            BodyPhysics.AddForce(acceleration, ForceMode.Acceleration);
#else
            transform.position += velocity * Time.fixedDeltaTime;
#endif

            SetBodyPositionAndShape();
        }
    }
}
