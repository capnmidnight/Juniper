using Juniper.Unity.Input.Pointers;

using System;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Juniper.Unity.Input
{
    public interface IInputModule
    {
        bool AnyPointerDragging
        {
            get;
        }

        event EventHandler<PointerFoundEventArgs> PointerFound;

        void AddPointer(IPointerDevice pointer);
        PointerEventData Clone(int pointerDataID, PointerEventData original);
        void EnableControllers(bool value);
        void EnableGaze(bool value);
        void EnableHands(bool value);
        void EnableMouse(bool value);
        void EnableTouch(bool value);
        T MakePointer<T>(Transform parent, string path) where T : Component, IPointerDevice;
        void Process();
        void WithPointer(Action<IPointerDevice> onPointerAvailable);
    }
}