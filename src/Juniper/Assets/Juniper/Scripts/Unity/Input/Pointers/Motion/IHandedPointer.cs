using Juniper.Input;

namespace Juniper.Unity.Input.Pointers.Motion
{
    public interface IHandedPointer : IPointerDevice
    {
        Hands Hand
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
