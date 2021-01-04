using System;
using Juniper.Haptics;
using Juniper.Mathematics;

using UnityEngine;

namespace Juniper.Input.Pointers.Motion
{
    public abstract class AbstractHandedPointer<HandIDType, ButtonIDType, ConfigType, HapticType> :
        AbstractPointerDevice<ButtonIDType, ConfigType, HapticType>,
        IHandedPointer
        where HandIDType : struct, IComparable
        where ButtonIDType : struct
        where ConfigType : AbstractHandedPointerConfiguration<HandIDType, ButtonIDType>, new()
        where HapticType : AbstractHapticDevice
    {
        public static T[] MakeControllers<T>(Func<string, T> MakePointer)
            where T : AbstractHandedPointer<HandIDType, ButtonIDType, ConfigType, HapticType>
        {
            return new[] {
                MakeMotionController(MakePointer, Hand.Left),
                MakeMotionController(MakePointer, Hand.Right)
            };
        }

        /// <summary>
        /// Create a new hand pointer object for an interaction source that hasn't yet been seen.
        /// </summary>
        private static T MakeMotionController<T>(Func<string, T> MakePointer, Hand hand)
            where T : AbstractHandedPointer<HandIDType, ButtonIDType, ConfigType, HapticType>
        {
            var pointer = MakePointer(PointerConfig.MakePointerName(hand));
#if UNITY_EDITOR
            pointer.motionFilter = ResourceExt.EditorLoadAsset<KalmanMotionFilter>("Assets/Juniper/Assets/Prefabs/Filters/handTrackingKalmanFilter.asset");
#endif
            pointer.Hand = hand;
            return pointer;
        }

        public Hand _hand;

        public virtual Hand Hand
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
                return Hand == Hand.Left;
            }
        }

        public bool IsRightHand
        {
            get
            {
                return Hand == Hand.Right;
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
