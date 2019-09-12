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

        bool ControllersEnabled { get; set; }
        bool GazeEnabled { get; set; }
        bool HandsEnabled { get; set; }
        bool MouseEnabled { get; set; }
        bool TouchEnabled { get; set; }
        bool VoiceEnabled { get; set; }

        T MakePointer<T>(Transform parent, string path) where T : Component, IPointerDevice;

        void Process();
    }
}
