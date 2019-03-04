using Juniper.Haptics;

using System;

using UnityEngine;

namespace Juniper.Input.Pointers.Motion
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

        public HandIDType? NativeHandID
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

        public override Vector2 ScreenPoint
        {
            get
            {
                return ScreenFromWorld(InteractionEndPoint);
            }
        }

        public override Vector2 ViewportPoint
        {
            get
            {
                return ViewportFromWorld(InteractionEndPoint);
            }
        }
    }
}