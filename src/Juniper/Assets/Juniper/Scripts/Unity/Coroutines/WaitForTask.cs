using System;
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
        public int testCount;
        private DateTime start;
        private DateTime end;

        public WaitForTask(Task task)
        {
            waiting = task;
            start = DateTime.Now;
        }

        public TimeSpan Elapsed
        {
            get
            {
                return end - start;
            }
        }

        public override bool keepWaiting
        {
            get
            {
                ++testCount;

                if (waiting.IsFaulted)
                {
                    throw waiting.Exception;
                }

                end = DateTime.Now;
                return !waiting.IsCompleted && !waiting.IsCanceled;
            }
        }
    }
}