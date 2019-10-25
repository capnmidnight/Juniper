using System.Collections;
using System.Collections.Generic;

using Juniper.Progress;
using UnityEngine;

namespace Juniper.Widgets
{
    public class MenuCollection : SubSceneController
    {
        public const string START_GAME_KEY = "Juniper.Game.Start";

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

        public void ShowMenuView(string name)
        {
            this.Run(ShowMenuViewCoroutine(name));
        }

        public void SetStartValues(string json)
        {
            PlayerPrefs.SetString(START_GAME_KEY, json);
        }

        private IEnumerator ShowMenuViewCoroutine(string name)
        {
            foreach (var view in views)
            {
                if (view.Key != name && view.Value.CanExit)
                {
                    yield return view.Value.ExitCoroutine();
                    view.Value.Deactivate();
                }
            }

            if (name != null)
            {
                views[name].Activate();
                yield return views[name].EnterCoroutine();
            }
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
    }
}
