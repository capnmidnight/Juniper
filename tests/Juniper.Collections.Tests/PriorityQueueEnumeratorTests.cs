/*
 * FILE: PriorityQueue.cs
 * AUTHOR: Sean T. McBeth
 * DATE: JAN-25-2007
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Collections.Generic.Tests
{
    /// <summary>
    /// Unit tests for PriorityQueueEnumerator class.
    /// </summary>
    [TestClass]
    public class PriorityQueueEnumeratorTests
    {
        /// <summary>
        /// The enumerator should only access items that were in the original queue
        /// </summary>
        [TestMethod]
        public void CurrentGetsItemsThatAreInQueue()
        {
            var pq = MakeBasicPQ();
            IEnumerator en = pq.GetEnumerator();
            while (en.MoveNext())
            {
                Assert.IsTrue(pq.Contains(en.Current), "found an item that wasn't in the queue");
            }
        }

        [TestMethod]
        public void ExtendsGenericIEnumerator()
        {
            var pq = MakeBasicPQ();
            IEnumerator en = pq.GetEnumerator();
            Assert.IsInstanceOfType(en, typeof(IEnumerator));
        }

        [TestMethod]
        public void ExtendsIDisposable()
        {
            var pq = MakeBasicPQ();
            IEnumerator en = pq.GetEnumerator();
            Assert.IsInstanceOfType(en, typeof(IDisposable));
        }

        [TestMethod]
        public void ExtendsIEnumerator()
        {
            var pq = MakeBasicPQ();
            IEnumerator en = pq.GetEnumerator();
            Assert.IsInstanceOfType(en, typeof(IEnumerator<object>));
        }

        /// <summary>
        /// Getting an enumerator on a queue with only 1 item
        /// </summary>
        [TestMethod]
        public void MoveNextOnEmptyQueueReturnsFalse()
        {
            var pq = new PriorityQueue<object>();
            IEnumerator en = pq.GetEnumerator();
            Assert.IsFalse(en.MoveNext(), "MoveNext did not return false");
        }

        /// <summary>
        /// Moving past the end of the enumerator should return false
        /// </summary>
        [TestMethod]
        public void MoveNextReturnsFalsePastEnd()
        {
            var pq = MakeBasicPQ();
            IEnumerator en = pq.GetEnumerator();
            en.MoveNext();
            en.MoveNext();
            en.MoveNext();
            Assert.IsFalse(en.MoveNext(), "MoveNext did not return false");
        }

        /// <summary>
        /// Getting an enumerator on a queue with only 1 item
        /// </summary>
        [TestMethod]
        public void MoveNextReturnsTrue()
        {
            var pq = MakeBasicPQ();
            IEnumerator en = pq.GetEnumerator();
            Assert.IsTrue(en.MoveNext(), "MoveNext did not return true");
        }

        /// <summary>
        /// MoveNext should throw an exception if the underlying connection is changed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MoveNextThrowsExceptionAfterDequeue()
        {
            var pq = MakeBasicPQ();
            IEnumerator en = pq.GetEnumerator();
            en.MoveNext();
            pq.Dequeue();
            en.MoveNext();
        }

        /// <summary>
        /// MoveNext should throw an exception if the underlying connection is changed
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MoveNextThrowsExceptionAfterEnqueue()
        {
            var pq = MakeBasicPQ();
            IEnumerator en = pq.GetEnumerator();
            en.MoveNext();
            pq.Enqueue(new object());
            en.MoveNext();
        }

        /// <summary>
        /// Changing the queue should invalidate the enumeration
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void ResetThrowsExceptionAfterDequeue()
        {
            var pq = MakeBasicPQ();
            IEnumerator en = pq.GetEnumerator();
            en.MoveNext();
            pq.Dequeue();
            en.Reset();
        }

        /// <summary>
        /// Changing the queue should invalidate the enumeration
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void ResetThrowsExceptionAfterEnqueue()
        {
            var pq = MakeBasicPQ();
            IEnumerator en = pq.GetEnumerator();
            en.MoveNext();
            pq.Enqueue(new object());
            en.Reset();
        }

        private PriorityQueue<object> MakeBasicPQ()
        {
            var pq = new PriorityQueue<object>();
            for (var i = 0; i < 3; ++i)
            {
                pq.Enqueue(new object());
            }
            return pq;
        }
    }
}
