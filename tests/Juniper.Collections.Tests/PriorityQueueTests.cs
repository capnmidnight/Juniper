/*
 * FILE: PriorityQueue__test.cs
 * AUTHOR: Sean T. McBeth
 * DATE: JAN-24-2007
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Collections.Generic.Tests
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
            var pq = new PriorityQueue<object>();
            pq.Clear();
            Assert.AreEqual(0, pq.Count);
        }

        /// <summary>
        /// Clearing the queue should reset the count to 0
        /// </summary>
        [TestMethod]
        public void ClearResetsCountToZero()
        {
            var pq = new PriorityQueue<object>();
            for (var i = 0; i < 10; ++i)
            {
                pq.Enqueue(new object());
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
            IComparer<object> comp = new MockComparer();
            var pq = new PriorityQueue<object>(comp);
            Assert.AreSame(comp, pq.Comparer);
        }

        /// <summary>
        /// The queue should return a Comparer for the type it was created for.
        /// </summary>
        [TestMethod]
        public void ComparerPropertyImplicit1()
        {
            var pq = new PriorityQueue<object>();
            Assert.IsInstanceOfType(pq.Comparer, typeof(IComparer<object>));
        }

        /// <summary>
        /// The queue needs a simple default constructor
        /// </summary>
        [TestMethod]
        public void ConstructorSimple()
        {
            var pq = new PriorityQueue<object>();
            Assert.IsNotNull(pq);
        }

        /// <summary>
        /// The queue needs a constructor that takes a Comparer
        /// </summary>
        [TestMethod]
        public void ConstructorWithComparer()
        {
            IComparer<object> comp = new MockComparer();
            var pq = new PriorityQueue<object>(comp);
            Assert.IsNotNull(pq);
        }

        /// <summary>
        /// using a null comparer parameter should throw a NullReferenceException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void ConstructorWithNullComparer()
        {
            var pq = new PriorityQueue<object>(null);
            Assert.IsNotNull(pq);
        }

        /// <summary>
        /// Checking for an ojbect that is in the queue should return true
        /// </summary>
        [TestMethod]
        public void ContainsFindsReferenceTypes()
        {
            var pq = new PriorityQueue<object>();
            var o = new object();
            pq.Enqueue(new object());
            pq.Enqueue(o);
            pq.Enqueue(new object());
            Assert.IsTrue(pq.Contains(o));
        }

        /// <summary>
        /// Checking for an ojbect that is in the queue should return true
        /// </summary>
        [TestMethod]
        public void ContainsFindsValueTypes()
        {
            var pq = new PriorityQueue<object>();
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
        /// Checking for an object in an empty queue should return false
        /// </summary>
        [TestMethod]
        public void ContainsReturnsFalseOnEmpty()
        {
            var pq = new PriorityQueue<object>();
            Assert.IsFalse(pq.Contains(new object()));
        }

        /// <summary>
        /// Checking for an object that isn't in the queue should return false
        /// </summary>
        [TestMethod]
        public void ContainsReturnsFalseOnNonExistant()
        {
            var pq = new PriorityQueue<object>();
            pq.Enqueue(new object());
            Assert.IsFalse(pq.Contains(new object()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CopyToThrowsExceptionFromNullArray()
        {
            var pq = new PriorityQueue<object>();
            pq.Enqueue(new object());
            pq.CopyTo(null, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CopyToThrowsExceptionIfArrayIsTooSmall()
        {
            var pq = new PriorityQueue<object>();
            pq.Enqueue(new object());
            pq.Enqueue(new object());
            pq.Enqueue(new object());
            var arr = new object[1];
            pq.CopyTo(arr, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CopyToThrowsExceptionIfIndexIsLessThanZero()
        {
            var pq = new PriorityQueue<object>();
            pq.Enqueue(new object());
            var arr = new object[1];
            pq.CopyTo(arr, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CopyToThrowsExceptionIfIndexIsPastEnd()
        {
            var pq = new PriorityQueue<object>();
            pq.Enqueue(new object());
            var arr = new object[1];
            pq.CopyTo(arr, 1);
        }

        /// <summary>
        /// Copy the queue to an array from the very beginning
        /// </summary>
        [TestMethod]
        public void CopyToWithOneElement()
        {
            var pq = new PriorityQueue<object>();
            pq.Enqueue(new object());
            var arr = new object[1];
            pq.CopyTo(arr, 0);
            Assert.AreSame(pq.Peek(), arr[0]);
        }

        /// <summary>
        /// Copy the queue to an array from the very beginning
        /// </summary>
        [TestMethod]
        public void CopyToWithOneElementAndOffset()
        {
            var pq = new PriorityQueue<object>();
            pq.Enqueue(new object());
            var arr = new object[2];
            pq.CopyTo(arr, 1);
            Assert.IsNull(arr[0]);
            Assert.AreSame(pq.Peek(), arr[1]);
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
            var pq = new PriorityQueue<object>();
            for (var i = 0; i < 10; ++i)
            {
                pq.Enqueue(new object());
            }
            var arr = new object[13];
            pq.CopyTo(arr, 3);
            for (var i = 0; i < arr.Length; ++i)
            {
                if (i < 3)
                {
                    Assert.IsNull(arr[i]);
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
            var pq = new PriorityQueue<object>();
            for (var i = 0; i < 10; ++i)
            {
                pq.Enqueue(new object());
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
            var x = 11;
            var y = 13;
            Assert.AreEqual(x.CompareTo(y), comp.Compare(y, x));
        }

        /// <summary>
        /// Dequeuing an item should decrease the Count of the queue
        /// </summary>
        [TestMethod]
        public void DequeueDecreasesCount()
        {
            var pq = new PriorityQueue<object>();
            pq.Enqueue(new object());
            pq.Enqueue(new object());
            pq.Enqueue(new object());
            Assert.AreEqual(3, pq.Count);
            pq.Dequeue();
            Assert.AreEqual(2, pq.Count);
        }

        /// <summary>
        /// Dequeueing an empty queue should throw an Exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DequeueThrowsExceptionOnEmptyQueue()
        {
            var pq = new PriorityQueue<object>();
            pq.Dequeue();
        }

        /// <summary>
        /// Enqueueing a single object should return that object when it is dequeued
        /// </summary>
        [TestMethod]
        public void EnqueueDequeueOneItem()
        {
            var obj = new object();
            var pq = new PriorityQueue<object>();
            pq.Enqueue(obj);
            Assert.AreSame(obj, pq.Dequeue());
        }

        /// <summary>
        /// Adding items to the queue should increase the Count of the queue
        /// </summary>
        [TestMethod]
        public void EnqueueIncreasesCount()
        {
            var pq = new PriorityQueue<object>();
            pq.Enqueue(new object());
            Assert.AreEqual(1, pq.Count);
        }

        /// <summary>
        /// Trying to enqueue a null object should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void EnqueueingNullThrowsException()
        {
            var pq = new PriorityQueue<object>();
            pq.Enqueue(null);
        }

        [TestMethod]
        public void ExtendsGenericIEnumerable()
        {
            var pq = new PriorityQueue<object>();
            Assert.IsInstanceOfType(pq, typeof(IEnumerable<object>));
        }

        [TestMethod]
        public void ExtendsICollection()
        {
            var pq = new PriorityQueue<object>();
            Assert.IsInstanceOfType(pq, typeof(ICollection));
        }

        /// <summary>
        /// To be a proper collection, it should extend the IEnumberable interface
        /// </summary>
        [TestMethod]
        public void ExtendsIEnumerable()
        {
            var pq = new PriorityQueue<object>();
            Assert.IsInstanceOfType(pq, typeof(IEnumerable));
        }

        /// <summary>
        /// Getting an enumerator on an empty queue should not be null
        /// </summary>
        [TestMethod]
        public void GetEnumeratorOnEmpty()
        {
            var pq = new PriorityQueue<object>();
            Assert.IsNotNull(pq.GetEnumerator());
        }

        /// <summary>
        /// When first created, the Count should return 0
        /// </summary>
        [TestMethod]
        public void InitiallyEmpty()
        {
            var pq = new PriorityQueue<object>();
            Assert.AreEqual(0, pq.Count);
        }

        /// <summary>
        /// Since the queue is thread safe, it should always return true for IsSynchronized
        /// </summary>
        [TestMethod]
        public void IsSynchronizedReturnsTrue()
        {
            var pq = new PriorityQueue<object>();
            Assert.IsTrue(pq.IsSynchronized);
        }

        /// <summary>
        /// Non-comparable objects placed in the priority queue should make the PQ act like a normal queue
        /// </summary>
        [TestMethod]
        public void NonComparableObjectsCreatesNormalQueue()
        {
            object o1, o2, o3;
            o1 = new object();
            o2 = new object();
            o3 = new object();

            var pq = new PriorityQueue<object>();
            pq.Enqueue(o1);
            pq.Enqueue(o2);
            pq.Enqueue(o3);

            Assert.AreSame(o1, pq.Dequeue());
            Assert.AreSame(o2, pq.Dequeue());
            Assert.AreSame(o3, pq.Dequeue());
        }

        /// <summary>
        /// Peeking should not change the size of the queue
        /// </summary>
        [TestMethod]
        public void PeekDoesntChangeCount()
        {
            var pq = new PriorityQueue<object>();
            var obj = new object();
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
            var pq = new PriorityQueue<object>();
            var obj = new object();
            pq.Enqueue(obj);
            Assert.AreSame(obj, pq.Peek());
        }

        /// <summary>
        /// Peeking before Dequeueing should return the same object
        /// </summary>
        [TestMethod]
        public void PeekRetrievesTheSameItemAsDequeue()
        {
            var pq = new PriorityQueue<object>();
            pq.Enqueue(new object());
            Assert.AreSame(pq.Peek(), pq.Dequeue());
        }

        /// <summary>
        /// Peeking an empty queue should thow an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PeekThrowsExceptionOnEmptyQueue()
        {
            var pq = new PriorityQueue<object>();
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
            var pq = new PriorityQueue<object>();
            Assert.IsNotNull(pq.SyncRoot);
        }

        [TestMethod]
        public void SyncRootReturnsUniqueReferences()
        {
            var pq1 = new PriorityQueue<object>();
            var pq2 = new PriorityQueue<object>();
            Assert.AreNotSame(pq1.SyncRoot, pq2.SyncRoot);
        }
    }

    internal class MockComparer : IComparer<object>
    {
        public int Compare(object obj1, object obj2)
        {
            return 0;
        }
    }
}
