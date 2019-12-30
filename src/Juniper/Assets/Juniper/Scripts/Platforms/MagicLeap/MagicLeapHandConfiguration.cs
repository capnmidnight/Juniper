#if UNITY_XR_MAGICLEAP

using Juniper.Input;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace Juniper.Input.Pointers.Motion
{
    public class MagicLeapHandConfiguration : AbstractHandTrackerConfiguration<MLHandType, MLHandKeyPose>
    {
        public MagicLeapHandConfiguration()
        {
            AddButton(MLHandKeyPose.Pinch, KeyCode.Mouse0);
            AddButton(MLHandKeyPose.Ok, KeyCode.Mouse1);
            AddButton(MLHandKeyPose.Thumb, KeyCode.Mouse2);
        }

        public override MLHandType? this[Hand hand]
        {
            get
            {
                switch (hand)
                {
                    case Hand.Left:
                    return MLHandType.Left;

                    case Hand.Right:
                    return MLHandType.Right;

                    default:
                    return null;
                }
            }
        }

        public MLHand this[MLHandType? hand]
        {
            get
            {
                switch (hand)
                {
                    case MLHandType.Left:
                    return MLHands.Left;

                    case MLHandType.Right:
                    return MLHands.Right;

                    default:
                    return null;
                }
            }
        }
    }
}

#endif