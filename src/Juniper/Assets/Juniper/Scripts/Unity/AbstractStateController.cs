using System;

using UnityEngine;
using UnityEngine.Events;

namespace Juniper
{
    /// <summary>
    /// A subscene is a game object loaded from another scene. The scenes all get loaded at runtime
    /// and then you can make different parts of it visible on the fly. This procedure deactivates
    /// any subscenes that are not the desired subscene, calling any Exit functions along the way. In
    /// the new scene, TransitionController Enter functions are called as well. It is suitable for
    /// running in a coroutine to track when the end of the switching process occurs.
    /// </summary>
    public abstract class AbstractStateController : MonoBehaviour
    {
        /// <summary>
        /// An event that occurs when the controller is starting to go through the Enter transition.
        /// Prefer <see cref="Entering"/> if you are programmatically adding event handlers at
        /// runtime. If you are adding event handlers in the Unity Editor, prefer <see
        /// cref="onEntering"/>. If you are waiting for this event in a subclass of StateController,
        /// prefer overriding the <see cref="OnEnable"/> method.
        /// </summary>
        public UnityEvent onEntering = new UnityEvent();

        /// <summary>
        /// An event that occurs when the controller has finished going through the Enter transition.
        /// Prefer <see cref="Entered"/> if you are programmatically adding event handlers at
        /// runtime. If you are adding event handlers in the Unity Editor, prefer <see
        /// cref="onEntered"/>. If you are waiting for this event in a subclass of StateController,
        /// prefer overriding the <see cref="OnEntered"/> method.
        /// </summary>
        public UnityEvent onEntered = new UnityEvent();

        /// <summary>
        /// An event that occurs when the controller is starting to go through the Exit transition.
        /// Prefer <see cref="Exiting"/> if you are programmatically adding event handlers at
        /// runtime. If you are adding event handlers in the Unity Editor, prefer <see
        /// cref="onExiting"/>. If you are waiting for this event in a subclass of StateController,
        /// prefer overriding the <see cref="OnExiting"/> method.
        /// </summary>
        public UnityEvent onExiting = new UnityEvent();

        /// <summary>
        /// An event that occurs when the controller has finished going through the Exit transition.
        /// Prefer <see cref="Exited"/> if you are programmatically adding event handlers at runtime.
        /// If you are adding event handlers in the Unity Editor, prefer <see cref="onExited"/>. If
        /// you are waiting for this event in a subclass of StateController, prefer overriding the
        /// <see cref="OnDisable"/> method.
        /// </summary>
        public UnityEvent onExited = new UnityEvent();

        /// <summary>
        /// An event that occurs when the controller is starting to go through the Enter transition.
        /// Prefer <see cref="Entering"/> if you are programmatically adding event handlers at
        /// runtime. If you are adding event handlers in the Unity Editor, prefer <see
        /// cref="onEntering"/>. If you are waiting for this event in a subclass of StateController,
        /// prefer overriding the <see cref="OnEnable"/> method.
        /// </summary>
        public event EventHandler Entering;

        /// <summary>
        /// An event that occurs when the controller has finished going through the Enter transition.
        /// Prefer <see cref="Entered"/> if you are programmatically adding event handlers at
        /// runtime. If you are adding event handlers in the Unity Editor, prefer <see
        /// cref="onEntered"/>. If you are waiting for this event in a subclass of StateController,
        /// prefer overriding the <see cref="OnEntered"/> method.
        /// </summary>
        public event EventHandler Entered;

        /// <summary>
        /// An event that occurs when the controller is starting to go through the Exit transition.
        /// Prefer <see cref="Exiting"/> if you are programmatically adding event handlers at
        /// runtime. If you are adding event handlers in the Unity Editor, prefer <see
        /// cref="onExiting"/>. If you are waiting for this event in a subclass of StateController,
        /// prefer overriding the <see cref="OnExiting"/> method.
        /// </summary>
        public event EventHandler Exiting;

        /// <summary>
        /// An event that occurs when the controller has finished going through the Exit transition.
        /// Prefer <see cref="Exited"/> if you are programmatically adding event handlers at runtime.
        /// If you are adding event handlers in the Unity Editor, prefer <see cref="onExited"/>. If
        /// you are waiting for this event in a subclass of StateController, prefer overriding the
        /// <see cref="OnDisable"/> method.
        /// </summary>
        public event EventHandler Exited;

        public virtual bool IsComplete
        {
            get
            {
                return state == STOPPED;
            }
        }

        public bool IsRunning
        {
            get
            {
                return !IsComplete;
            }
        }

        public bool HasStopped()
        {
            return IsComplete;
        }

        /// <summary>
        /// Fire the OnEnable event and perform the Enter transition.
        /// </summary>
        public virtual void Enter()
        {
            state = ENTERING;
            if (!isActiveAndEnabled)
            {
                enabled = true;
                this.SetTreeActive(true);
            }
            else
            {
                OnEnable();
            }
        }

        public virtual void SkipEnter()
        {
            skipEvent = true;
            Enter();
            Update();
            state = STOPPED;
            Update();
            skipEvent = false;
        }

        /// <summary>
        /// Fire the OnExiting event and perform the Exit transition.
        /// </summary>
        public virtual void Exit()
        {
            state = EXITING;
            OnExiting();
        }

        public virtual void SkipExit()
        {
            skipEvent = true;
            Exit();
            Update();
            state = STOPPED;
            Update();
            skipEvent = false;
        }

        /// <summary>
        /// Called as the object goes from Inactive to Active. Prefer <see cref="Entering"/> if you
        /// are programmatically adding event handlers at runtime. If you are adding event handlers
        /// in the Unity Editor, prefer <see cref="onEntering"/>. If you are waiting for this event
        /// in a subclass of StateController, prefer overriding the <see cref="OnEnable"/> method.
        /// </summary>
        public void OnEnable()
        {
            OnEnabled();
        }

        /// <summary>
        /// Called as the object goes from Active to Inactive. Prefer <see cref="Exited"/> if you are
        /// programmatically adding event handlers at runtime. If you are adding event handlers in
        /// the Unity Editor, prefer <see cref="onExited"/>. If you are waiting for this event in a
        /// subclass of StateController, prefer overriding the <see cref="OnDisable"/> method.
        /// </summary>
        public void OnDisable()
        {
            OnDisabled();
        }

        public virtual void Update()
        {
            if (IsComplete && state != lastState)
            {
                if (lastState == ENTERING)
                {
                    OnEntered();
                }
                else if (lastState == EXITING)
                {
                    enabled = false;
                }
            }

            lastState = state;
        }

        protected const int ENTERING = 1;

        protected const int STOPPED = 0;

        protected const int EXITING = -1;

        private WaitUntil _waiter;

        public WaitUntil Waiter
        {
            get
            {
                if (_waiter == null)
                {
                    _waiter = new WaitUntil(() => IsComplete);
                }
                else
                {
                    _waiter.Reset();
                }
                return _waiter;
            }
        }

        /// <summary>
        /// Whether or not the transition is running in the forward or backward direction (which is
        /// important for Fade(in|out) and Shrink(in|out) transitions).
        /// </summary>
        protected int state;

        protected virtual void OnEnabled()
        {
            if (!skipEvent)
            {
                OnEntering();
            }
        }

        protected virtual void OnDisabled()
        {
            if (!skipEvent)
            {
                OnExited();
            }
        }

        /// <summary>
        /// Called as the object goes from Inactive to Active. Prefer <see cref="Entering"/> if you
        /// are programmatically adding event handlers at runtime. If you are adding event handlers
        /// in the Unity Editor, prefer <see cref="onEntering"/>. If you are waiting for this event
        /// in a subclass of StateController, prefer overriding the <see cref="OnEnable"/> method.
        /// </summary>
        protected virtual void OnEntering()
        {
            onEntering?.Invoke();
            Entering?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Calls the <see cref="onEntered"/> and <see cref="Entered"/> events, if they are valid.
        /// Prefer <see cref="Entered"/> if you are programmatically adding event handlers at
        /// runtime. If you are adding event handlers in the Unity Editor, prefer <see
        /// cref="onEntered"/>. If you are waiting for this event in a subclass of StateController,
        /// prefer overriding the <see cref="OnEntered"/> method.
        /// </summary>
        protected virtual void OnEntered()
        {
            onEntered?.Invoke();
            Entered?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Calls the <see cref="onExiting"/> and <see cref="Exiting"/> events, if they are valid.
        /// Prefer <see cref="Exiting"/> if you are programmatically adding event handlers at
        /// runtime. If you are adding event handlers in the Unity Editor, prefer <see
        /// cref="onExiting"/>. If you are waiting for this event in a subclass of StateController,
        /// prefer overriding the <see cref="OnExiting"/> method.
        /// </summary>
        protected virtual void OnExiting()
        {
            onExiting?.Invoke();
            Exiting?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Calls the <see cref="onExited"/> and <see cref="Exited"/> events, if they are valid.
        /// Prefer <see cref="Exited"/> if you are programmatically adding event handlers at runtime.
        /// If you are adding event handlers in the Unity Editor, prefer <see cref="onExited"/>. If
        /// you are waiting for this event in a subclass of StateController, prefer overriding the
        /// <see cref="OnExited"/> method.
        /// </summary>
        protected virtual void OnExited()
        {
            onExited?.Invoke();
            Exited?.Invoke(this, EventArgs.Empty);
        }

        private int lastState;
        private bool skipEvent;
    }
}
