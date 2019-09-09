using System.Linq;
using Juniper.Progress;
using UnityEngine;

namespace Juniper.Widgets
{
    public class MenuView : AbstractStateController
    {
        private AbstractStateController[] transitions;

        public void Awake()
        {
            transitions = (from trans in GetComponents<AbstractStateController>()
                           where trans != this && trans.enabled
                           select trans)
                        .ToArray();
        }

        public override void SkipEnter()
        {
            base.SkipEnter();

            foreach(var trans in transitions)
            {
                trans.SkipEnter();
            }
        }

        public override void SkipExit()
        {
            base.SkipExit();

            foreach(var trans in transitions)
            {
                trans.SkipExit();
            }
        }

        public override void Enter(IProgress prog = null)
        {
            base.Enter(prog);

            var subProgs = prog.Split(transitions.Length);
            for(int i = 0; i < transitions.Length; ++i)
            {
                transitions[i].Enter(subProgs[i]);
            }
        }

        public override void Exit(IProgress prog = null)
        {
            var subProgs = prog.Split(transitions.Length);
            for (int i = 0; i < transitions.Length; ++i)
            {
                transitions[i].Exit(subProgs[i]);
            }

            base.Exit(prog);
        }

        public void Update()
        {
            var allComplete = true;
            for (int i = 0; i < transitions.Length && allComplete; ++i)
            {
                allComplete = transitions[i].IsComplete;
            }

            if (allComplete)
            {
                Complete();
            }
        }
    }
}
