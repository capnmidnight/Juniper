using System.Collections.Generic;

using Juniper.Animation;
using Juniper.Progress;

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
        public bool unloadSceneOnExit = true;
        private MasterSceneController master;

        public virtual void Awake()
        {
            childTransitions = GetComponentsInChildren<AbstractTransitionController>(true);
            master = ComponentExt.FindAny<MasterSceneController>();
        }

        public override bool IsComplete
        {
            get
            {
                var childrenComplete = true;
                if (childTransitions != null)
                {
                    foreach (var child in childTransitions)
                    {
                        if (!child.IsComplete)
                        {
                            childrenComplete = false;
                            break;
                        }
                    }
                }
                return base.IsComplete && childrenComplete;
            }
        }

        /// <summary>
        /// Triggers the exit process and then deactivates the subscene.
        /// </summary>
        /// <returns>The coroutine.</returns>
        [ContextMenu("Exit")]
        public override void Exit(IProgress prog = null)
        {
            if (isActiveAndEnabled)
            {
                prog.ForEach(
                    Exitable,
                    (child, p) => child.Exit(p));
            }

            base.Exit(prog);
        }

        /// <summary>
        /// All of the StateControllers that are in this gameObject's hierarchy (including itself),
        /// but not this SubSceneController.
        /// </summary>
        /// <value>The child state controllers.</value>
        private AbstractTransitionController[] childTransitions;

        /// <summary>
        /// <see cref="childTransitions"/> that are not enabled, but exist on active game objects.
        /// </summary>
        /// <value>The enterable.</value>
        private IEnumerable<AbstractTransitionController> Enterable
        {
            get
            {
                if (childTransitions != null)
                {
                    foreach (var trans in childTransitions)
                    {
                        if (!trans.enabled)
                        {
                            yield return trans;
                        }
                    }
                }
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
                if (childTransitions != null)
                {
                    foreach (var trans in childTransitions)
                    {
                        if (trans.enabled)
                        {
                            yield return trans;
                        }
                    }
                }
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

        public void SwitchToScene(string sceneName)
        {
            master.SwitchToScene(sceneName);
        }

        public void ShowScene(string sceneName)
        {
            master.ShowScene(sceneName);
        }

        public void Quit()
        {
            master.Quit();
        }
    }
}