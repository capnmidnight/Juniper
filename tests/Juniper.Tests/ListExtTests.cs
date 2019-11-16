using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Collections.Tests
{
    /// <summary>
    /// These tests are taken from the Examples section of <see cref="https://developer.mozilla.org/en-US/docs/web/javascript/reference/global_objects/array/Splice"/>
    /// </summary>
    [TestClass]
    public class ListExtTests
    {
        [TestMethod]
        public void Splice1()
        {
            // Remove 0(zero) elements from index 2, and insert "drum"
            var actualList = new List<string> { "angel", "clown", "mandarin", "sturgeon" };
            var actualRemoved = actualList.Splice(2, 0, "drum");
            var expectedList = new[] { "angel", "clown", "drum", "mandarin", "sturgeon" };
            var expectedRemoved = Array.Empty<string>();

            Assert.IsTrue(expectedList.Matches(actualList));
            Assert.IsTrue(actualRemoved.Matches(expectedRemoved));
        }

        [TestMethod]
        public void Splice2()
        {
            // Remove 0(zero) elements from index 2, and insert "drum" and "guitar"
            var actualList = new List<string> { "angel", "clown", "mandarin", "sturgeon" };
            var actualRemoved = actualList.Splice(2, 0, "drum", "guitar");
            var expectedList = new[] { "angel", "clown", "drum", "guitar", "mandarin", "sturgeon" };
            var expectedRemoved = Array.Empty<string>();

            Assert.IsTrue(expectedList.Matches(actualList));
            Assert.IsTrue(actualRemoved.Matches(expectedRemoved));
        }

        [TestMethod]
        public void Splice3()
        {
            // Remove 1 element from index 3
            var actualList = new List<string> { "angel", "clown", "drum", "mandarin", "sturgeon" };
            var actualRemoved = actualList.Splice(3, 1);
            var expectedList = new[] { "angel", "clown", "drum", "sturgeon" };
            var expectedRemoved = new[] { "mandarin" };

            Assert.IsTrue(actualList.Matches(expectedList));
            Assert.IsTrue(actualRemoved.Matches(expectedRemoved));
        }

        [TestMethod]
        public void Splice4()
        {
            // Remove 1 element from index 2, and insert "trumpet"
            var actualList = new List<string> { "angel", "clown", "drum", "sturgeon" };
            var actualRemoved = actualList.Splice(2, 1, "trumpet");
            var expectedList = new[] { "angel", "clown", "trumpet", "sturgeon" };
            var expectedRemoved = new[] { "drum" };

            Assert.IsTrue(actualList.Matches(expectedList));
            Assert.IsTrue(actualRemoved.Matches(expectedRemoved));
        }

        [TestMethod]
        public void Splice5()
        {
            // Remove 2 elements from index 0, and insert "parrot", "anemone" and "blue"
            var actualList = new List<string> { "angel", "clown", "trumpet", "sturgeon" };
            var actualRemoved = actualList.Splice(0, 2, "parrot", "anemone", "blue");
            var expectedList = new[] { "parrot", "anemone", "blue", "trumpet", "sturgeon" };
            var expectedRemoved = new[] { "angel", "clown" };

            Assert.IsTrue(actualList.Matches(expectedList));
            Assert.IsTrue(actualRemoved.Matches(expectedRemoved));
        }

        [TestMethod]
        public void Splice6()
        {
            // Remove 2 elements from index 2
            var actualList = new List<string> { "parrot", "anemone", "blue", "trumpet", "sturgeon" };
            var actualRemoved = actualList.Splice(2, 2);
            var expectedList = new[] { "parrot", "anemone", "sturgeon" };
            var expectedRemoved = new[] { "blue", "trumpet" };

            Assert.IsTrue(actualList.Matches(expectedList));
            Assert.IsTrue(actualRemoved.Matches(expectedRemoved));
        }

        [TestMethod]
        public void Splice7()
        {
            // Remove 1 element from index - 2
            var actualList = new List<string> { "angel", "clown", "mandarin", "sturgeon" };
            var actualRemoved = actualList.Splice(-2, 1);
            var expectedList = new[] { "angel", "clown", "sturgeon" };
            var expectedRemoved = new[] { "mandarin" };

            Assert.IsTrue(actualList.Matches(expectedList));
            Assert.IsTrue(actualRemoved.Matches(expectedRemoved));
        }

        [TestMethod]
        public void Splice8()
        {
            // Remove all elements after index 2(incl.)
            var actualList = new List<string> { "angel", "clown", "mandarin", "sturgeon" };
            var actualRemoved = actualList.Splice(2);
            var expectedList = new[] { "angel", "clown" };
            var expectedRemoved = new[] { "mandarin", "sturgeon" };

            Assert.IsTrue(actualList.Matches(expectedList));
            Assert.IsTrue(actualRemoved.Matches(expectedRemoved));
        }
    }
}