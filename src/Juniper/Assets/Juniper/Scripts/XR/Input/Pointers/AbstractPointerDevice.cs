using System;

using Juniper.Audio;
using Juniper.Display;
using Juniper.Haptics;
using Juniper.Statistics;

using UnityEngine;
using UnityEngine.EventSystems;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Input.Pointers
{
    public abstract class AbstractPointerDevice<ButtonIDType, ConfigType> :
        MonoBehaviour,
        IInstallable,
        IPointerDevice,
        IPointerButtons<ButtonIDType>
        where ButtonIDType : struct
        where ConfigType : AbstractPointerConfiguration<ButtonIDType>, new()
    {
        protected static readonly Vector2 VIEWPORT_MIDPOINT =
            0.5f * Vector2.one;

        protected static readonly ConfigType PointerConfig =
            new ConfigType();

        public Type ButtonType
        {
            get
            {
                return typeof(ButtonIDType);
            }
        }

        protected readonly ButtonMapper<ButtonIDType> nativeButtons = new ButtonMapper<ButtonIDType>();

        [ReadOnly]
        public bool Connected;

        public int PointerDataID
        {
            get
            {
                return GetInstanceID();
            }
        }

        /// <summary>
        /// The minimum distance from the camera at which to place the pointer.
        /// </summary>
        public float MinimumPointerDistance
        {
            get
            {
                return Mathf.Max(eventManager.minPointerDistance, 1.1f * DisplayManager.MainCamera.nearClipPlane);
            }
        }

        protected Vector3 pointerOffset;

        public float MaximumPointerDistance
        {
            get
            {
                return Mathf.Min(eventManager.maxPointerDistance, 0.9f * DisplayManager.MainCamera.farClipPlane);
            }
        }

        public Material LaserPointerMaterial;

        public int Layer
        {
            get
            {
                return gameObject.layer;
            }
            set
            {
                if (value != gameObject.layer)
                {
                    foreach (var trans in transform.FamilyTree())
                    {
                        trans.gameObject.layer = value;
                    }
                }
            }
        }

        public bool LockedOnTarget
        {
            get; set;
        }

        public IEventSystemHandler EventTarget
        {
            get; set;
        }

        public abstract bool IsConnected
        {
            get;
        }

        public bool IsEnabled
        {
            get
            {
                return isActiveAndEnabled && IsConnected;
            }
        }

        /// <summary>
        /// Returns true when the device is supposed to be disabled.
        /// </summary>
        /// <value><c>true</c> if is disabled; otherwise, <c>false</c>.</value>
        public bool IsDisabled
        {
            get
            {
                return !IsEnabled;
            }
        }

        public PhysicsRaycaster Raycaster
        {
            get
            {
                return probe?.Raycaster;
            }
        }

        public virtual bool AnyButtonPressed
        {
            get
            {
                return IsButtonPressed(InputButton.Left)
                    || IsButtonPressed(InputButton.Right)
                    || IsButtonPressed(InputButton.Middle);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Juniper.Input.PointerDevice"/> class.
        /// </summary>
        public virtual void Awake()
        {
            Install(false);

            eventManager = ComponentExt.FindAny<UnifiedInputModule>();
            stage = ComponentExt.FindAny<Avatar>();

            pointerOffset = MinimumPointerDistance * Vector3.forward;

            Haptics = MakeHapticsDevice();

            nativeButtons.ButtonDownNeeded += IsButtonDown;
            nativeButtons.ButtonUpNeeded += IsButtonUp;
            nativeButtons.ButtonPressedNeeded += IsButtonPressed;
            nativeButtons.ClonedPointerEventNeeded += Clone;
            nativeButtons.InteractionNeeded += PlayInteraction;
        }

        public void Start()
        {
            eventManager.AddPointer(this);
        }

        public virtual void Install(bool reset)
        {
            PointerConfig.Install(nativeButtons, gameObject, reset);
            OnProbeFound();
        }

        public virtual void Uninstall()
        {
            PointerConfig.Uninstall(gameObject);
        }

        public virtual void Reinstall()
        {
            Install(true);
        }

#if UNITY_EDITOR

        public void Reset()
        {
            Reinstall();
        }

#endif

        public bool showProbe = true;

        /// <summary>
        /// The cursor probe that shows the physical location of the current selection.
        /// </summary>
        [SerializeField]
        public IProbe probe;

        public virtual void OnProbeFound()
        {
            if (probe == null)
            {
                probe = Probe.Ensure(transform, ProbeName);
            }
        }

        public string ProbeName
        {
            get
            {
                return name + "-probe";
            }
        }

        public Vector3 Origin
        {
            get
            {
                return transform.position;
            }
        }

        public Vector3 LastOrigin
        {
            get;
            private set;
        }

        public Vector3 OriginDelta
        {
            get
            {
                return Origin - LastOrigin;
            }
        }

        public abstract Vector3 WorldPoint
        {
            get;
        }

        public Vector3 LastWorldPoint
        {
            get; private set;
        }

        public Vector3 WorldDelta
        {
            get
            {
                return WorldPoint - LastWorldPoint;
            }
        }

        /// <summary>
        /// The target at and through which the pointer rays fire.
        /// </summary>
        /// <value>The interaction end point.</value>
        public Vector3 SmoothedWorldPoint
        {
            get
            {
                return motionFilter?.PredictedPosition ?? WorldPoint;
            }
        }

        public Vector3 LastSmoothedWorldPoint
        {
            get;
            private set;
        }

        public Vector3 SmoothedWorldPointDelta
        {
            get
            {
                return SmoothedWorldPoint - LastSmoothedWorldPoint;
            }
        }

        /// <summary>
        /// The direction the pointer is pointing, from <see cref="InteractionOrigin"/> to <see cref="SmoothedWorldPoint"/>.
        /// </summary>
        /// <value>The interaction direction.</value>
        public Vector3 Direction
        {
            get
            {
                return (SmoothedWorldPoint - Origin).normalized;
            }
        }

        public Vector3 LastDirection
        {
            get
            {
                return (LastSmoothedWorldPoint - LastOrigin).normalized;
            }
        }

        public Vector2 ScreenPoint
        {
            get
            {
                return ScreenFromWorld(SmoothedWorldPoint);
            }
        }

        public Vector2 LastScreenPoint
        {
            get
            {
                return ScreenFromWorld(LastSmoothedWorldPoint);
            }
        }

        public Vector2 ScreenDelta
        {
            get
            {
                return ScreenPoint - LastScreenPoint;
            }
        }

        public Vector2 ViewportPoint
        {
            get
            {
                return ViewportFromWorld(SmoothedWorldPoint);
            }
        }

        public Vector2 LastViewportPoint
        {
            get
            {
                return ViewportFromWorld(LastSmoothedWorldPoint);
            }
        }

        public Vector2 ViewportDelta
        {
            get
            {
                return ViewportPoint - LastViewportPoint;
            }
        }

        /// <summary>
        /// Mouse wheel and touch-pad scroll. This is a 2-dimensional value, as even with a
        /// single-wheel scroll mouse, you can hold the SHIFT key to scroll in the horizontal direction.
        /// </summary>
        public Vector2 ScrollDelta
        {
            get; protected set;
        }

        public Vector2 ScreenFromWorld(Vector3 worldPoint)
        {
            DisplayManager.MoveEventCameraToPointer(this);
            return DisplayManager.EventCamera.WorldToScreenPoint(worldPoint);
        }

        public Vector2 ScreenFromViewport(Vector2 viewportPoint)
        {
            DisplayManager.MoveEventCameraToPointer(this);
            return DisplayManager.EventCamera.ViewportToScreenPoint(viewportPoint);
        }

        public Vector2 ViewportFromWorld(Vector3 worldPoint)
        {
            DisplayManager.MoveEventCameraToPointer(this);
            return DisplayManager.EventCamera.WorldToViewportPoint(worldPoint);
        }

        public Vector2 ViewportFromScreen(Vector2 screenPoint)
        {
            DisplayManager.MoveEventCameraToPointer(this);
            return DisplayManager.EventCamera.ScreenToViewportPoint(screenPoint);
        }

        public Vector3 WorldFromScreen(Vector2 screenPoint)
        {
            DisplayManager.MoveEventCameraToPointer(this);
            return DisplayManager.EventCamera.ScreenToWorldPoint((Vector3)screenPoint + pointerOffset);
        }

        public Vector3 WorldFromViewport(Vector2 viewportPoint)
        {
            DisplayManager.MoveEventCameraToPointer(this);
            return DisplayManager.EventCamera.ViewportToWorldPoint((Vector3)viewportPoint + pointerOffset);
        }

#if UNITY_EDITOR
        private AbstractMotionFilter parentMotionFilter;
#endif

        private AbstractMotionFilter lastMotionFilter;
        public AbstractMotionFilter motionFilter;

        /// <summary>
        /// Update the position of the pointer and the pointer probe. Also check to see if the
        /// configuration has been changed to hide the pointer probe.
        /// </summary>
        public virtual void Update()
        {
            if (motionFilter != lastMotionFilter)
            {
#if UNITY_EDITOR
                parentMotionFilter = motionFilter;
#endif
                motionFilter = Instantiate(motionFilter);
                lastMotionFilter = motionFilter;
            }

#if UNITY_EDITOR
            motionFilter?.Copy(parentMotionFilter);
#endif

            Connected = IsConnected;

            if (probe != null)
            {
                probe.SetActive(IsEnabled && showProbe);
                probe.LaserPointerMaterial = LaserPointerMaterial;
            }

            if (IsEnabled)
            {
                motionFilter?.UpdateState(WorldPoint);
                InternalUpdate();
                probe?.AlignProbe(Direction, transform.up);
            }
        }

        protected AbstractUnifiedInputModule eventManager;

        public IInputModule InputModule
        {
            get
            {
                return eventManager;
            }
        }

        protected Avatar stage;

        public virtual bool IsDragging
        {
            get
            {
                return EventTarget != null && nativeButtons.IsDragging;
            }
        }

        /// <summary>
        /// The haptic feedback system associated with the device. For touch pointers, this is the
        /// global haptic system. For motion controllers, each controller has its own haptic system.
        /// </summary>
        /// <value>The haptics.</value>
        public AbstractHapticDevice Haptics
        {
            get; protected set;
        }

        private float finishTime;
        private Interaction lastAction;

        public virtual void PlayInteraction(Interaction action)
        {
            if (Time.unscaledTime > finishTime || action != lastAction)
            {
                lastAction = action;
                finishTime = Time.unscaledTime + InteractionAudio.Play(action, probe.Cursor, Haptics);
            }
        }

        protected PointerEventData Clone(int pointerDataID, PointerEventData evtData)
        {
            return InputModule?.Clone(pointerDataID, evtData);
        }

        public virtual void Process(PointerEventData evtData, float pixelDragThresholdSquared)
        {
            if (!IsDragging)
            {
                TestEnterExit(evtData);
            }

            EventTarget = ProcessButtons(evtData, pixelDragThresholdSquared);

            if (evtData.clickCount == -1)
            {
                evtData.clickCount = 0;
            }

            LastWorldPoint = evtData.pointerCurrentRaycast.worldPosition;
            LastSmoothedWorldPoint = SmoothedWorldPoint;
            LastOrigin = Origin;

            probe?.SetCursor(
                evtData.pointerCurrentRaycast.gameObject != null,
                AnyButtonPressed,
                LastWorldPoint,
                evtData.pointerCurrentRaycast.worldNormal);
        }

        protected virtual IEventSystemHandler ProcessButtons(PointerEventData evtData, float pixelDragThresholdSquared)
        {
            return nativeButtons.Process(evtData, pixelDragThresholdSquared);
        }

        private void TestEnterExit(PointerEventData evtData)
        {
            var target = ExecuteEvents.GetEventHandler<IPointerEnterHandler>(evtData.pointerCurrentRaycast.gameObject);
            if (target != evtData.pointerEnter)
            {
                evtData.clickCount = -1;
                if (evtData.pointerEnter != null)
                {
                    ExecuteEvents.ExecuteHierarchy(evtData.pointerEnter, evtData, ExecuteEvents.pointerExitHandler);
                    PlayInteraction(Interaction.Exited);
                    evtData.pointerEnter = null;
                }

                if (target != null)
                {
                    evtData.pointerEnter = ExecuteEvents.ExecuteHierarchy(target, evtData, ExecuteEvents.pointerEnterHandler);
                    PlayInteraction(Interaction.Entered);
                }
            }
        }

        public virtual bool IsButtonPressed(InputButton button)
        {
            return nativeButtons.IsButtonPressed(button);
        }

        public virtual bool IsButtonUp(InputButton button)
        {
            return nativeButtons.IsButtonUp(button);
        }

        public virtual bool IsButtonDown(InputButton button)
        {
            return nativeButtons.IsButtonDown(button);
        }

        protected virtual void InternalUpdate()
        {
        }

        public abstract bool IsButtonPressed(ButtonIDType button);

        public abstract bool IsButtonDown(ButtonIDType button);

        public abstract bool IsButtonUp(ButtonIDType button);

        protected abstract AbstractHapticDevice MakeHapticsDevice();
    }
}
