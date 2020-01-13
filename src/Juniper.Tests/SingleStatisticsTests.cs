using System;

using NUnit.Framework;

using static System.Math;

namespace Juniper.Mathematics.Tests
{
    [TestFixture]
    public class SingleStatisticsTests
    {
        [Test]
        public void MakeABuffer()
        {
            Assert.IsNotNull(new SingleStatisticsCollection(100));
        }

        [Test]
        public void StartsEmpty()
        {
            var buffer = new SingleStatisticsCollection(100);
            Assert.AreEqual(0, buffer.Count);
        }

        [Test]
        public void ThrowsExceptionWithNoCapacity()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new SingleStatisticsCollection(0));
        }

        [Test]
        public void AddingIncreasesCount()
        {
            var buffer = new SingleStatisticsCollection(1)
            {
                5
            };
            Assert.AreEqual(1, buffer.Count);
        }

        [Test]
        public void CanGetBackOutAgain()
        {
            var buffer = new SingleStatisticsCollection(1)
            {
                7
            };
            Assert.AreEqual(7, buffer[0]);
        }

        [Test]
        public void CanGetBackOutAgain2()
        {
            var buffer = new SingleStatisticsCollection(5);
            Assert.AreEqual(0, buffer.Count);
            buffer.Add(1);
            Assert.AreEqual(1, buffer.Count);
            Assert.AreEqual(1, buffer[0]);
            buffer.Add(2);
            Assert.AreEqual(2, buffer.Count);
            Assert.AreEqual(1, buffer[0]);
            Assert.AreEqual(2, buffer[1]);
            buffer.Add(3);
            Assert.AreEqual(3, buffer.Count);
            Assert.AreEqual(1, buffer[0]);
            Assert.AreEqual(2, buffer[1]);
            Assert.AreEqual(3, buffer[2]);
            buffer.Add(4);
            Assert.AreEqual(4, buffer.Count);
            Assert.AreEqual(1, buffer[0]);
            Assert.AreEqual(2, buffer[1]);
            Assert.AreEqual(3, buffer[2]);
            Assert.AreEqual(4, buffer[3]);
            buffer.Add(5);
            Assert.AreEqual(5, buffer.Count);
            Assert.AreEqual(1, buffer[0]);
            Assert.AreEqual(2, buffer[1]);
            Assert.AreEqual(3, buffer[2]);
            Assert.AreEqual(4, buffer[3]);
            Assert.AreEqual(5, buffer[4]);
        }

        [Test]
        public void LoopingDoesntGrowSize()
        {
            var buffer = new SingleStatisticsCollection(1)
            {
                3,
                2
            };
            Assert.AreEqual(1, buffer.Count);
        }

        [Test]
        public void LoopingOverwrites()
        {
            var buffer = new SingleStatisticsCollection(1)
            {
                3,
                2
            };
            Assert.AreEqual(2, buffer[0]);
        }

        [Test]
        public void LoopingAgainDoesntGrowSize()
        {
            var buffer = new SingleStatisticsCollection(1)
            {
                3,
                2,
                1
            };
            Assert.AreEqual(1, buffer.Count);
        }

        [Test]
        public void LoopingAgainOverwrites()
        {
            var buffer = new SingleStatisticsCollection(1)
            {
                3,
                2,
                1
            };
            Assert.AreEqual(1, buffer[0]);
        }

        [Test]
        public void LargerLooping()
        {
            var buffer = new SingleStatisticsCollection(100);
            for (var i = 0; i < 120; ++i)
            {
                buffer.Add(i);
                Assert.AreEqual(Min(100, i + 1), buffer.Count);
            }
        }

        [Test]
        public void MaintainsOrder()
        {
            var buffer = new SingleStatisticsCollection(5)
            {
                5,
                3,
                4,
                1,
                2
            };
            Assert.AreEqual(5, buffer[0]);
            Assert.AreEqual(3, buffer[1]);
            Assert.AreEqual(4, buffer[2]);
            Assert.AreEqual(1, buffer[3]);
            Assert.AreEqual(2, buffer[4]);
        }

        [Test]
        public void ShiftsOldValuesOut()
        {
            var buffer = new SingleStatisticsCollection(4)
            {
                5,
                3,
                4,
                1
            };
            Assert.AreEqual(5, buffer[0]);
            Assert.AreEqual(3, buffer[1]);
            Assert.AreEqual(4, buffer[2]);
            Assert.AreEqual(1, buffer[3]);
            buffer.Add(2);
            Assert.AreEqual(3, buffer[0]);
            Assert.AreEqual(4, buffer[1]);
            Assert.AreEqual(1, buffer[2]);
            Assert.AreEqual(2, buffer[3]);
        }

        [Test]
        public void ComputesMin()
        {
            var buffer = new SingleStatisticsCollection(4)
            {
                5,
                3,
                4,
                1
            };
            Assert.AreEqual(1, buffer.Minimum);
        }

        [Test]
        public void ComputesMax()
        {
            var buffer = new SingleStatisticsCollection(4)
            {
                5,
                3,
                4,
                1
            };
            Assert.AreEqual(5, buffer.Maximum);
        }

        [Test]
        public void ComputesMean()
        {
            var buffer = new SingleStatisticsCollection(4)
            {
                5,
                3,
                4,
                1
            };
            Assert.AreEqual(3.25f, buffer.Mean);
        }

        [Test]
        public void ComputesMedian()
        {
            var buffer = new SingleStatisticsCollection(4)
            {
                5,
                3,
                4,
                1
            };
            Assert.AreEqual(3f, buffer.Median);
        }

        [Test]
        public void ComputesTrivialStandardDeviation()
        {
            var buffer = new SingleStatisticsCollection(4)
            {
                5,
                5,
                5,
                5
            };
            Assert.AreEqual(0f, buffer.StandardDeviation);
        }

        [Test]
        public void ComputesBasicStandardDeviation()
        {
            var buffer = new SingleStatisticsCollection(4)
            {
                0,
                2,
                0,
                2
            };
            Assert.AreEqual((float)Sqrt(4 / 3f), buffer.StandardDeviation);
        }

        [Test]
        public void ComputesStandardDeviation()
        {
            var buffer = new SingleStatisticsCollection(4)
            {
                5,
                3,
                4,
                1
            };
            Assert.AreEqual(1.70782518f, buffer.StandardDeviation);
        }

        [Test]
        public void CopyUnfullWithoutOffsetThrowsExceptionWithTooSmallDestination()
        {
            var buffer = new SingleStatisticsCollection(4)
            {
                5,
                7,
                3
            };
            var arr = new float[2];
            Assert.Throws<ArgumentException>(() =>
                buffer.CopyTo(arr, 0));
        }

        [Test]
        public void CopyUnfullWithOffsetThrowsExceptionWithTooSmallDestination()
        {
            var buffer = new SingleStatisticsCollection(4)
            {
                5,
                7,
                3
            };
            var arr = new float[3];
            Assert.Throws<ArgumentException>(() =>
                buffer.CopyTo(arr, 1));
        }

        [Test]
        public void CopyWithNegativeOffsetThrowsException()
        {
            var buffer = new SingleStatisticsCollection(4)
            {
                5,
                7,
                3
            };
            var arr = new float[3];
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                buffer.CopyTo(arr, -1));
        }

        [Test]
        public void CopyWithTooLargeOffsetThrowsException()
        {
            var buffer = new SingleStatisticsCollection(4)
            {
                5,
                7,
                3
            };
            var arr = new float[3];
            Assert.Throws<ArgumentException>(() =>
                buffer.CopyTo(arr, 3));
        }

        [Test]
        public void CopyUnfullUnwrappedBufferWithoutOffsetMaintainsOrder()
        {
            var buffer = new SingleStatisticsCollection(4)
            {
                5,
                7,
                3
            };
            var arr = new float[3];
            buffer.CopyTo(arr, 0);
            Assert.AreEqual(5, arr[0]);
            Assert.AreEqual(7, arr[1]);
            Assert.AreEqual(3, arr[2]);
        }

        [Test]
        public void CopyUnfullUnwrappedBufferWithOffsetMaintainsOrder()
        {
            var buffer = new SingleStatisticsCollection(4)
            {
                5,
                7,
                3
            };
            var arr = new float[4];
            buffer.CopyTo(arr, 1);
            Assert.AreEqual(0, arr[0]);
            Assert.AreEqual(5, arr[1]);
            Assert.AreEqual(7, arr[2]);
            Assert.AreEqual(3, arr[3]);
        }

        [Test]
        public void CopyFullUnwrappedBufferWithoutOffsetMaintainsOrder()
        {
            var buffer = new SingleStatisticsCollection(4)
            {
                5,
                7,
                3,
                13
            };
            var arr = new float[4];
            buffer.CopyTo(arr, 0);
            Assert.AreEqual(5, arr[0]);
            Assert.AreEqual(7, arr[1]);
            Assert.AreEqual(3, arr[2]);
            Assert.AreEqual(13, arr[3]);
        }

        [Test]
        public void CopyFullUnwrappedBufferWithOffsetMaintainsOrder()
        {
            var buffer = new SingleStatisticsCollection(4)
            {
                5,
                7,
                3,
                13
            };
            var arr = new float[5];
            buffer.CopyTo(arr, 1);
            Assert.AreEqual(0, arr[0]);
            Assert.AreEqual(5, arr[1]);
            Assert.AreEqual(7, arr[2]);
            Assert.AreEqual(3, arr[3]);
            Assert.AreEqual(13, arr[4]);
        }

        [Test]
        public void CopyWrappedBufferWithoutOffsetMaintainsOrder()
        {
            var buffer = new SingleStatisticsCollection(4)
            {
                5,
                7,
                3,
                13,
                2
            };
            var arr = new float[4];
            buffer.CopyTo(arr, 0);
            Assert.AreEqual(7, arr[0]);
            Assert.AreEqual(3, arr[1]);
            Assert.AreEqual(13, arr[2]);
            Assert.AreEqual(2, arr[3]);
        }

        [Test]
        public void CopyWrappedBufferWithOffsetMaintainsOrder()
        {
            var buffer = new SingleStatisticsCollection(4)
            {
                5,
                7,
                3,
                13,
                2
            };
            var arr = new float[5];
            buffer.CopyTo(arr, 1);
            Assert.AreEqual(0, arr[0]);
            Assert.AreEqual(7, arr[1]);
            Assert.AreEqual(3, arr[2]);
            Assert.AreEqual(13, arr[3]);
            Assert.AreEqual(2, arr[4]);
        }

        [Test]
        public void InsertMaintainsOrder()
        {
            var buffer = new SingleStatisticsCollection(4);
            buffer.Insert(0, 5);
            Assert.AreEqual(5, buffer[0]);
            buffer.Insert(0, 7);
            Assert.AreEqual(7, buffer[0]);
            Assert.AreEqual(5, buffer[1]);
            buffer.Insert(1, 3);
            Assert.AreEqual(7, buffer[0]);
            Assert.AreEqual(3, buffer[1]);
            Assert.AreEqual(5, buffer[2]);
            buffer.Insert(1, 13);
            Assert.AreEqual(7, buffer[0]);
            Assert.AreEqual(13, buffer[1]);
            Assert.AreEqual(3, buffer[2]);
            Assert.AreEqual(5, buffer[3]);
            buffer.Insert(2, 2);
            Assert.AreEqual(13, buffer[0]);
            Assert.AreEqual(3, buffer[1]);
            Assert.AreEqual(2, buffer[2]);
            Assert.AreEqual(5, buffer[3]);
            buffer.Insert(0, 17);
            Assert.AreEqual(17, buffer[0]);
            Assert.AreEqual(3, buffer[1]);
            Assert.AreEqual(2, buffer[2]);
            Assert.AreEqual(5, buffer[3]);
        }
    }
}