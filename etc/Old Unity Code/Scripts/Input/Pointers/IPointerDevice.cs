using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Juniper.Input.Pointers
{
    public interface ILaserPointer
    {
        Material LaserPointerNormalMaterial
        {
            get;
            set;
        }

        Material LaserPointerEnabledMaterial
        {
            get;
            set;
        }

        Material LaserPointerDisabledMaterial
        {
            get;
            set;
        }
    }

    public interface IPointerDevice : ILaserPointer
    {
        Transform transform { get; }

        IEventSystemHandler EventTarget
        {
            get; set;
        }

        UnifiedInputModule InputModule
        {
            get;
        }

        /// <summary>
        /// Unique pointer identifiers keep the pointer events cached in Unity's Event System.
        /// </summary>
        /// <value>The pointer identifier.</value>
        int PointerDataID
        {
            get;
        }

        string ProbeName
        {
            get;
        }

        void OnProbeFound();

        IProbe Probe { get; }

        Type ButtonType
        {
            get;
        }

        bool isActiveAndEnabled
        {
            get;
        }

        bool IsConnected
        {
            get;
        }

        bool ProcessInUpdate
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

        bool IsDragging
        {
            get;
        }

        PhysicsRaycaster Raycaster
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

        Vector3 Origin
        {
            get;
        }

        Vector3 LastOrigin
        {
            get;
        }

        Vector3 OriginDelta
        {
            get;
        }

        Vector3 WorldPoint
        {
            get;
        }

        Vector3 LastWorldPoint
        {
            get;
        }

        Vector3 WorldDelta
        {
            get;
        }

        Vector3 SmoothedWorldPoint
        {
            get;
        }

        Vector3 LastSmoothedWorldPoint
        {
            get;
        }

        Vector3 SmoothedWorldPointDelta
        {
            get;
        }

        Vector3 Direction
        {
            get;
        }

        Vector3 LastDirection
        {
            get;
        }

        Vector3 CursorPosition
        {
            get;
        }

        Vector2 ViewportPoint
        {
            get;
        }

        Vector2 LastViewportPoint
        {
            get;
        }

        Vector2 ViewportDelta
        {
            get;
        }

        Vector2 ScreenPoint
        {
            get;
        }

        Vector2 LastScreenPoint
        {
            get;
        }

        Vector2 ScreenDelta
        {
            get;
        }

        Vector2 ScrollDelta
        {
            get;
        }

        int Layer
        {
            get;
            set;
        }

        Vector2 ViewportFromWorld(Vector3 worldPoint);

        Vector2 ViewportFromScreen(Vector2 screenPoint);

        Vector2 ScreenFromWorld(Vector3 worldPoint);

        Vector2 ScreenFromViewport(Vector2 viewportPoint);

        Vector3 WorldFromScreen(Vector2 screenPoint);

        Vector3 WorldFromViewport(Vector2 viewportPoint);

        bool IsButtonDown(KeyCode button);

        bool IsButtonPressed(KeyCode button);

        bool IsButtonUp(KeyCode button);

        bool IsButtonDown(VirtualTouchPadButtons button);

        bool IsButtonPressed(VirtualTouchPadButtons button);

        bool IsButtonUp(VirtualTouchPadButtons button);

        bool IsButtonDown(VirtualTriggerButton button);

        bool IsButtonPressed(VirtualTriggerButton button);

        bool IsButtonUp(VirtualTriggerButton button);

        void Process(JuniperPointerEventData evtData, float pixelDragThresholdSquared, List<KeyCode> keyPresses, bool paused);
    }
}
