using System;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Juniper.Unity.Input.Pointers
{
    public interface IPointerDevice
    {
        string name
        {
            get;
        }

        bool isActiveAndEnabled
        {
            get;
        }

        Transform transform
        {
            get;
        }

        bool IsConnected
        {
            get;
        }

        bool IsDisabled
        {
            get;
        }

        bool IsEnabled
        {
            get;
        }

        bool LockedOnTarget
        {
            get; set;
        }

        Type ButtonType
        {
            get;
        }

        PhysicsRaycaster Raycaster
        {
            get;
        }

        Camera EventCamera
        {
            get;
        }

        IEventSystemHandler EventTarget
        {
            get; set;
        }

        AbstractUnifiedInputModule InputModule
        {
            get;
        }

        Vector3 InteractionDirection
        {
            get;
        }

        Vector3 InteractionEndPoint
        {
            get;
        }

        bool IsDragging
        {
            get;
        }

        float MaximumPointerDistance
        {
            get;
        }

        float MinimumPointerDistance
        {
            get;
        }

        int PointerID
        {
            get; set;
        }

        string ProbeName
        {
            get;
        }

        Vector2 ScrollDelta
        {
            get;
        }

        Vector3 WorldPoint
        {
            get;
        }

        Vector2 ViewportPoint
        {
            get;
        }

        Vector2 ScreenPoint
        {
            get;
        }

        Vector2 ScreenDelta
        {
            get;
        }

        Vector2 ViewportFromWorld(Vector3 worldPoint);

        Vector2 ViewportFromScreen(Vector2 screenPoint);

        Vector2 ScreenFromWorld(Vector3 worldPoint);

        Vector2 ScreenFromViewport(Vector2 viewportPoint);

        Vector3 WorldFromScreen(Vector2 screenPoint);

        Vector3 WorldFromViewport(Vector2 viewportPoint);

        bool IsButtonDown(PointerEventData.InputButton button);

        bool IsButtonPressed(PointerEventData.InputButton button);

        bool IsButtonUp(PointerEventData.InputButton button);

        void Process(PointerEventData evtData, float pixelDragThresholdSquared);

        void SetProbe(Probe p);
    }
}
