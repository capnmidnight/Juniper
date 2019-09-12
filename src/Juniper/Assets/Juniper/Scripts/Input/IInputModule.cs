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

        bool ControllersAvailable { get; }
        bool ControllersRequested { get; set; }
        bool ControllersEnabled { get; }

        bool GazeAvailable { get; }
        bool GazeRequested { get; set; }
        bool GazeEnabled { get; }

        bool HandsAvailable { get; }
        bool HandsRequested { get; set; }
        bool HandsEnabled { get; }

        bool MouseAvailable { get; }
        bool MouseRequested { get; set; }
        bool MouseEnabled { get; }

        bool TouchAvailable { get; }
        bool TouchRequested { get; set; }
        bool TouchEnabled { get; }

        bool VoiceAvailable { get; }
        bool VoiceRequested { get; set; }
        bool VoiceEnabled { get; }

        T MakePointer<T>(Transform parent, string path) where T : Component, IPointerDevice;

        void Process();
    }
}
