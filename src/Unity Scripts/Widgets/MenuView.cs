using System.Linq;

using Juniper.IO;

namespace Juniper.Widgets
{
    public class MenuView : AbstractStateController
    {
        private AbstractStateController[] transitions;
        private int waitingCount;

        public void Awake()
        {
            transitions = (from trans in GetComponents<AbstractStateController>()
                           where trans != this && trans.enabled
                           select trans)
                        .ToArray();

            foreach(var trans in transitions)
            {
                trans.Entered += Trans_EnteredExited;
                trans.Exited += Trans_EnteredExited;
            }
        }

        private void Trans_EnteredExited(object sender, System.EventArgs e)
        {
            --waitingCount;
            if(waitingCount == 0)
            {
                Complete();
            }
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

        public override void Enter(IProgress prog)
        {
            base.Enter(prog);
            waitingCount = transitions.Length;
            var subProgs = prog.Split(transitions.Length);
            for(int i = 0; i < transitions.Length; ++i)
            {
                transitions[i].Enter(subProgs[i]);
            }
        }

        public override void Exit(IProgress prog)
        {
            var subProgs = prog.Split(transitions.Length);
            waitingCount = transitions.Length;
            for (int i = 0; i < transitions.Length; ++i)
            {
                transitions[i].Exit(subProgs[i]);
            }

            base.Exit(prog);
        }
    }
}
