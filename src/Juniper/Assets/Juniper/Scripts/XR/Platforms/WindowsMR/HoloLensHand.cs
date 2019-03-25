#if UNITY_XR_WINDOWSMR_METRO && HOLOLENS
using UnityEngine;
using UnityEngine.XR.WSA.Input;
using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Unity.Input.Pointers.Motion
{
    public class HoloLensProbeConfiguration : AbstractHandTrackerConfiguration<InteractionSourceHandedness>
    {
        public HoloLensProbeConfiguration() :
            base(InteractionSourceHandedness.Unknown, InteractionSourceHandedness.Unknown) { }
    }

    /// <summary>
    /// A motion controller or hand-tracking. Currently only implements WindowsMR.
    /// </summary>
    public abstract class HoloLensHand : AbstractWindowsMRDevice<HoloLensProbeConfiguration, Haptics.NoHaptics>
    {
        /// <summary>
        /// The interaction origin from the last frame, used for interpolating the position and
        /// smoothing it out over time.
        /// </summary>
        private Vector3 lastOrigin;

        /// <summary>
        /// How quickly the interaction origin is moving, and in what direction. Used to extrapolate
        /// the position of the origin during frames in which we don't receive an updated State frame.
        /// </summary>
        private Vector3 originVelocity;

        /// <summary>
        /// The interaction end point, i.e. through where the rays fire for the raycaster.
        /// </summary>
        private Vector3 endPoint;

        /// <summary>
        /// The interaction end point from the last frame, used for interpolating the position and
        /// smoothing it out over time.
        /// </summary>
        private Vector3 lastEndPoint;

        /// <summary>
        /// How quickly the interaction end point is moving, and in what direction. Used to
        /// extrapolate the position of the end point during frames in which we don't receive an
        /// updated State frame.
        /// </summary>
        private Vector3 endPointVelocity;

        public override void Awake()
        {
            base.Awake();

            AddButton(WindowsMRButtons.AirTap, InputButton.Left);
        }

        /// <summary>
        /// Extrapolate the position of the pointer.
        /// </summary>
        public void FixedUpdate()
        {
            transform.position += originVelocity * Time.fixedDeltaTime;
            endPoint += endPointVelocity * Time.fixedDeltaTime;
            transform.forward = (endPoint - transform.position).normalized;
        }

        /// <summary>
        /// Determine a good "forward" direction the fires from an eye through the pointer.
        /// </summary>
        /// <returns>The forward.</returns>
        /// <param name="currentPoint">Current point.</param>
        private Vector3 Forward(Vector3 currentPoint)
        {
            var camT = CameraExtensions.MainCamera.transform;
            var s = GetCurrentSide(currentPoint - camT.transform.position);
            var onLeft = s == InteractionSourceHandedness.Left;
            var onRight = s == InteractionSourceHandedness.Right;
            var horiz = onLeft ? -0.035f : onRight ? 0.03f : 0;
            var o = camT.transform.position + horiz * camT.transform.right;
            var forward = currentPoint - o;
            forward.Normalize();
            return forward;
        }

        private InteractionSourceHandedness side;

        /// <summary>
        /// Gets and sets the most recent controller State frame. On set, also decomposes the state
        /// frame into constituent parts that <see cref="UnifiedInputModule"/> can use to fire events.
        /// </summary>
        /// <value>The state of the input.</value>
        public override InteractionSourceState InputState
        {
            set
            {
                base.InputState = value;

                var pose = value.sourcePose;

                var forward = Forward(transform.position);
                var rotation = Quaternion.LookRotation(forward);
                side = GetCurrentSide(forward);

                const float vert = 0.1f;
                var horiz = side == InteractionSourceHandedness.Right ? -0.03f : 0.03f;
                const float fwd = 0.03f;
                var point = transform.position + (rotation * new Vector3(horiz, vert, fwd));

                forward = Forward(point);

                transform.position = point;
                transform.forward = forward;
                transform.rotation = stage.transform.rotation * transform.rotation;
                endPoint = transform.position + transform.forward;

                originVelocity = (transform.position - lastOrigin) / Time.deltaTime;
                endPointVelocity = (endPoint - lastEndPoint) / Time.deltaTime;

                lastOrigin = transform.position;
                lastEndPoint = endPoint;
            }
        }

        /// <summary>
        /// Figure out a side on which the
        /// </summary>
        /// <returns>The current side.</returns>
        /// <param name="forward">Forward.</param>
        private InteractionSourceHandedness GetCurrentSide(Vector3 forward)
        {
            var unrotate = Quaternion.Inverse(CameraExtensions.MainCamera.transform.rotation);
            var test = unrotate * forward;
            return test.x > 0 ? InteractionSourceHandedness.Right : InteractionSourceHandedness.Left;
        }

        public override bool IsLeftHand { get { return side == InteractionSourceHandedness.Left; } }

        public override bool IsRightHand { get { return side == InteractionSourceHandedness.Right; } }
    }
}
#endif
