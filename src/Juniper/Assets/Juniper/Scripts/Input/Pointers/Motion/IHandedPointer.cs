using Juniper.Input.Pointers;

namespace Juniper.Input.Pointers.Motion
{
    public interface IHandedPointer : IPointerDevice
    {
        Hand Hand
        {
            get;
        }

        bool IsDominantHand
        {
            get;
        }

        bool IsLeftHand
        {
            get;
        }

        bool IsNonDominantHand
        {
            get;
        }

        bool IsRightHand
        {
            get;
        }
    }
}
