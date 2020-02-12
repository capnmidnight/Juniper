using UnityEngine;
using UnityEngine.EventSystems;

namespace Juniper.Input.Pointers
{
    public interface IProbe
    {
        Transform Cursor
        {
            get;
        }

        PhysicsRaycaster Raycaster
        {
            get;
        }

        Vector2 TouchPoint
        {
            get;
            set;
        }

        bool CanGaze
        {
            get;
            set;
        }

        Material LaserPointerMaterial { get; set; }

        void AlignProbe(Vector3 dir, Vector3 up);

        void SetCursor(bool targeted, bool pressed, Vector3 targetPosition, Vector3 targetForward);

        void SetGaze(float gazeProgress);

        void Destroy();

        void SetActive(bool active);

        XR.Pose ToJuniperPose();
    }
}
