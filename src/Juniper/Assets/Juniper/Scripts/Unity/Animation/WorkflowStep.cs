using System;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

#if !UNITY_MODULES_ANIMATION
using Animator = UnityEngine.MonoBehaviour;
#endif

namespace Juniper.Unity.Animation
{
    /// <summary>
    /// A series of workflow steps can be used to show different UI elements and have other
    /// transitions and logic applied as they are flipped through in sequence.
    /// </summary>
    public class WorkflowStep
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Juniper.Animation.WorkflowStep"/> class.
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="animationClip">Animation clip.</param>
        public WorkflowStep(Transform view, string animationClip)
            : this(view, animationClip, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Juniper.Animation.WorkflowStep"/> class.
        /// </summary>
        /// <param name="view">View.</param>
        public WorkflowStep(Transform view)
            : this(view, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Juniper.Animation.WorkflowStep"/> class.
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="action">Action.</param>
        public WorkflowStep(Transform view, Action action)
            : this(view, null, action)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Juniper.Animation.WorkflowStep"/> class.
        /// </summary>
        /// <param name="action">Action.</param>
        public WorkflowStep(Action action)
            : this(null, null, action)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Juniper.Animation.WorkflowStep"/> class.
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="animationClip">Animation clip.</param>
        /// <param name="action">Action.</param>
        public WorkflowStep(Transform view, string animationClip, Action action)
        {
            step = view;
#if UNITY_ANIMATOR
            animationClipName = animationClip;
#endif
            act = action;

            if (step != null)
            {
                buttons = step.GetComponentsInChildren<Button>(true).ToArray();
                foreach (var btn in buttons)
                {
                    btn.onClick.AddListener(DisableButtons);
                }
            }
        }

        /// <summary>
        /// Get the StateController from <see cref="step"/>, if one exists.
        /// </summary>
        /// <value>The transition.</value>
        public AbstractTransitionController Transition
        {
            get
            {
                return step?.GetComponent<AbstractTransitionController>();
            }
        }

        /// <summary>
        /// Enter the step, playing any optionals animations on the provided model and finally
        /// executing the optional logic.
        /// </summary>
        /// <returns>The coroutine.</returns>
        /// <param name="model">Model.</param>
        public void Show(Animator model)
        {
            EnableButtons();

            var trans = Transition;

            trans?.Enter();
            if (trans == null)
            {
                step?.Activate();
            }

#if UNITY_ANIMATOR
            if (animationClipName != null)
            {
                model.Play(animationClipName);
            }
#endif

            act?.Invoke();
        }

        /// <summary>
        /// The (optional) UI element to display when the step is active.
        /// </summary>
        private readonly Transform step;

        /// <summary>
        /// Any clicky buttons that are included in <see cref="step"/>.
        /// </summary>
        private readonly Button[] buttons;

#if UNITY_ANIMATOR
        /// <summary>
        /// The (optional) animation to play on a 3D model when the step is entered.
        /// </summary>
        private readonly string animationClipName;
#endif

        /// <summary>
        /// The (optional) ancillary logic to execute when the step is entered.
        /// </summary>
        private readonly Action act;

        /// <summary>
        /// Sets all of the contained buttons to either enabled or disabled.
        /// </summary>
        /// <param name="enabled">If set to <c>true</c> enabled.</param>
        private void SetButtonsEnabled(bool enabled)
        {
            if (buttons != null)
            {
                foreach (var btn in buttons)
                {
                    btn.interactable = enabled;
                }
            }
        }

        /// <summary>
        /// Sets all of the contained buttons to disabled.
        /// </summary>
        private void DisableButtons()
        {
            SetButtonsEnabled(false);
        }

        /// <summary>
        /// Sets all of the contained buttons to enabled.
        /// </summary>
        private void EnableButtons()
        {
            SetButtonsEnabled(true);
        }
    }
}
