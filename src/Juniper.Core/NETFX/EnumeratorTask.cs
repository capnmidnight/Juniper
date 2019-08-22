using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace System
{
    public class EnumeratorTask<T> : Task<T>
    {
        private static Func<T> CreateFromEnumerator(IEnumerator<T> iter)
        {
            return () =>
            {
                while (iter.MoveNext())
                {
                    // do nothing
                }

                return iter.Current;
            };
        }

        public EnumeratorTask(IEnumerator<T> iter) : base(CreateFromEnumerator(iter))
        {
        }

        public EnumeratorTask(IEnumerator<T> iter, CancellationToken cancellationToken) : base(CreateFromEnumerator(iter), cancellationToken)
        {
        }

        public EnumeratorTask(IEnumerator<T> iter, TaskCreationOptions creationOptions) : base(CreateFromEnumerator(iter), creationOptions)
        {
        }

        public EnumeratorTask(IEnumerator<T> iter, TaskCreationOptions creationOptions, CancellationToken cancellationToken) : base(CreateFromEnumerator(iter), cancellationToken, creationOptions)
        {
        }
    }
}