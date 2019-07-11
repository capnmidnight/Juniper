using System.Threading.Tasks;
using UnityEngine;

namespace Juniper.Unity.Coroutines
{
    /// <summary>
    /// A Unity coroutine waiter that can handle .NET asynchronous tasks.
    /// </summary>
    public class WaitForTask : CustomYieldInstruction
    {
        private readonly Task waiting;

        public WaitForTask(Task task)
        {
            waiting = task;
        }

        public override bool keepWaiting
        {
            get
            {
                if (waiting.IsFaulted)
                {
                    throw waiting.Exception;
                }

                return !waiting.IsCompleted && !waiting.IsCanceled;
            }
        }
    }
}
