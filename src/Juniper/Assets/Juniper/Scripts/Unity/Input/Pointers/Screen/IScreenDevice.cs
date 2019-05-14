using Juniper.Input.Pointers;

using UnityEngine;

namespace Juniper.Input.Pointers.Screen
{
    public interface IScreenDevice : IPointerDevice
    {
        Vector3 CameraPositionOffset
        {
            get;
        }

        Quaternion CameraRotationOffset
        {
            get;
        }
    }
}
