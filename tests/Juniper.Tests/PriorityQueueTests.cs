/*
 * FILE: PriorityQueue__test.cs
 * AUTHOR: Sean T. McBeth
 * DATE: JAN-24-2007
 */

using System;
using System.Collections;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Collections.Tests
{
    /// <summary>
    /// Unit tests for PriorityQueue class.
    /// </summary>
    [TestClass]
    public class PriorityQueueTests
    {
        /// <summary>
        /// Clearing an empty queue should have no effect
        /// </summary>
        [TestMethod]
        public void ClearHasNoEffectOnEmptyQueue()
        {
            var pq = new PriorityQueue<int>();
            pq.Clear();
            Assert.AreEqual(0, pq.Count);
        }

        /// <summary>
        /// Clearing the queue should reset the count to 0
        /// </summary>
        [TestMethod]
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
        [TestMethod]
        public void ComparerPropertyExplicit()
        {
            var comp = new MockComparer();
            var pq = new PriorityQueue<int>(comp);
            Assert.AreSame(comp, pq.Comparer);
        }

        /// <summary>
        /// The queue should return a Comparer for the type it was created for.
        /// </summary>
        [TestMethod]
        public void ComparerPropertyImplicit1()
        {
            var pq = new PriorityQueue<int>();
            Assert.IsInstanceOfType(pq.Comparer, typeof(IComparer<int>));
        }

        /// <summary>
        /// The queue needs a simple default constructor
        /// </summary>
        [TestMethod]
        public void ConstructorSimple()
        {
            var pq = new PriorityQueue<int>();
            Assert.IsNotNull(pq);
        }

        /// <summary>
        /// The queue needs a constructor that takes a Comparer
        /// </summary>
        [TestMethod]
        public void ConstructorWithComparer()
        {
            var comp = new MockComparer();
            var pq = new PriorityQueue<int>(comp);
            Assert.IsNotNull(pq);
        }

        /// <summary>
        /// using a null comparer parameter should throw an ArgumentNullException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorWithNullComparer()
        {
            var pq = new PriorityQueue<int>(null, null);
            Assert.IsNotNull(pq);
        }

        /// <summary>
        /// Checking for an ojbect that is in the queue should return true
        /// </summary>
        [TestMethod]
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
        [TestMethod]
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
        [TestMethod]
        public void ContainsReturnsFalseOnEmpty()
        {
            var pq = new PriorityQueue<int>();
            Assert.IsFalse(pq.Contains(5));
        }

        /// <summary>
        /// Checking for an int that isn't in the queue should return false
        /// </summary>
        [TestMethod]
        public void ContainsReturnsFalseOnNonExistant()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(5);
            Assert.IsFalse(pq.Contains(7));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CopyToThrowsExceptionFromNullArray()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(5);
            pq.CopyTo(null, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CopyToThrowsExceptionIfArrayIsTooSmall()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(3);
            pq.Enqueue(5);
            pq.Enqueue(7);
            var arr = new int[1];
            pq.CopyTo(arr, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CopyToThrowsExceptionIfIndexIsLessThanZero()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(3);
            var arr = new int[1];
            pq.CopyTo(arr, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CopyToThrowsExceptionIfIndexIsPastEnd()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(3);
            var arr = new int[1];
            pq.CopyTo(arr, 1);
        }

        /// <summary>
        /// Copy the queue to an array from the very beginning
        /// </summary>
        [TestMethod]
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
        [TestMethod]
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
        [TestMethod]
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
        [TestMethod]
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
        [TestMethod]
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
        [TestMethod]
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
        [TestMethod]
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
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DequeueThrowsExceptionOnEmptyQueue()
        {
            var pq = new PriorityQueue<int>();
            pq.Dequeue();
        }

        /// <summary>
        /// Enqueueing a single int should return that int when it is dequeued
        /// </summary>
        [TestMethod]
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
        [TestMethod]
        public void EnqueueIncreasesCount()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(3);
            Assert.AreEqual(1, pq.Count);
        }

        [TestMethod]
        public void ExtendsGenericIEnumerable()
        {
            var pq = new PriorityQueue<int>();
            Assert.IsInstanceOfType(pq, typeof(IEnumerable<int>));
        }

        [TestMethod]
        public void ExtendsICollection()
        {
            var pq = new PriorityQueue<int>();
            Assert.IsInstanceOfType(pq, typeof(ICollection));
        }

        /// <summary>
        /// To be a proper collection, it should extend the IEnumberable interface
        /// </summary>
        [TestMethod]
        public void ExtendsIEnumerable()
        {
            var pq = new PriorityQueue<int>();
            Assert.IsInstanceOfType(pq, typeof(IEnumerable));
        }

        /// <summary>
        /// Getting an enumerator on an empty queue should not be null
        /// </summary>
        [TestMethod]
        public void GetEnumeratorOnEmpty()
        {
            var pq = new PriorityQueue<int>();
            Assert.IsNotNull(pq.GetEnumerator());
        }

        /// <summary>
        /// When first created, the Count should return 0
        /// </summary>
        [TestMethod]
        public void InitiallyEmpty()
        {
            var pq = new PriorityQueue<int>();
            Assert.AreEqual(0, pq.Count);
        }

        /// <summary>
        /// Since the queue is thread safe, it should always return true for IsSynchronized
        /// </summary>
        [TestMethod]
        public void IsSynchronizedReturnsTrue()
        {
            var pq = new PriorityQueue<int>();
            Assert.IsTrue(pq.IsSynchronized);
        }

        /// <summary>
        /// Peeking should not change the size of the queue
        /// </summary>
        [TestMethod]
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
        [TestMethod]
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
        [TestMethod]
        public void PeekRetrievesTheSameItemAsDequeue()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(3);
            Assert.AreEqual(pq.Peek(), pq.Dequeue());
        }

        /// <summary>
        /// Peeking an empty queue should thow an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void PeekThrowsExceptionOnEmptyQueue()
        {
            var pq = new PriorityQueue<int>();
            pq.Peek();
        }

        /// <summary>
        /// A priority queue on integers should result in a sorted list when sequentially dequeueing
        /// the items.
        /// </summary>
        [TestMethod]
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
                Console.WriteLine("{0} -> {1}", last, i);
                Assert.IsTrue(last <= i, "items out of sequence in queue");
                last = i;
            }
        }

        /// <summary>
        /// A queue of strings should sort the strings lexicographically (aka "alphabetically")
        /// </summary>
        [TestMethod]
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

        [TestMethod]
        public void SyncRootReturnsNonNullReference()
        {
            var pq = new PriorityQueue<int>();
            Assert.IsNotNull(pq.SyncRoot);
        }

        [TestMethod]
        public void SyncRootReturnsUniqueReferences()
        {
            var pq1 = new PriorityQueue<int>();
            var pq2 = new PriorityQueue<int>();
            Assert.AreNotSame(pq1.SyncRoot, pq2.SyncRoot);
        }
    }

    internal class MockComparer : IComparer<int>
    {
        public int Compare(int obj1, int obj2)
        {
            return 0;
        }
    }
}
