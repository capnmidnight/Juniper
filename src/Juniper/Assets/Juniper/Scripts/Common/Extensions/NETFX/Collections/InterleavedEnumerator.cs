using System.Collections.Generic;
using System.Linq;

namespace System.Collections
{
    /// <summary>
    /// For N enumerators, creates a single enumerator that iterates over each of them in a single
    /// step until all are empty. This is useful for constructing coroutines that are meant to run in
    /// parallel and we want to know when all of them have finished.
    /// </summary>
    public class InterleavedEnumerator : IEnumerator
    {
        /// <summary>
        /// Creates an interleaved enumerator out of a fixed set of Enumerators.
        /// </summary>
        /// <param name="enumerators">Enums.</param>
        public InterleavedEnumerator(params IEnumerator[] enumerators)
        {
            enums = enumerators;
            stillGoing = new bool[enums.Length];
            for (var i = 0; i < stillGoing.Length; ++i)
            {
                stillGoing[i] = true;
            }
        }

        /// <summary>
        /// Creates an interleaved enumerator out of a generic collection of Enumerators.
        /// </summary>
        /// <param name="enums">Enums.</param>
        public InterleavedEnumerator(IEnumerable<IEnumerator> enums)
            : this(enums.ToArray())
        {
        }

        /// <summary>
        /// Returns an Enumerator that contains the <see cref="IEnumerator.Current"/> value of all
        /// contained Enumerators. Null values and Enumerators that have reached their end are ignored.
        /// </summary>
        /// <value>The current.</value>
        public object Current
        {
            get
            {
                if (current == null)
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    return current;
                }
            }
        }

        /// <summary>
        /// Takes N enumerators and moves through them all, even if some of them are null or finished.
        /// </summary>
        /// <returns><c>True</c> when all enumerators can no longer move to another item.</returns>
        public bool MoveNext()
        {
            current = null;
            if (stillGoing.Contains(true))
            {
                for (var i = 0; i < enums.Length; ++i)
                {
                    if (stillGoing[i])
                    {
                        var iter = enums[i];
                        stillGoing[i] = iter != null && iter.MoveNext();
                        if (!stillGoing[i])
                        {
                            enums[i] = null;
                        }
                    }
                }

                if (stillGoing.Contains(true))
                {
                    var output = new List<object>();

                    for (var i = 0; i < enums.Length; ++i)
                    {
                        var e = enums[i];

                        if (e != null && e.Current != null)
                        {
                            output.Add(e.Current);
                        }
                    }

                    current = output.GetEnumerator();
                }
            }

            return current != null;
        }

        /// <summary>
        /// Reset all of the enumerators to their very beginning.
        /// </summary>
        public void Reset()
        {
            current = null;
            for (var i = 0; i < enums.Length; ++i)
            {
                var iter = enums[i];
                if (iter != null)
                {
                    iter.Reset();
                    stillGoing[i] = true;
                }
            }
        }

        /// <summary>
        /// The enumerators to interleave.
        /// </summary>
        private IEnumerator[] enums;

        /// <summary>
        /// The cached state of the previous return value for each enumerators MoveNext method, so we
        /// know whether or not to keep attempting to read from a particular iterator.
        /// </summary>
        private bool[] stillGoing;

        /// <summary>
        /// The cached "current" enumerator, which gets replaced when <see cref="MoveNext"/> is called.
        /// </summary>
        private IEnumerator current;
    }
}