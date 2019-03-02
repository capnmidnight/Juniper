using System;
using System.Collections.Generic;
using System.Linq;
using Juniper.Input.Pointers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Juniper.Input
{
    /// <summary>
    /// Finds all of the <see cref="IPointerDevice"/> s and fires raycaster events for all
    /// of them.
    /// </summary>
    [DisallowMultipleComponent]
    public class UnifiedInputModule : PointerInputModule, IInstallable
    {
        private readonly List<IPointerDevice> newDevices = new List<IPointerDevice>();
        public readonly List<IPointerDevice> Devices = new List<IPointerDevice>();

        /// <summary>
        /// Set to the clonable object that should be used for the pointer probe.
        /// </summary>
        public GameObject pointerPrefab;

        public PointerFoundEvent onPointerFound;

        public event EventHandler<PointerFoundEventArgs> PointerFound;

        public void AddPointer(IPointerDevice pointer)
        {
            if (!Devices.Contains(pointer) && !newDevices.Contains(pointer))
            {
                pointer.PointerID = Devices.Count * 10000;
                pointer.InputModule = this;
                newDevices.Add(pointer);
            }
        }

        public void WithPointer(Action<IPointerDevice> onPointerAvailable)
        {
            if (Devices.Count > 0)
            {
                onPointerAvailable(Devices[0]);
            }
            else
            {
                EventHandler<PointerFoundEventArgs> subAct = null;
                subAct = new EventHandler<PointerFoundEventArgs>((sender, args) =>
                {
                    PointerFound -= subAct;
                    onPointerAvailable(args.device);
                });
                PointerFound += subAct;
            }
        }

        public class PointerFoundEvent : UnityEvent<IPointerDevice> { }

        protected StageExtensions stage;

#if GOOGLEVR
        public static bool AnyActiveGoogleInstantPreview =>
            ComponentExt.FindAll<Gvr.Internal.InstantPreview>()
                .Any(ComponentExt.IsActivated);
#endif

        protected override void Awake()
        {
            base.Awake();

            Install(false);

            stage = ComponentExt.FindAny<StageExtensions>();

#if GOOGLEVR
            var gci = GetComponent<GvrControllerInput>();
            var gee = GetComponent<GvrEditorEmulator>();
            gci.enabled = gee.enabled = AnyActiveGoogleInstantPreview;
#endif
        }

        public virtual void Reinstall() =>
            Install(true);

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            Reinstall();
        }
#endif

#if UNITY_MODULES_UI
        protected override void Start()
        {
            base.Start();

            raycasters = FindObjectsOfType<GraphicRaycaster>().ToList();
        }

        private List<GraphicRaycaster> raycasters = new List<GraphicRaycaster>();

        public void AddRaycaster(GraphicRaycaster graphicRaycaster) =>
            raycasters.MaybeAdd(graphicRaycaster);
#endif

        /// <summary>
        /// A method for sorting raycasts.
        /// </summary>
        /// <returns>The comparer.</returns>
        /// <param name="lhs">Lhs.</param>
        /// <param name="rhs">Rhs.</param>
        private static int RaycastComparer(RaycastResult lhs, RaycastResult rhs)
        {
            if (lhs.module.eventCamera != null
                && rhs.module.eventCamera != null)
            {
                return lhs.distance.CompareTo(rhs.distance);
            }
            else if (lhs.module.sortOrderPriority != rhs.module.sortOrderPriority)
            {
                return lhs.module.sortOrderPriority.CompareTo(rhs.module.sortOrderPriority);
            }
            else if (lhs.module.renderOrderPriority != rhs.module.renderOrderPriority)
            {
                return lhs.module.renderOrderPriority.CompareTo(rhs.module.renderOrderPriority);
            }
            else if (lhs.depth != rhs.depth)
            {
                return rhs.depth.CompareTo(lhs.depth);
            }
            else if (Mathf.Abs(lhs.distance - rhs.distance) > 0.00001f)
            {
                return lhs.distance.CompareTo(rhs.distance);
            }
            else
            {
                return lhs.index.CompareTo(rhs.index);
            }
        }

        private void OnPointerFound(IPointerDevice pointer)
        {
            PointerFound?.Invoke(this, new PointerFoundEventArgs(pointer));
            onPointerFound?.Invoke(pointer);
        }

        /// <summary>
        /// Find all the pointers and fire raycaster events for them.
        /// </summary>
        public override void Process()
        {
            foreach (var pointer in newDevices)
            {
                Devices.Add(pointer);
                OnPointerFound(pointer);
            }

            newDevices.Clear();

            foreach (var pointer in Devices)
            {
                if (pointer.IsEnabled)
                {
                    PointerEventData evtData;
                    GetPointerData(pointer.PointerID, out evtData, true);
                    evtData.delta = pointer.ScreenDelta;
                    evtData.position = pointer.ScreenPoint;
                    evtData.scrollDelta = pointer.ScrollDelta;
                    evtData.useDragThreshold = eventSystem.pixelDragThreshold > 0;
                    evtData.pointerCurrentRaycast = UpdateRay(pointer, pointer.transform.position, evtData);

                    pointer.Process(evtData, eventSystem.pixelDragThreshold * eventSystem.pixelDragThreshold);
                }
            }
        }

        public PointerEventData Clone(PointerEventData original, int offset)
        {
            PointerEventData clone;
            GetPointerData(original.pointerId + offset, out clone, true);

            clone.delta = original.delta;
            clone.position = original.position;
            clone.scrollDelta = original.scrollDelta;
            clone.pointerEnter = original.pointerEnter;
            clone.useDragThreshold = original.useDragThreshold;
            clone.pointerCurrentRaycast = original.pointerCurrentRaycast;
            if (original.clickCount == -1)
            {
                clone.eligibleForClick = false;
                clone.clickCount = 0;
            }

            return clone;
        }

        private RaycastResult UpdateRay(IPointerDevice pointer, Vector3 lastPointerPosition, PointerEventData evtData)
        {
            var ray = evtData.pointerCurrentRaycast;
            if (pointer.LockedOnTarget)
            {
                var from = ray.worldPosition - lastPointerPosition;
                var to = ray.distance * (pointer.InteractionEndPoint - pointer.transform.position);
                var delta = to - from;
                var rot = Quaternion.FromToRotation(from.normalized, to.normalized);

                ray.gameObject = evtData.pointerPress;
                ray.worldPosition = (rot * ray.worldPosition) + delta;
                ray.worldNormal = rot * ray.worldNormal;
                ray.screenPosition = evtData.position;
            }
            else
            {
                RaycastAll(pointer, evtData);
                if (m_RaycastResultCache.Count > 0)
                {
                    ray = m_RaycastResultCache[0];
                    evtData.hovered.Clear();
                    foreach (var r in m_RaycastResultCache)
                    {
                        evtData.hovered.Add(r.gameObject);
                    }
                }
                else
                {
                    ray.Clear();
                    ray.worldPosition = pointer.InteractionEndPoint;
                    ray.distance = pointer.MinimumPointerDistance;
                    ray.worldNormal = -pointer.InteractionDirection;
                    ray.screenPosition = evtData.position;
                }
            }

            return ray;
        }

        /// <summary>
        /// Fire a raycast using all of the GraphicRaycasters in the system, plus the one
        /// PhysicsRaycaster that is associated with the event Camera.
        /// </summary>
        /// <param name="pointer">Pointer.</param>
        /// <param name="eventData">Event data.</param>
        private void RaycastAll(IPointerDevice pointer, PointerEventData eventData)
        {
            m_RaycastResultCache.Clear();

#if UNITY_MODULES_PHYSICS
            pointer.Raycaster?.Raycast(eventData, m_RaycastResultCache);
#endif

#if UNITY_MODULES_UI
            foreach (var raycaster in raycasters)
            {
                if (raycaster.isActiveAndEnabled)
                {
                    var canv = raycaster.GetComponent<Canvas>();
                    if (pointer.EventCamera.IsInView(canv))
                    {
                        canv.worldCamera = pointer.EventCamera;
                        var start = m_RaycastResultCache.Count;
                        raycaster.Raycast(eventData, m_RaycastResultCache);
                        for (var i = start; i < m_RaycastResultCache.Count; ++i)
                        {
                            var ray = m_RaycastResultCache[i];
                            ray.worldNormal = canv.transform.forward;

                            var pos = (Vector3)ray.screenPosition;
                            pos.z = ray.distance + 0.35f;
                            ray.worldPosition = pointer.EventCamera.ScreenToWorldPoint(pos);

                            m_RaycastResultCache[i] = ray;
                        }
                    }
                }
            }
#endif

            m_RaycastResultCache.Sort(RaycastComparer);
        }

        public void Install(bool reset)
        {
            reset &= Application.isEditor;

#if OCULUS
            this.EnsureComponent<OVRManager>();

#elif GOOGLEVR
            this.EnsureComponent<GvrControllerInput>();
            this.EnsureComponent<GvrEditorEmulator>();

#if UNITY_EDITOR
            var ip = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(
                System.IO.PathExt.FixPath("Assets/GoogleVR/Prefabs/InstantPreview/GvrInstantPreviewMain.prefab"));
            UnityEditor.PrefabUtility.InstantiatePrefab(ip);
#endif
#endif

#if UNITY_EDITOR
            if (pointerPrefab == null)
            {
                pointerPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(System.IO.PathExt.FixPath("Assets/Juniper/Prefabs/DiskProbe.prefab"));
            }
#endif
        }

        public void Uninstall()
        {
#if OCULUS
            this.RemoveComponent<OVRManager>();

#elif GOOGLEVR
            this.RemoveComponent<GvrEditorEmulator>();
            this.RemoveComponent<GvrControllerInput>();
            var ip = ComponentExt.FindAny<InstantPreviewHelper>();
            if (ip != null)
            {
                ip.gameObject.Destroy();
            }
#endif
        }

#if WINDOWSMR || HOLOLENS
        /// <summary>
        /// Manages the input controllers, either motion controllers on WindowsMR headsets, or hand
        /// tracking on HoloLens.
        /// </summary>
        public override void UpdateModule()
        {
            base.UpdateModule();
            Pointers.Motion.MotionController.UpdateReadings();
        }
#endif
    }
}
