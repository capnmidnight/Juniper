using System;

namespace Juniper
{
    public abstract class AbstractNamedAction<ActionT>
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