#if UNITY_XR_MAGICLEAP

using Juniper.Input;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace Juniper.Input.Pointers.Motion
{
    public abstract class MagicLeapHand
        : AbstractHandTracker<MLHandType, MLHandKeyPose, MagicLeapHandConfiguration>
    {
        private MLHandKeyPose currentPose;
        private MLHandKeyPose lastPose = MLHandKeyPose.NoHand;
        private MLHand handObj;
        private float lastSeenTime;

        public override bool IsConnected
        {
            get
            {
                var visible = handObj?.IsVisible == true;
                if (visible)
                {
                    lastSeenTime = Time.unscaledTime;
                }
                else
                {
                    visible = (Time.unscaledTime - lastSeenTime) < 0.2f;
                }
                return visible;
            }
        }

        public override bool IsDominantHand
        {
            get
            {
                return Hand == Hands.Right;
            }
        }

        private IEnumerable<MLKeyPoint> KeyPoints
        {
            get
            {
                foreach (var point in handObj.Wrist.KeyPoints)
                {
                    yield return point;
                }
                foreach (var point in handObj.Thumb.KeyPoints)
                {
                    yield return point;
                }
                foreach (var point in handObj.Index.KeyPoints)
                {
                    yield return point;
                }
                foreach (var point in handObj.Middle.KeyPoints)
                {
                    yield return point;
                }
                foreach (var point in handObj.Ring.KeyPoints)
                {
                    yield return point;
                }
                foreach (var point in handObj.Pinky.KeyPoints)
                {
                    yield return point;
                }
            }
        }

        private Transform[] bones;

        public override void Awake()
        {
            if (Hand == Hands.None)
            {
                this.Destroy();
            }
            else
            {
                base.Awake();

                if (!MLHands.IsStarted)
                {
                    var result = MLHands.Start();
                    if (result.IsOk)
                    {
                        var poses = (from hand in Find.All<MagicLeapHand>()
                                     from button in hand.nativeButtons.Buttons
                                     select button).ToArray();
                        MLHands.KeyPoseManager.EnableKeyPoses(poses, true);
                    }
                }

                handObj = PointerConfig[NativeHandID];

                bones = KeyPoints.Select((point, i) =>
                    this.Ensure<Transform>(
                        $"Bone{i}",
                        () =>
                        {
                            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            sphere.transform.localScale = 0.005f * Vector3.one;
                            if (point.IsValid)
                            {
                                sphere.transform.localPosition = point.Position;
                            }
                            var collid = sphere.GetComponent<Collider>();
                            collid.isTrigger = true;
                            sphere.Deactivate();
                            return sphere;
                        }).Value).ToArray();
            }
        }

        public void OnDestroy()
        {
            if (MLHands.IsStarted)
            {
                MLHands.Stop();
            }
        }

        private Vector3[] handShape = new Vector3[4];

        private Quaternion GetOrientation(MLKeyPoint wristCenter, params MLKeyPoint[] hand)
        {
            if (handShape.Length != hand.Length)
            {
                handShape = new Vector3[hand.Length];
            }

            for (var i = 0; i < hand.Length; ++i)
            {
                handShape[i] = (hand[i].Position - wristCenter.Position).normalized;
            }

            var forward = Vector3.zero;
            var up = Vector3.zero;
            for (var i = 0; i < handShape.Length - 1; ++i)
            {
                var j = i + 1;
                var a = handShape[i];
                var b = handShape[j];
                forward += Vector3.Cross(a, b);
                up += a;
            }

            if (IsRightHand)
            {
                forward *= -1;
            }

            forward.Normalize();
            up.Normalize();

            var q = Quaternion.LookRotation(forward, up);
            var right = Vector3.Cross(forward, up);
            var twist = Quaternion.AngleAxis(twistHand, forward);
            var raise = Quaternion.AngleAxis(raiseHand, right);
            var rot = Quaternion.AngleAxis(rotateHand, up);
            return raise * rot * twist * q;
        }

        public float raiseHand = 60f;

        public float rotateHand = -13f;

        public float twistHand = 15;

        [Range(0, 1)]
        public float SLERP = 0.25f;

        private static readonly Quaternion ZERO = new Quaternion(0, 0, 0, 1);

        protected override void InternalUpdate()
        {
            if (handObj.IsVisible)
            {
                lastPose = currentPose;
                currentPose = handObj.KeyPose;

                transform.localPosition = handObj.Center;
                var q = GetOrientation(
                    handObj.Wrist.Center,
                    handObj.Index.MCP,
                    handObj.Middle.MCP);

                if (q != ZERO)
                {
                    transform.localRotation = Quaternion.Slerp(transform.localRotation, q, SLERP);
                }

                var counter = 0;
                foreach (var point in KeyPoints)
                {
                    bones[counter].SetActive(point.IsValid);
                    bones[counter].position = point.Position;
                    ++counter;
                }
            }
        }

        public override bool IsButtonPressed(MLHandKeyPose button)
        {
            return button == currentPose;
        }

        public override bool IsButtonDown(MLHandKeyPose button)
        {
            return button == currentPose && button != lastPose;
        }

        public override bool IsButtonUp(MLHandKeyPose button)
        {
            return button != currentPose && button == lastPose;
        }
    }
}

#endif