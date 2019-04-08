using System;

using Juniper.Input;
using Juniper.Unity.Haptics;
using Juniper.Unity.Statistics;
using UnityEngine;

namespace Juniper.Unity.Input.Pointers.Motion
{
    public abstract class AbstractHandedPointerConfiguration<HandIDType, ButtonIDType>
        : AbstractPointerConfiguration<ButtonIDType>
        where HandIDType : struct, IComparable
        where ButtonIDType : struct
    {
        public Type HandType
        {
            get
            {
                return typeof(HandIDType);
            }
        }

        public string MakePointerName(Hands hand)
        {
            return $"{hand}{PointerNameStub}";
        }

        protected abstract string PointerNameStub
        {
            get;
        }

        public abstract HandIDType? this[Hands hand] { get; }
    }

    public abstract class AbstractHandedPointer<HandIDType, ButtonIDType, ConfigType, HapticsType> :
        AbstractPointerDevice<ButtonIDType, HapticsType, ConfigType>,
        IHandedPointer
        where HandIDType : struct, IComparable
        where ButtonIDType : struct
        where ConfigType : AbstractHandedPointerConfiguration<HandIDType, ButtonIDType>, new()
        where HapticsType : AbstractHapticDevice
    {
        public static T[] MakeControllers<T>(Func<string, T> MakePointer)
            where T : AbstractHandedPointer<HandIDType, ButtonIDType, ConfigType, HapticsType>
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
            where T : AbstractHandedPointer<HandIDType, ButtonIDType, ConfigType, HapticsType>
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
