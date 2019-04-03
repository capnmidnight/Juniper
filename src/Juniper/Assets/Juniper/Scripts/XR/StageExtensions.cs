using System.Linq;
using Juniper.Unity.Anchoring;
using Juniper.Unity.Display;
using Juniper.Unity.Input;
using Juniper.Unity.Widgets;

using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_MODULES_UI
#endif

namespace Juniper.Unity
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

        public virtual void Awake()
        {
            Install(false);

#if UNITY_MODULES_PHYSICS
            usePhysicsBasedMovement = JuniperPlatform.SupportedARMode == AugmentedRealityTypes.None;
            BodyShape.enabled = usePhysicsBasedMovement;
#endif

            var casters = HeadShadow.GetComponentsInChildren<Renderer>()
                .Union(Body.GetComponentsInChildren<Renderer>());
            foreach(var caster in casters)
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

        private static GameObject MakeShadowCaster(PrimitiveType type, Vector3? scale, Vector3? position = null)
        {
            var caster = GameObject.CreatePrimitive(type);
            caster.transform.localScale = scale ?? Vector3.one;
            caster.transform.localPosition = position ?? Vector3.zero;

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

            HeadShadow = Head.Ensure<Transform>("HeadShadow", () =>
                MakeShadowCaster(
                    PrimitiveType.Sphere,
                    new Vector3(0.384284f, 0.3163f, 0.3831071f)));

            HeadShadow.Ensure<Transform>("Goggles", () =>
                MakeShadowCaster(
                    PrimitiveType.Cube,
                    new Vector3(0.85f, 0.5f, 0.5f),
                    new Vector3(0, 0, 0.311f)));

            Hands = this.Ensure<Transform>("Hands");

            Body = this.Ensure<Transform>("Body", () =>
                MakeShadowCaster(
                    PrimitiveType.Capsule,
                    new Vector3(0.5f, 0.5f * defaultAvatarHeight, 0.5f),
                    0.5f * defaultAvatarHeight * Vector3.down));

#if UNITY_MODULES_PHYSICS
            var bs = this.Ensure<CapsuleCollider>();
            if (bs.IsNew)
            {
                bs.Value.SetMaterial(shoes);
                bs.Value.height = defaultAvatarHeight;
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
            var grounder = BodyPhysics.Ensure<Grounded>();
            grounder.Value.WhenGrounded(() =>
            {
                BodyPhysics.useGravity = useGravity;
                grounder.Destroy();
            });
#endif

            this.Remove<AbstractVelocityLocomotion>();
            gameObject.AddComponent<DefaultLocomotion>();

            return true;
        }

        public void Uninstall()
        {
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
                // systems, the system can figure out the right orientation for the controller as you
                // rotate your body in place, but it can't figure out the position relative to the
                // 6DOF tracking of the headset.
                Body.position = BodyShape.transform.position + userCenter - (0.5f * defaultAvatarHeight * Vector3.up);
            }
#else
            Body.position = userCenter;
#endif

#if !UNITY_XR_MAGICLEAP
            Hands.position = Body.position;
#endif
        }
    }
}
