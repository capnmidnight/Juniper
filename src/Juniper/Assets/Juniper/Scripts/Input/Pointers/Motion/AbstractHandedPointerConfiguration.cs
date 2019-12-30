using System;

using Juniper.Input;

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

        public string MakePointerName(Hand hand)
        {
            return $"{hand}{PointerNameStub}";
        }

        protected abstract string PointerNameStub
        {
            get;
        }

        public abstract HandIDType? this[Hand hand] { get; }
    }
}
