using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;


using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System
{
    [TestClass]
    public class IEnumerableExtTests
    {
        [TestMethod]
        public void MoveNext_Empty()
        {
            var arr = new IEnumerator[0];
            Assert.IsFalse(arr.MoveNext());
        }

        [TestMethod]
        public void MoveNext_Null()
        {
            var arr = new IEnumerator[1];
            Assert.IsFalse(arr.MoveNext());
        }

        [TestMethod]
        public void MoveNext_EmptyArr()
        {
            var arr = new[] {
            new object[0].GetEnumerator()
        };
            Assert.IsFalse(arr.MoveNext());
        }

        [TestMethod]
        public void MoveNext_OneElementArr()
        {
            var arr = new[] {
            new []{ 2 }.GetEnumerator()
        };
            Assert.IsTrue(arr.MoveNext());
            Assert.IsFalse(arr.MoveNext());
        }

        [TestMethod]
        public void MoveNext_OneElementArr_Null()
        {
            var arr = new[] {
            new []{ 2 }.GetEnumerator(),
            null
        };
            Assert.IsTrue(arr.MoveNext());
            Assert.IsFalse(arr.MoveNext());
        }

        [TestMethod]
        public void MoveNext_Null_OneElementArr()
        {
            var arr = new[] {
            null,
            new []{ 2 }.GetEnumerator()
        };
            Assert.IsTrue(arr.MoveNext());
            Assert.IsFalse(arr.MoveNext());
        }

        [TestMethod]
        public void MoveNext_OneElementArr_Empty()
        {
            var arr = new[] {
            new []{ 2 }.GetEnumerator(),
            new object[0].GetEnumerator()
        };
            Assert.IsTrue(arr.MoveNext());
            Assert.IsFalse(arr.MoveNext());
        }

        [TestMethod]
        public void MoveNext_Empty_OneElementArr()
        {
            var arr = new[] {
            new object[0].GetEnumerator(),
            new []{ 2 }.GetEnumerator()
        };
            Assert.IsTrue(arr.MoveNext());
            Assert.IsFalse(arr.MoveNext());
        }

        [TestMethod]
        public void MoveNext_OneElementArr_OneElementArr()
        {
            var arr = new[] {
            new []{ 2 }.GetEnumerator(),
            new []{ 3 }.GetEnumerator()
        };
            Assert.IsTrue(arr.MoveNext());
            Assert.IsFalse(arr.MoveNext());
        }

        [TestMethod]
        public void MoveNext_TwoElementArr()
        {
            var arr = new[] {
            new []{ 2, 3 }.GetEnumerator()
        };
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(arr.MoveNext());
            Assert.IsFalse(arr.MoveNext());
        }

        [TestMethod]
        public void MoveNext_TwoElementArr_Null()
        {
            var arr = new[] {
            new []{ 2, 3 }.GetEnumerator(),
            null
        };
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(arr.MoveNext());
            Assert.IsFalse(arr.MoveNext());
        }

        [TestMethod]
        public void MoveNext_Null_TwoElementArr()
        {
            var arr = new[] {
            null,
            new []{ 2, 3 }.GetEnumerator()
        };
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(arr.MoveNext());
            Assert.IsFalse(arr.MoveNext());
        }

        [TestMethod]
        public void MoveNext_TwoElementArr_Empty()
        {
            var arr = new[] {
            new []{ 2, 3 }.GetEnumerator(),
            new object[0].GetEnumerator()
        };
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(arr.MoveNext());
            Assert.IsFalse(arr.MoveNext());
        }

        [TestMethod]
        public void MoveNext_Empty_TwoElementArr()
        {
            var arr = new[] {
            new object[0].GetEnumerator(),
            new []{ 2, 3 }.GetEnumerator()
        };
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(arr.MoveNext());
            Assert.IsFalse(arr.MoveNext());
        }

        [TestMethod]
        public void MoveNext_TwoElementArr_OneElementArr()
        {
            var arr = new[] {
            new []{ 2, 5 }.GetEnumerator(),
            new []{ 3 }.GetEnumerator()
        };
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(arr.MoveNext());
            Assert.IsFalse(arr.MoveNext());
        }

        [TestMethod]
        public void MoveNext_OneElementArr_TwoElementArr()
        {
            var arr = new[] {
            new []{ 3 }.GetEnumerator(),
            new []{ 2, 5 }.GetEnumerator()
        };
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(arr.MoveNext());
            Assert.IsFalse(arr.MoveNext());
        }

        [TestMethod]
        public void MoveNext_TwoElementArr_TwoElementArr()
        {
            var arr = new[] {
            new []{ 3, 7 }.GetEnumerator(),
            new []{ 2, 5 }.GetEnumerator()
        };
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(arr.MoveNext());
            Assert.IsFalse(arr.MoveNext());
        }

        [TestMethod]
        public void Current_Empty()
        {
            var arr = new IEnumerator[0];
            Assert.IsTrue(arr.Current().Empty());
        }

        [TestMethod]
        public void Current_Null()
        {
            var arr = new IEnumerator[1];
            Assert.IsTrue(arr.Current().Empty());
        }

        [TestMethod]
        public void Current_EmptyArr()
        {
            var arr = new[] {
            new object[0].GetEnumerator()
        };
            Assert.ThrowsException<InvalidOperationException>(() =>
                arr.Current().Empty());
        }

        [TestMethod]
        public void Current_OneElementArr_NoMove()
        {
            var arr = new[] {
            new []{ 2 }.GetEnumerator()
        };
            Assert.ThrowsException<InvalidOperationException>(() =>
                arr.Current().Empty());
        }

        [TestMethod]
        public void Current_OneElementArr_Move()
        {
            var elems = new[] { 2 };
            var arr = new[] {
            elems.GetEnumerator()
        };
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Matches(arr.Current()));
        }

        [TestMethod]
        public void Current_OneElementArr_Null()
        {
            var elems = new[] { 2 };
            var arr = new[] {
            elems.GetEnumerator(),
            null
        };
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Matches(arr.Current()));
        }

        [TestMethod]
        public void Current_Null_OneElementArr()
        {
            var elems = new[] { 2 };
            var arr = new[] {
            null,
            elems.GetEnumerator()
        };
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Matches(arr.Current()));
        }

        [TestMethod]
        public void Current_OneElementArr_Empty()
        {
            var elems = new[] { 2 };
            var arr = new[] {
            elems.GetEnumerator(),
            new object[0].GetEnumerator()
        };
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Matches(arr.Current()));
        }

        [TestMethod]
        public void Current_Empty_OneElementArr()
        {
            var elems = new[] { 2 };
            var arr = new[] {
            new object[0].GetEnumerator(),
            elems.GetEnumerator()
        };
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Matches(arr.Current()));
        }

        [TestMethod]
        public void Current_OneElementArr_OneElementArr()
        {
            var elems = new[] { 2, 3 };
            var arr = new[] {
            elems.Take(1).GetEnumerator(),
            elems.Skip(1).GetEnumerator()
        };
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Matches(arr.Current()));
        }

        [TestMethod]
        public void Current_TwoElementArr()
        {
            var elems = new[] { 2, 3 };
            var arr = new[] {
            elems.GetEnumerator()
        };
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Take(1).Matches(arr.Current()));
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Skip(1).Matches(arr.Current()));
        }

        [TestMethod]
        public void Current_TwoElementArr_Null()
        {
            var elems = new[] { 2, 3 };
            var arr = new[] {
            elems.GetEnumerator(),
            null
        };
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Take(1).Matches(arr.Current()));
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Skip(1).Matches(arr.Current()));
        }

        [TestMethod]
        public void Current_Null_TwoElementArr()
        {
            var elems = new[] { 2, 3 };
            var arr = new[] {
            null,
            elems.GetEnumerator()
        };
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Take(1).Matches(arr.Current()));
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Skip(1).Matches(arr.Current()));
        }

        [TestMethod]
        public void Current_TwoElementArr_Empty()
        {
            var elems = new[] { 2, 3 };
            var arr = new[] {
            new object[0].GetEnumerator(),
            elems.GetEnumerator()
        };
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Take(1).Matches(arr.Current()));
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Skip(1).Matches(arr.Current()));
        }

        [TestMethod]
        public void Current_Empty_TwoElementArr()
        {
            var elems = new[] { 2, 3 };
            var arr = new[] {
            elems.GetEnumerator(),
            new object[0].GetEnumerator()
        };
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Take(1).Matches(arr.Current()));
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Skip(1).Matches(arr.Current()));
        }

        [TestMethod]
        public void Current_TwoElementArr_OneElementArr()
        {
            var arr = new[] {
            new []{ 2, 5 }.GetEnumerator(),
            new []{ 3 }.GetEnumerator()
        };
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(new[] { 2, 3 }.Matches(arr.Current()));
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(new[] { 5 }.Matches(arr.Current()));
        }

        [TestMethod]
        public void Current_OneElementArr_TwoElementArr()
        {
            var arr = new[] {
            new []{ 3 }.GetEnumerator(),
            new []{ 2, 5 }.GetEnumerator()
        };
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(new[] { 3, 2 }.Matches(arr.Current()));
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(new[] { 5 }.Matches(arr.Current()));
        }

        [TestMethod]
        public void Current_TwoElementArr_TwoElementArr()
        {
            var arr = new[] {
            new []{ 3, 7 }.GetEnumerator(),
            new []{ 2, 5 }.GetEnumerator()
        };
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(new[] { 3, 2 }.Matches(arr.Current()));
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(new[] { 7, 5 }.Matches(arr.Current()));
        }

        [TestMethod]
        public void Interleave_Empty()
        {
            var arr = new InterleavedEnumerator();
            Assert.IsFalse(arr.MoveNext());
        }

        [TestMethod]
        public void Interleave_Null()
        {
            var arr = new InterleavedEnumerator(new IEnumerator[1]);
            Assert.IsFalse(arr.MoveNext());
        }

        [TestMethod]
        public void Interleave_EmptyArr()
        {
            var arr = new InterleavedEnumerator(
                new object[0].GetEnumerator()
            );
            Assert.IsFalse(arr.MoveNext());
        }

        [TestMethod]
        public void Interleave_OneElementArr_NoMove()
        {
            var arr = new InterleavedEnumerator(
                new[] { 2 }.GetEnumerator()
            );
            Assert.ThrowsException<InvalidOperationException>(() => arr.Current);
        }

        [TestMethod]
        public void Interleave_OneElementArr_Move()
        {
            var elems = new[] { 2 };
            var arr = new InterleavedEnumerator(
                elems.GetEnumerator()
            );
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Matches((IEnumerator)arr.Current));
        }

        [TestMethod]
        public void Interleave_OneElementArr_Null()
        {
            var elems = new[] { 2 };
            var arr = new InterleavedEnumerator(
                elems.GetEnumerator(),
                null
            );
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Matches((IEnumerator)arr.Current));
        }

        [TestMethod]
        public void Interleave_Null_OneElementArr()
        {
            var elems = new[] { 2 };
            var arr = new InterleavedEnumerator(
                null,
                elems.GetEnumerator()
            );
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Matches((IEnumerator)arr.Current));
        }

        [TestMethod]
        public void Interleave_OneElementArr_Empty()
        {
            var elems = new[] { 2 };
            var arr = new InterleavedEnumerator(
                elems.GetEnumerator(),
                new object[0].GetEnumerator()
            );
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Matches((IEnumerator)arr.Current));
        }

        [TestMethod]
        public void Interleave_Empty_OneElementArr()
        {
            var elems = new[] { 2 };
            var arr = new InterleavedEnumerator(
                new object[0].GetEnumerator(),
                elems.GetEnumerator()
            );
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Matches((IEnumerator)arr.Current));
        }

        [TestMethod]
        public void Interleave_OneElementArr_OneElementArr()
        {
            var elems = new[] { 2, 3 };
            var arr = new InterleavedEnumerator(
                elems.Take(1).GetEnumerator(),
                elems.Skip(1).GetEnumerator()
            );
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Matches((IEnumerator)arr.Current));
        }

        [TestMethod]
        public void Interleave_TwoElementArr()
        {
            var elems = new[] { 2, 3 };
            var arr = new InterleavedEnumerator(
                elems.GetEnumerator()
            );
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Take(1).Matches((IEnumerator)arr.Current));
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Skip(1).Matches((IEnumerator)arr.Current));
        }

        [TestMethod]
        public void Interleave_TwoElementArr_Null()
        {
            var elems = new[] { 2, 3 };
            var arr = new InterleavedEnumerator(
                elems.GetEnumerator(),
                null
            );
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Take(1).Matches((IEnumerator)arr.Current));
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Skip(1).Matches((IEnumerator)arr.Current));
        }

        [TestMethod]
        public void Interleave_Null_TwoElementArr()
        {
            var elems = new[] { 2, 3 };
            var arr = new InterleavedEnumerator(
                null,
                elems.GetEnumerator()
            );
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Take(1).Matches((IEnumerator)arr.Current));
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Skip(1).Matches((IEnumerator)arr.Current));
        }

        [TestMethod]
        public void Interleave_TwoElementArr_Empty()
        {
            var elems = new[] { 2, 3 };
            var arr = new InterleavedEnumerator(
                new object[0].GetEnumerator(),
                elems.GetEnumerator()
            );
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Take(1).Matches((IEnumerator)arr.Current));
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Skip(1).Matches((IEnumerator)arr.Current));
        }

        [TestMethod]
        public void Interleave_Empty_TwoElementArr()
        {
            var elems = new[] { 2, 3 };
            var arr = new InterleavedEnumerator(
                elems.GetEnumerator(),
                new object[0].GetEnumerator()
            );
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Take(1).Matches((IEnumerator)arr.Current));
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(elems.Skip(1).Matches((IEnumerator)arr.Current));
        }

        [TestMethod]
        public void Interleave_TwoElementArr_OneElementArr()
        {
            var arr = new InterleavedEnumerator(
                new[] { 2, 5 }.GetEnumerator(),
                new[] { 3 }.GetEnumerator()
            );
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(new[] { 2, 3 }.Matches((IEnumerator)arr.Current));
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(new[] { 5 }.Matches((IEnumerator)arr.Current));
        }

        [TestMethod]
        public void Interleave_OneElementArr_TwoElementArr()
        {
            var arr = new InterleavedEnumerator(
                new[] { 3 }.GetEnumerator(),
                new[] { 2, 5 }.GetEnumerator()
            );
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(new[] { 3, 2 }.Matches((IEnumerator)arr.Current));
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(new[] { 5 }.Matches((IEnumerator)arr.Current));
        }

        [TestMethod]
        public void Interleave_TwoElementArr_TwoElementArr()
        {
            var arr = new InterleavedEnumerator(
                new[] { 3, 7 }.GetEnumerator(),
                new[] { 2, 5 }.GetEnumerator()
            );
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(new[] { 3, 2 }.Matches((IEnumerator)arr.Current));
            Assert.IsTrue(arr.MoveNext());
            Assert.IsTrue(new[] { 7, 5 }.Matches((IEnumerator)arr.Current));
        }
    }
}

