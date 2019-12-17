using System;
using System.Collections;
using System.Diagnostics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Iteration.Tests
{
    [TestClass]
    public class IterationTests : IComparer
    {
        private const int items = 1000000;
        private const int iters = 100;

        private interface I
        {
            void Update();
        }

        private struct A : I
        {
            private readonly float x;
            private readonly float y;

            public float Result;

            public A(float x, float y)
            {
                this.x = x;
                this.y = y;
                Result = 0;
            }

            public void Update()
            {
                Result += x + y;
            }
        }

        private struct B : I
        {
            private readonly float x;
            private readonly float y;

            public float Result;

            public B(float x, float y)
            {
                this.x = x;
                this.y = y;
                Result = 0;
            }

            public void Update()
            {
                Result += x - y;
            }
        }

        private struct C : I
        {
            private readonly float x;
            private readonly float y;

            public float Result;

            public C(float x, float y)
            {
                this.x = x;
                this.y = y;
                Result = 0;
            }

            public void Update()
            {
                Result += x * y;
            }
        }

        private struct D : I
        {
            private readonly float x;
            private readonly float y;

            public float Result;

            public D(float x, float y)
            {
                this.x = x;
                this.y = y;
                Result = 0;
            }

            public void Update()
            {
                Result += x / y;
            }
        }

        [TestMethod]
        public void Time()
        {
            var rand = new Random();
            var itemsRaw = new I[items];
            for (int i = 0; i < items; ++i)
            {
                switch (rand.Next(4))
                {
                    case 0:
                        itemsRaw[i] = (new A((float)rand.NextDouble(), (float)rand.NextDouble()));
                        break;
                    case 1:
                        itemsRaw[i] = (new B((float)rand.NextDouble(), (float)rand.NextDouble()));
                        break;
                    case 2:
                        itemsRaw[i] = (new C((float)rand.NextDouble(), (float)rand.NextDouble()));
                        break;
                    case 3:
                        itemsRaw[i] = (new D((float)rand.NextDouble(), (float)rand.NextDouble()));
                        break;
                }
            }

            var itemsByType = new I[items];
            Array.Copy(itemsRaw, itemsByType, items);
            Array.Sort(itemsByType, this);

            var timeRaw = TimeSpan.Zero;
            var timeByType = TimeSpan.Zero;
            var timer = new Stopwatch();

            for (int i = 0; i < iters; ++i)
            {
                timeRaw += Test(timer, itemsRaw);
            }

            for (int i = 0; i < iters; ++i)
            {
                timeByType += Test(timer, itemsByType);
            }

            var p = (timeByType.TotalMilliseconds - timeRaw.TotalMilliseconds) / timeRaw.TotalMilliseconds;
            Assert.IsTrue(timeByType < timeRaw, $"{timeByType.TotalMilliseconds} >= {timeRaw.TotalMilliseconds} (+{p * 100}%)");
        }

        private TimeSpan Test(Stopwatch timer, I[] items)
        {
            timer.Restart();
            for (int i = 0; i < items.Length; ++i)
            {
                items[i].Update();
            }

            return timer.Elapsed;
        }

        public int Compare(object x, object y)
        {
            return x.GetType().Name.CompareTo(y.GetType().Name);
        }
    }
}
