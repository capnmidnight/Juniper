
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
            Find.Any(out master);
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