/*
 * FILE: PriorityQueue.cs
 * AUTHOR: Sean T. McBeth
 * DATE: JAN-25-2007
 */

using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

namespace Juniper.Collections.Tests
{
    /// <summary>
    /// Unit tests for PriorityQueueEnumerator class.
    /// </summary>
    [TestFixture]
    public class PriorityQueueEnumeratorTests
    {
        /// <summary>
        /// The enumerator should only access items that were in the original queue
        /// </summary>
        [Test]
        public void CurrentGetsItemsThatAreInQueue()
        {
            var pq = MakeBasicPQ();
            var en = pq.GetEnumerator();
            while (en.MoveNext())
            {
                Assert.IsTrue(pq.Contains(en.Current), "found an item that wasn't in the queue");
            }
        }

        [Test]
        public void ExtendsGenericIEnumerator()
        {
            var pq = MakeBasicPQ();
            var en = pq.GetEnumerator();
            Assert.IsInstanceOf<IEnumerator>(en);
        }

        [Test]
        public void ExtendsIDisposable()
        {
            var pq = MakeBasicPQ();
            var en = pq.GetEnumerator();
            Assert.IsInstanceOf<IDisposable>(en);
        }

        [Test]
        public void ExtendsIEnumerator()
        {
            var pq = MakeBasicPQ();
            var en = pq.GetEnumerator();
            Assert.IsInstanceOf<IEnumerator<int>>(en);
        }

        /// <summary>
        /// Getting an enumerator on a queue with only 1 item
        /// </summary>
        [Test]
        public void MoveNextOnEmptyQueueReturnsFalse()
        {
            var pq = new PriorityQueue<int>();
            var en = pq.GetEnumerator();
            Assert.IsFalse(en.MoveNext(), "MoveNext did not return false");
        }

        /// <summary>
        /// Moving past the end of the enumerator should return false
        /// </summary>
        [Test]
        public void MoveNextReturnsFalsePastEnd()
        {
            var pq = MakeBasicPQ();
            var en = pq.GetEnumerator();
            en.MoveNext();
            en.MoveNext();
            en.MoveNext();
            Assert.IsFalse(en.MoveNext(), "MoveNext did not return false");
        }

        /// <summary>
        /// Getting an enumerator on a queue with only 1 item
        /// </summary>
        [Test]
        public void MoveNextReturnsTrue()
        {
            var pq = MakeBasicPQ();
            var en = pq.GetEnumerator();
            Assert.IsTrue(en.MoveNext(), "MoveNext did not return true");
        }

        /// <summary>
        /// MoveNext should throw an exception if the underlying connection is changed
        /// </summary>
        [Test]
        public void MoveNextThrowsExceptionAfterDequeue()
        {
            var pq = MakeBasicPQ();
            var en = pq.GetEnumerator();
            en.MoveNext();
            pq.Dequeue();
            Assert.Throws<InvalidOperationException>(() => en.MoveNext());
        }

        /// <summary>
        /// MoveNext should throw an exception if the underlying connection is changed
        /// </summary>
        [Test]
        public void MoveNextThrowsExceptionAfterEnqueue()
        {
            var pq = MakeBasicPQ();
            var en = pq.GetEnumerator();
            en.MoveNext();
            pq.Enqueue(3);
            Assert.Throws<InvalidOperationException>(() => en.MoveNext());
        }

        /// <summary>
        /// Changing the queue should invalidate the enumeration
        /// </summary>
        public void ResetThrowsExceptionAfterDequeue()
        {
            var pq = MakeBasicPQ();
            var en = pq.GetEnumerator();
            en.MoveNext();
            pq.Dequeue();
            Assert.Throws<InvalidOperationException>(() => en.Reset());
        }

        /// <summary>
        /// Changing the queue should invalidate the enumeration
        /// </summary>
        [Test]
        public void ResetThrowsExceptionAfterEnqueue()
        {
            var pq = MakeBasicPQ();
            var en = pq.GetEnumerator();
            en.MoveNext();
            pq.Enqueue(3);
            Assert.Throws<InvalidOperationException>(() => en.Reset());
        }

        private PriorityQueue<int> MakeBasicPQ()
        {
            var pq = new PriorityQueue<int>();
            for (var i = 0; i < 3; ++i)
            {
                pq.Enqueue(i);
            }

            return pq;
        }
    }
}
