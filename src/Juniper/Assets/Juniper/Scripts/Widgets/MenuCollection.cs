using System.Collections;
using System.Collections.Generic;

using Juniper.Progress;
using UnityEngine;

namespace Juniper.Widgets
{
    public class MenuCollection : SubSceneController
    {
        private readonly Dictionary<string, MenuView> views = new Dictionary<string, MenuView>();

        public string firstView;

        public void OnValidate()
        {
            if (string.IsNullOrEmpty(firstView))
            {
                var views = GetComponentsInChildren<MenuView>(true);
                if (views.Length > 0)
                {

                    firstView = views[0].name;
                }
            }
        }

        public override void Awake()
        {
            base.Awake();

            this.views.Clear();

            var views = GetComponentsInChildren<MenuView>(true);
            foreach (var view in views)
            {
                AddView(view);
            }
        }

        public void Start()
        {
            foreach (var view in this.views.Values)
            {
                view.SkipExit();
                view.Deactivate();
            }
        }

        private void AddView(MenuView view)
        {
            views.Add(view.name, view);
        }

        public void ShowView(string name)
        {
            StartCoroutine(ShowViewCoroutine(name));
        }

        private IEnumerator ShowViewCoroutine(string name)
        {
            foreach (var view in views)
            {
                if (view.Key != name && !view.Value.IsExited)
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

        public override void Enter(IProgress prog = null)
        {
            base.Enter(prog);
            prog?.Report(1);
            Complete();
        }

        protected override void OnEntered()
        {
            base.OnEntered();
            ShowView(firstView);
        }

        protected override void OnExiting()
        {
            base.OnExiting();
            StartCoroutine(ExitingCouroutine());
        }

        private IEnumerator ExitingCouroutine()
        {
            yield return ShowViewCoroutine(null);
            Complete();
        }
    }
}
