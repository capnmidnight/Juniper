/*
 * FILE: PriorityQueue__test.cs
 * AUTHOR: Sean T. McBeth
 * DATE: JAN-24-2007
 */

using NUnit.Framework;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Juniper.Collections
{
    /// <summary>
    /// Unit tests for PriorityQueue class.
    /// </summary>
    [TestFixture]
    public class PriorityQueueTests
    {
        /// <summary>
        /// Clearing an empty queue should have no effect
        /// </summary>
        [Test]
        public void ClearHasNoEffectOnEmptyQueue()
        {
            var pq = new PriorityQueue<int>();
            pq.Clear();
            Assert.AreEqual(0, pq.Count);
        }

        /// <summary>
        /// Clearing the queue should reset the count to 0
        /// </summary>
        [Test]
        public void ClearResetsCountToZero()
        {
            var pq = new PriorityQueue<int>();
            for (var i = 0; i < 10; ++i)
            {
                pq.Enqueue(i);
            }

            pq.Clear();
            Assert.AreEqual(0, pq.Count);
        }

        /// <summary>
        /// The queue should return the Comparer that it was created with.
        /// </summary>
        [Test]
        public void ComparerPropertyExplicit()
        {
            var comp = new MockComparer();
            var pq = new PriorityQueue<int>(comp);
            Assert.AreSame(comp, pq.Comparer);
        }

        /// <summary>
        /// The queue should return a Comparer for the type it was created for.
        /// </summary>
        [Test]
        public void ComparerPropertyImplicit1()
        {
            var pq = new PriorityQueue<int>();
            Assert.IsInstanceOf<IComparer<int>>(pq.Comparer);
        }

        /// <summary>
        /// The queue needs a simple default constructor
        /// </summary>
        [Test]
        public void ConstructorSimple()
        {
            var pq = new PriorityQueue<int>();
            Assert.IsNotNull(pq);
        }

        /// <summary>
        /// The queue needs a constructor that takes a Comparer
        /// </summary>
        [Test]
        public void ConstructorWithComparer()
        {
            var comp = new MockComparer();
            var pq = new PriorityQueue<int>(comp);
            Assert.IsNotNull(pq);
        }

        /// <summary>
        /// using a null comparer parameter should throw an ArgumentNullException
        /// </summary>
        [Test]
        public void ConstructorWithNullComparer()
        {
            Assert.Throws<ArgumentNullException>(() => new PriorityQueue<int>(null, null));
        }

        /// <summary>
        /// Checking for an ojbect that is in the queue should return true
        /// </summary>
        [Test]
        public void ContainsFindsReferenceTypes()
        {
            var pq = new PriorityQueue<int>();
            const int o = 2;
            pq.Enqueue(3);
            pq.Enqueue(o);
            pq.Enqueue(5);
            Assert.IsTrue(pq.Contains(o));
        }

        /// <summary>
        /// Checking for an ojbect that is in the queue should return true
        /// </summary>
        [Test]
        public void ContainsFindsValueTypes()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(1);
            pq.Enqueue(1);
            pq.Enqueue(1);
            pq.Enqueue(2);
            pq.Enqueue(2);
            pq.Enqueue(3);
            pq.Enqueue(3);
            pq.Enqueue(3);
            Assert.IsTrue(pq.Contains(2));
        }

        /// <summary>
        /// Checking for an int in an empty queue should return false
        /// </summary>
        [Test]
        public void ContainsReturnsFalseOnEmpty()
        {
            var pq = new PriorityQueue<int>();
            Assert.IsFalse(pq.Contains(5));
        }

        /// <summary>
        /// Checking for an int that isn't in the queue should return false
        /// </summary>
        [Test]
        public void ContainsReturnsFalseOnNonExistant()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(5);
            Assert.IsFalse(pq.Contains(7));
        }

        [Test]
        public void CopyToThrowsExceptionFromNullArray()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(5);
            Assert.Throws<ArgumentNullException>(() => pq.CopyTo(null, 0));
        }

        [Test]
        public void CopyToThrowsExceptionIfArrayIsTooSmall()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(3);
            pq.Enqueue(5);
            pq.Enqueue(7);
            var arr = new int[1];
            Assert.Throws<ArgumentException>(() => pq.CopyTo(arr, 0));
        }

        [Test]
        public void CopyToThrowsExceptionIfIndexIsLessThanZero()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(3);
            var arr = new int[1];
            Assert.Throws<ArgumentOutOfRangeException>(() => pq.CopyTo(arr, -1));
        }

        [Test]
        public void CopyToThrowsExceptionIfIndexIsPastEnd()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(3);
            var arr = new int[1];
            Assert.Throws<ArgumentException>(() => pq.CopyTo(arr, 1));
        }

        /// <summary>
        /// Copy the queue to an array from the very beginning
        /// </summary>
        [Test]
        public void CopyToWithOneElement()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(3);
            var arr = new int[1];
            pq.CopyTo(arr, 0);
            Assert.AreEqual(pq.Peek(), arr[0]);
        }

        /// <summary>
        /// Copy the queue to an array from the very beginning
        /// </summary>
        [Test]
        public void CopyToWithOneElementAndOffset()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(3);
            var arr = new int[2];
            pq.CopyTo(arr, 1);
            Assert.AreEqual(0, arr[0]);
            Assert.AreEqual(pq.Peek(), arr[1]);
        }

        /// <summary>
        /// Copy the queue to an array from the very beginning
        /// </summary>
        [Test]
        public void CopyToWithTenElements()
        {
            var pq = new PriorityQueue<int>();
            var r = new Random();
            for (var i = 0; i < 10; ++i)
            {
                pq.Enqueue(r.Next(10));
            }

            var arr = new int[10];
            pq.CopyTo(arr, 0);
            for (var i = 0; i < arr.Length; ++i)
            {
                Assert.IsTrue(pq.Contains(arr[i]));
            }
        }

        /// <summary>
        /// Copy the queue to an array from the very beginning
        /// </summary>
        [Test]
        public void CopyToWithTenElementsWithOffset()
        {
            var pq = new PriorityQueue<int>();
            for (var i = 0; i < 10; ++i)
            {
                pq.Enqueue(i + 1);
            }

            var arr = new int[13];
            pq.CopyTo(arr, 3);
            for (var i = 0; i < arr.Length; ++i)
            {
                if (i < 3)
                {
                    Assert.AreEqual(0, arr[i]);
                }
                else
                {
                    Assert.IsTrue(pq.Contains(arr[i]));
                }
            }
        }

        /// <summary>
        /// The queue should support a means for creating a raw array of the items contained within
        /// </summary>
        [Test]
        public void CreateArrayFromQueue()
        {
            var pq = new PriorityQueue<int>();
            for (var i = 0; i < 10; ++i)
            {
                pq.Enqueue(i);
            }

            var arr = pq.ToArray();
            for (var i = 0; i < 10; ++i)
            {
                Assert.IsTrue(pq.Contains(arr[i]));
            }
        }

        /// <summary>
        /// The queue should return a Comparer for the type it was created for.
        /// </summary>
        [Test]
        public void DefaultComparerReturnsSameResultsAsNormalComparer()
        {
            var pq = new PriorityQueue<int>();
            var comp = pq.Comparer;
            const int x = 11;
            const int y = 13;
            Assert.AreEqual(x.CompareTo(y), comp.Compare(x, y));
        }

        /// <summary>
        /// Dequeuing an item should decrease the Count of the queue
        /// </summary>
        [Test]
        public void DequeueDecreasesCount()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(3);
            pq.Enqueue(5);
            pq.Enqueue(7);
            Assert.AreEqual(3, pq.Count);
            pq.Dequeue();
            Assert.AreEqual(2, pq.Count);
        }

        /// <summary>
        /// Dequeueing an empty queue should throw an Exception
        /// </summary>
        [Test]
        public void DequeueThrowsExceptionOnEmptyQueue()
        {
            var pq = new PriorityQueue<int>();
            Assert.Throws<ArgumentOutOfRangeException>(() => pq.Dequeue());
        }

        /// <summary>
        /// Enqueueing a single int should return that int when it is dequeued
        /// </summary>
        [Test]
        public void EnqueueDequeueOneItem()
        {
            const int obj = 3;
            var pq = new PriorityQueue<int>();
            pq.Enqueue(obj);
            Assert.AreEqual(obj, pq.Dequeue());
        }

        /// <summary>
        /// Adding items to the queue should increase the Count of the queue
        /// </summary>
        [Test]
        public void EnqueueIncreasesCount()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(3);
            Assert.AreEqual(1, pq.Count);
        }

        [Test]
        public void ExtendsGenericIEnumerable()
        {
            var pq = new PriorityQueue<int>();
            Assert.IsInstanceOf<IEnumerable<int>>(pq);
        }

        [Test]
        public void ExtendsICollection()
        {
            var pq = new PriorityQueue<int>();
            Assert.IsInstanceOf<ICollection>(pq);
        }

        /// <summary>
        /// To be a proper collection, it should extend the IEnumberable interface
        /// </summary>
        [Test]
        public void ExtendsIEnumerable()
        {
            var pq = new PriorityQueue<int>();
            Assert.IsInstanceOf<IEnumerable>(pq);
        }

        /// <summary>
        /// Getting an enumerator on an empty queue should not be null
        /// </summary>
        [Test]
        public void GetEnumeratorOnEmpty()
        {
            var pq = new PriorityQueue<int>();
            Assert.IsNotNull(pq.GetEnumerator());
        }

        /// <summary>
        /// When first created, the Count should return 0
        /// </summary>
        [Test]
        public void InitiallyEmpty()
        {
            var pq = new PriorityQueue<int>();
            Assert.AreEqual(0, pq.Count);
        }

        /// <summary>
        /// Since the queue is thread safe, it should always return true for IsSynchronized
        /// </summary>
        [Test]
        public void IsSynchronizedReturnsTrue()
        {
            var pq = new PriorityQueue<int>();
            Assert.IsTrue(pq.IsSynchronized);
        }

        /// <summary>
        /// Peeking should not change the size of the queue
        /// </summary>
        [Test]
        public void PeekDoesntChangeCount()
        {
            var pq = new PriorityQueue<int>();
            const int obj = 3;
            pq.Enqueue(obj);
            pq.Peek();
            Assert.AreEqual(1, pq.Count);
        }

        /// <summary>
        /// Peeking should show the first element in the queue
        /// </summary>
        [Test]
        public void PeekGetsTheOnlyItemInQueue()
        {
            var pq = new PriorityQueue<int>();
            const int obj = 3;
            pq.Enqueue(obj);
            Assert.AreEqual(obj, pq.Peek());
        }

        /// <summary>
        /// Peeking before Dequeueing should return the same int
        /// </summary>
        [Test]
        public void PeekRetrievesTheSameItemAsDequeue()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(3);
            Assert.AreEqual(pq.Peek(), pq.Dequeue());
        }

        /// <summary>
        /// Peeking an empty queue should thow an exception
        /// </summary>
        [Test]
        public void PeekThrowsExceptionOnEmptyQueue()
        {
            var pq = new PriorityQueue<int>();
            Assert.Throws<ArgumentOutOfRangeException>(() => pq.Peek());
        }

        /// <summary>
        /// A priority queue on integers should result in a sorted list when sequentially dequeueing
        /// the items.
        /// </summary>
        [Test]
        public void PQSortsInts()
        {
            var pq = new PriorityQueue<int>();
            var rand = new Random();
            for (var i = 0; i < 10; ++i)
            {
                pq.Enqueue(rand.Next(10));
            }

            var last = -1;
            while (pq.Count > 0)
            {
                var i = pq.Dequeue();
                System.Console.WriteLine("{0} -> {1}", last, i);
                Assert.IsTrue(last <= i, "items out of sequence in queue");
                last = i;
            }
        }

        /// <summary>
        /// A queue of strings should sort the strings lexicographically (aka "alphabetically")
        /// </summary>
        [Test]
        public void PQSortsStringsLexicographically()
        {
            var pq = new PriorityQueue<string>();
            pq.Enqueue("Hello");
            pq.Enqueue("Bob");
            pq.Enqueue("World");
            pq.Enqueue("tOM");
            Assert.AreEqual("Bob", pq.Dequeue());
            Assert.AreEqual("Hello", pq.Dequeue());
            Assert.AreEqual("tOM", pq.Dequeue());
            Assert.AreEqual("World", pq.Dequeue());
        }

        [Test]
        public void SyncRootReturnsNonNullReference()
        {
            var pq = new PriorityQueue<int>();
            Assert.IsNotNull(pq.SyncRoot);
        }

        [Test]
        public void SyncRootReturnsUniqueReferences()
        {
            var pq1 = new PriorityQueue<int>();
            var pq2 = new PriorityQueue<int>();
            Assert.AreNotSame(pq1.SyncRoot, pq2.SyncRoot);
        }
    }
}
