using System;

using Juniper.Mathematics;

using UnityEngine;

namespace Juniper.Input.Pointers.Motion
{
    public abstract class AbstractHandedPointer<HandIDType, ButtonIDType, ConfigType> :
        AbstractPointerDevice<ButtonIDType, ConfigType>,
        IHandedPointer
        where HandIDType : struct, IComparable
        where ButtonIDType : struct
        where ConfigType : AbstractHandedPointerConfiguration<HandIDType, ButtonIDType>, new()
    {
        public static T[] MakeControllers<T>(Func<string, T> MakePointer)
            where T : AbstractHandedPointer<HandIDType, ButtonIDType, ConfigType>
        {
            return new[] {
                MakeMotionController(MakePointer, Hands.Left),
                MakeMotionController(MakePointer, Hands.Right)
            };
        }

        /// <summary>
        /// Create a new hand pointer object for an interaction source that hasn't yet been seen.
        /// </summary>
        private static T MakeMotionController<T>(Func<string, T> MakePointer, Hands hand)
            where T : AbstractHandedPointer<HandIDType, ButtonIDType, ConfigType>
        {
            var pointer = MakePointer(PointerConfig.MakePointerName(hand));
#if UNITY_EDITOR
            pointer.motionFilter = ResourceExt.EditorLoadAsset<KalmanMotionFilter>("Assets/Juniper/Prefabs/Filters/handTrackingKalmanFilter.asset");
#endif
            pointer.Hand = hand;
            return pointer;
        }

        public Hands _hand;

        public virtual Hands Hand
        {
            get
            {
                return _hand;
            }
            set
            {
                _hand = value;
                NativeHandID = PointerConfig[value];
            }
        }

        public virtual HandIDType? NativeHandID
        {
            get; protected set;
        }

        public bool IsLeftHand
        {
            get
            {
                return Hand == Hands.Left;
            }
        }

        public bool IsRightHand
        {
            get
            {
                return Hand == Hands.Right;
            }
        }

        public abstract bool IsDominantHand
        {
            get;
        }

        public bool IsNonDominantHand
        {
            get
            {
                return !IsDominantHand;
            }
        }

        public override Vector3 WorldPoint
        {
            get
            {
                return transform.position + (MinimumPointerDistance * transform.forward);
            }
        }
    }
}
