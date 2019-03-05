using UnityEngine;

namespace Juniper.Unity.Input.Pointers.Screen
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
