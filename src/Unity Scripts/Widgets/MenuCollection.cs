using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using Juniper.IO;

using UnityEngine;

namespace Juniper.Widgets
{
    public class MenuCollection : SubSceneController
    {
        private readonly Dictionary<string, MenuView> views = new Dictionary<string, MenuView>();

        public string mainMenuView;
        private string nextView;

        public void SetReturnMenuView(string nextView)
        {
            this.nextView = nextView;
        }

        public void ReturnToMainMenu()
        {
            SetReturnMenuView(mainMenuView);
            SwitchToScene(gameObject.scene.name);
        }

        public void ShowMenuView(string name)
        {
            this.Run(ShowMenuViewCoroutine(name));
        }

        private IEnumerator ShowMenuViewCoroutine(string name)
        {
            foreach (var view in views)
            {
                if (view.Key != name && view.Value.CanExit)
                {
                    yield return view.Value.ExitAsync().AsCoroutine();
                    view.Value.Deactivate();
                }
            }

            if (name != null)
            {
                views[name].Activate();
                yield return views[name].EnterAsync().AsCoroutine();
            }
        }

        public override void Awake()
        {
            base.Awake();

            nextView = mainMenuView;

            this.views.Clear();

            var views = GetComponentsInChildren<MenuView>(true);
            foreach (var view in views)
            {
                AddView(view);
            }
        }

        public void Start()
        {
            foreach (var view in views.Values)
            {
                view.SkipExit();
                view.Deactivate();
            }
        }

        private void AddView(MenuView view)
        {
            views.Add(view.name, view);
        }

        public override void Enter(IProgress prog)
        {
            base.Enter(prog);
            prog.Report(1);
            Complete();
        }

        protected override void OnEntered()
        {
            base.OnEntered();
            ShowMenuView(nextView);
        }

        protected override void OnExiting()
        {
            base.OnExiting();
            this.Run(ExitingCouroutine());
        }

        private IEnumerator ExitingCouroutine()
        {
            yield return ShowMenuViewCoroutine(null);
            Complete();
        }

#if UNITY_EDITOR

        public void OnValidate()
        {
            if (string.IsNullOrEmpty(mainMenuView))
            {
                var views = GetComponentsInChildren<MenuView>(true);
                if (views.Length > 0)
                {

                    mainMenuView = views[0].name;
                }
            }
        }

#endif
    }
}
