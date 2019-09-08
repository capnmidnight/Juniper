using System;

using Juniper.Input;
using Juniper.Progress;

using UnityEngine;
using UnityEngine.Events;

namespace Juniper.Animation
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
        public event EventHandler<TransitionValueChangedEventArgs> ValueChanged;

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
        /// The <see cref="progress"/> value after the <see cref="Tweener"/> transformation.
        /// </summary>
        private float tweenedValue;

        /// <summary>
        /// The length of time, in seconds, the transition should take to complete.
        /// </summary>
        public abstract float TransitionLength { get; }

        /// <summary>
        /// The function that performs the 'tweening.
        /// </summary>
        private Func<float, float, Direction, float> Tweener { get { return Tween.Functions[tween]; } }

        public override void Enter(IProgress prog = null)
        {
            base.Enter(prog);
            if (skipEvents)
            {
                Update();
            }
        }

        public override void Exit(IProgress prog = null)
        {
            base.Exit(prog);
            if (skipEvents)
            {
                Update();
            }
        }

        protected override void Complete()
        {
            SetProgress(1);
            base.Complete();
            if (skipEvents)
            {
                Update();
            }
        }

        protected override void OnEntering()
        {
            attack = attackTime;
            release = releaseTime;
            SetProgress(0);
            base.OnEntering();
        }

        protected override void OnExiting()
        {
            attack = releaseTime;
            release = attackTime;
            SetProgress(0);
            base.OnExiting();
        }

        /// <summary>
        /// If the transition is currently in the running state, update its internal value according
        /// to its <see cref="TransitionLength"/>.
        /// </summary>
        protected virtual void Update()
        {
            if (IsRunning)
            {
                if (attack > 0)
                {
                    attack -= Time.deltaTime;
                }
                else if (progress < 1)
                {
                    var change = Time.deltaTime / TransitionLength;
                    SetProgress(Mathf.Clamp01(progress + change));
                }
                else if (release > 0)
                {
                    release -= Time.deltaTime;
                }
                else
                {
                    Complete();
                }
            }
        }

        /// <summary>
        /// Calls the tweening function and propogates the value to the implementing class.
        /// </summary>
        /// <param name="nextProgress">The current progress through the transition.</param>
        private void SetProgress(float nextProgress)
        {
            progress = nextProgress;

            var oldTweenedValue = tweenedValue;
            var value = State == Direction.Forward ? progress : (1 - progress);
            tweenedValue = Tweener(value, tweenK, State);
            OnValueChanged(tweenedValue, oldTweenedValue);
            RenderValue(tweenedValue);
        }

        /// <summary>
        /// Override this method in child classes to implement the animation of the transition.
        /// </summary>
        /// <param name="final">  </param>
        /// <param name="initial"></param>
        private void OnValueChanged(float final, float initial)
        {
            ValueChanged?.Invoke(this, new TransitionValueChangedEventArgs(initial, final));
            onValueChanged?.Invoke();
        }

        /// <summary>
        /// Child implementations of TransitionController must implement this abstract method to
        /// receive the transformed progress value ( <see cref="tweenedValue"/>) and realize it in
        /// the effect of the transition.
        /// </summary>
        /// <param name="value">Value.</param>
        protected abstract void RenderValue(float value);
    }
}