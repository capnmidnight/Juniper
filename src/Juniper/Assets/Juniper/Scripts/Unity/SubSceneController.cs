using System.Collections.Generic;
using System.Linq;
using Juniper.Input;
using Juniper.Progress;
using Juniper.Animation;

using UnityEngine;

namespace Juniper
{
    /// <summary>
    /// SubSceneControllers are used by the <see cref="MasterSceneController"/> to manage workflow
    /// through an application. They are parts of a greater scene that are loaded additively, with
    /// transitions between them. They are meant for keeping subsections of the application
    /// compartmentalized from each other, to discourage tightly coupling dependencies between
    /// different functional portions of the application.
    /// </summary>
    public class SubSceneController : AbstractStateController
    {
        public virtual void Awake()
        {
            childTransitions = GetComponentsInChildren<AbstractTransitionController>(true);
        }

        public virtual void Load(IProgress prog = null)
        {
            prog?.Report(1);
        }

        [ContextMenu("Enter")]
        public override void Enter()
        {
            isExitingPreviousScene = true;
            foreach (var subScene in CurrentSubScenes)
            {
                subScene.Exit();
            }
            base.Enter();
        }

        /// <summary>
        /// Triggers the exit process and then deactivates the subscene.
        /// </summary>
        /// <returns>The coroutine.</returns>
        [ContextMenu("Exit")]
        public override void Exit()
        {
            if (isActiveAndEnabled)
            {
                foreach (var child in Exitable)
                {
                    child.Exit();
                }
            }
            base.Exit();
        }

        public override void Update()
        {
            if (IsRunning)
            {
                if (state == Direction.Forward && !isExitingPreviousScene && AllComplete(Enterable)
                    || state == Direction.Reverse && AllComplete(Exitable))
                {
                    state = Direction.Stopped;
                }
                else if (state == Direction.Forward && isExitingPreviousScene && AllComplete(CurrentSubScenes))
                {
                    isExitingPreviousScene = false;
                    foreach (var child in Enterable)
                    {
                        child.Enter();
                    }
                }
            }

            base.Update();
        }

        /// <summary>
        /// Whether or not to deactivate the subscene when it has finished exiting.
        /// </summary>
        protected bool deactivateOnExit = true;

        protected override void OnExited()
        {
            base.OnExited();

            if (deactivateOnExit)
            {
                this.Deactivate();
            }
        }

        /// <summary>
        /// All of the StateControllers that are in this gameObject's hierarchy (including itself),
        /// but not this SubSceneController.
        /// </summary>
        /// <value>The child state controllers.</value>
        private AbstractTransitionController[] childTransitions;

        private bool isExitingPreviousScene;

        /// <summary>
        /// <see cref="childTransitions"/> that are not enabled, but exist on active game objects.
        /// </summary>
        /// <value>The enterable.</value>
        private IEnumerable<AbstractTransitionController> Enterable
        {
            get
            {
                return from trans in childTransitions
                       where !trans.enabled
                       select trans;
            }
        }

        /// <summary>
        /// Active and enabled <see cref="childTransitions"/>.
        /// </summary>
        /// <value>The exitable.</value>
        private IEnumerable<AbstractTransitionController> Exitable
        {
            get
            {
                return from trans in childTransitions
                       where trans.enabled
                       select trans;
            }
        }

        /// <summary>
        /// Get all of the sub scenes that have been entered that are not the current sub scene.
        /// </summary>
        /// <value>The current sub scenes.</value>
        private IEnumerable<SubSceneController> CurrentSubScenes
        {
            get
            {
                return from scene in FindObjectsOfType<SubSceneController>()
                       where scene.enabled && scene != this
                       select scene;
            }
        }

        /// <summary>
        /// Returns true when all of the parameter subScenes are <see cref="SubSceneController.IsComplete"/>.
        /// </summary>
        /// <returns>The complete.</returns>
        /// <param name="subScenes">Sub scenes.</param>
        private bool AllComplete(IEnumerable<AbstractStateController> subScenes)
        {
            foreach (var subScene in subScenes)
            {
                if (subScene.IsRunning)
                {
                    return false;
                }
            }
            return true;
        }
    }
}