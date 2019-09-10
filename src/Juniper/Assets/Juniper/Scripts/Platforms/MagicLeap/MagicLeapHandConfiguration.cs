#if UNITY_XR_MAGICLEAP

using Juniper.Input;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.XR.MagicLeap;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Input.Pointers.Motion
{
    public class MagicLeapHandConfiguration : AbstractHandTrackerConfiguration<MLHandType, MLHandKeyPose>
    {
        public MagicLeapHandConfiguration()
        {
            AddButton(MLHandKeyPose.Pinch, InputButton.Left);
            AddButton(MLHandKeyPose.Thumb, InputButton.Middle);
            AddButton(MLHandKeyPose.Ok, InputButton.Right);
        }

        public override MLHandType? this[Hands hand]
        {
            get
            {
                switch (hand)
                {
                    case Hands.Left:
                    return MLHandType.Left;

                    case Hands.Right:
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