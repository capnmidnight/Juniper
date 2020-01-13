using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;

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

            public float Result { get; set; }

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

            public float Result { get; set; }

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

            public float Result { get; set; }

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

            public float Result { get; set; }

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
            for (var i = 0; i < items; ++i)
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

            for (var i = 0; i < iters; ++i)
            {
                timeRaw += Test(timer, itemsRaw);
            }

            for (var i = 0; i < iters; ++i)
            {
                timeByType += Test(timer, itemsByType);
            }

            var p = (timeByType.TotalMilliseconds - timeRaw.TotalMilliseconds) / timeRaw.TotalMilliseconds;
            Assert.IsTrue(timeByType < timeRaw, $"{timeByType.TotalMilliseconds.ToString(CultureInfo.CurrentCulture)} >= {timeRaw.TotalMilliseconds.ToString(CultureInfo.CurrentCulture)} (+{Units.Converter.Label((float)p, Units.UnitOfMeasure.Proportion, Units.UnitOfMeasure.Percent)})");
        }

        private TimeSpan Test(Stopwatch timer, I[] items)
        {
            timer.Restart();
            for (var i = 0; i < items.Length; ++i)
            {
                items[i].Update();
            }

            return timer.Elapsed;
        }

        public int Compare(object x, object y)
        {
            return string.CompareOrdinal(
                x?.GetType().Name,
                y?.GetType().Name);
        }
    }
}
