using System;

using UnityEngine;
using UnityEngine.Events;

namespace Juniper.Unity.Animation
{
    /// <summary>
    /// Child classes of this class are able to animate transitions between program states.
    /// </summary>
    public abstract class AbstractTransitionController : AbstractStateController
    {
        /// <summary>
        /// The amount of time to delay before starting the transition. Remember that the exit is
        /// just the reverse of the enter transition, so the attackTime for the enter transition is
        /// the releaseTime for the exit.
        /// </summary>
        public float attackTime;

        /// <summary>
        /// The amount of time to delay after ending the transition. Remember that the exit is just
        /// the reverse of the enter transition, so the releaseTime for the enter transition is the
        /// attackTime for the exit.
        /// </summary>
        public float releaseTime;

        /// <summary>
        /// Attach event handlers in the Unity Editor to this field to receive notification when the
        /// transition has updated.
        /// </summary>
        public UnityEvent onValueChanged = new UnityEvent();

        /// <summary>
        /// The method to use to pre-compute the transition value.
        /// </summary>
        public TweenType tween;

        /// <summary>
        /// The constant value to provide to the tweening function. Currently, only the Bump tween
        /// function uses this value.
        /// </summary>
        [Tooltip("The constant value to provide to the tweening function. Currently, only the Bump tween function uses this value.")]
        public float tweenK;

        /// <summary>
        /// Attach event handlers at runtime to this field to receive notification when the
        /// transition has updated.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs> ValueChanged;

        /// <summary>
        /// The length of time, in seconds, the transition should take to complete.
        /// </summary>
        public abstract float TransitionLength
        {
            get;
        }

        /// <summary>
        /// Start the transition animation in the forward direction.
        /// </summary>
        [ContextMenu("Enter")]
        public override void Enter()
        {
            StartTransition(Direction.Forward);
            base.Enter();
        }

        /// <summary>
        /// Jump the transition to the ENTERED state, without animating through the ENTERING state.
        /// </summary>
        public override void SkipEnter()
        {
            StartTransition(Direction.Forward);
            base.SkipEnter();
            SetProgress(0);
        }

        /// <summary>
        /// Start the transition animation in the outward direction.
        /// </summary>
        [ContextMenu("Exit")]
        public override void Exit()
        {
            StartTransition(Direction.Reverse);
            base.Exit();
        }

        /// <summary>
        /// If the transition is currently in the running state, update its internal value according
        /// to its <see cref="TransitionLength"/>.
        /// </summary>
        public override void Update()
        {
            if (IsRunning)
            {
                if (attack > 0)
                {
                    attack -= Time.deltaTime;
                }
                else if (progress > 0)
                {
                    var change = Time.deltaTime / TransitionLength;
                    SetProgress(Mathf.Clamp01(progress - change));
                }
                else if (release > 0)
                {
                    release -= Time.deltaTime;
                }
                else
                {
                    SetProgress(0);
                    state = Direction.Stopped;
                }
            }

            base.Update();
        }

        /// <summary>
        /// Changes in the transition value can be captured in other scripts by subscribing to the
        /// ValueChanged event. This EventArgs object includes both the old and the new value.
        /// </summary>
        public class ValueChangedEventArgs : EventArgs
        {
            /// <summary>
            /// The value before the change happens.
            /// </summary>
            public readonly float OldValue;

            /// <summary>
            /// The value after the change happens.
            /// </summary>
            public readonly float NewValue;

            /// <summary>
            /// Creates a new transition value change event.
            /// </summary>
            /// <param name="oldf">Oldf.</param>
            /// <param name="newf">Newf.</param>
            public ValueChangedEventArgs(float oldf, float newf)
            {
                OldValue = oldf;
                NewValue = newf;
            }
        }

        /// <summary>
        /// Child implementations of TransitionController must implement this abstract method to
        /// receive the transformed progress value ( <see cref="tweenedValue"/>) and realize it in
        /// the effect of the transition.
        /// </summary>
        /// <param name="value">Value.</param>
        protected abstract void RenderValue(float value);

        /// <summary>
        /// The function that performs the 'tweening.
        /// </summary>
        private Func<float, float, Direction, float> tweenFunc;

        /// <summary>
        /// The amount of time to delay before starting a transition.
        /// </summary>
        private float attack;

        /// <summary>
        /// The amount of time after the end of a transition to delay the signal that it has finished.
        /// </summary>
        private float release;

        /// <summary>
        /// The current proportion of progress through the transition. If the <see
        /// cref="TransitionLength"/> is 0, the progress is immediately complete. It will be 1 on
        /// completion when ascending and 0 on completion when descending.
        /// </summary>
        private float progress;

        /// <summary>
        /// The <see cref="progress"/> value after the <see cref="tweenFunc"/> transformation.
        /// </summary>
        private float tweenedValue;

        /// <summary>
        /// Override this method in child classes to implement the animation of the transition.
        /// </summary>
        /// <param name="final">  </param>
        /// <param name="initial"></param>
        private void OnValueChanged(float final, float initial)
        {
            ValueChanged?.Invoke(this, new ValueChangedEventArgs(initial, final));
            onValueChanged?.Invoke();
        }

        /// <summary>
        /// Calls the tweening function and propogates the value to the implementing class.
        /// </summary>
        /// <param name="nextProgress">The current progress through the transition.</param>
        private void SetProgress(float nextProgress)
        {
            progress = nextProgress;

            var oldTweenedValue = tweenedValue;
            var value = state == Direction.Reverse ? progress : (1 - progress);
            tweenedValue = tweenFunc(value, tweenK, state);

            OnValueChanged(tweenedValue, oldTweenedValue);
            RenderValue(tweenedValue);
        }

        /// <summary>
        /// Set the transition into the running state.
        /// </summary>
        /// <param name="nextState">
        /// True, if this is an Enter transition. False, if this is an Exit transition.
        /// </param>
        private void StartTransition(Direction nextState)
        {
            state = nextState;
            attack = state == Direction.Forward ? attackTime : releaseTime;
            release = state == Direction.Forward ? releaseTime : attackTime;
            tweenFunc = Tween.Functions[tween];
            SetProgress(1);
        }
    }
}