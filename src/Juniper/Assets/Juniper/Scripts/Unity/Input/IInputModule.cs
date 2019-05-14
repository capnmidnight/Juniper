using Juniper.Input.Pointers;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Juniper.Input
{
    public interface IInputModule
    {
        bool AnyPointerDragging
        {
            get;
        }

        void AddPointer(IPointerDevice pointer);

        PointerEventData Clone(int pointerDataID, PointerEventData original);

        void EnableControllers(bool value, bool savePref);

        void EnableGaze(bool value, bool savePref);

        void EnableHands(bool value, bool savePref);

        void EnableMouse(bool value, bool savePref);

        void EnableTouch(bool value, bool savePref);

        T MakePointer<T>(Transform parent, string path) where T : Component, IPointerDevice;

        void Process();
    }
}
