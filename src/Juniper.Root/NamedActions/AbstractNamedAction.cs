using System;

namespace Juniper
{
    public interface INamedAction
    {
        string Name { get; }
        Delegate Method { get; }
    }

    public abstract class AbstractNamedAction<ActionT> :
        INamedAction
        where ActionT : Delegate
    {
        public static implicit operator ActionT(AbstractNamedAction<ActionT> namedAction)
        {
            if (namedAction is null)
            {
                return null;
            }

            return namedAction.ToAction();
        }

        public string Name { get; }

        public Delegate Method { get { return Action; } }

        protected ActionT Action { get; }

        protected AbstractNamedAction(string name, ActionT action)
        {
            Name = name;
            Action = action;
        }

        public ActionT ToAction()
        {
            return Action;
        }
    }
}