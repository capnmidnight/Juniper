using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.HTTP.Server.Tests
{
    [TestClass]
    public class CIDRBlockTests
    {
        private const string testAddressString0 = "192.160.0.0";
        private static readonly IPAddress testAddress0 = IPAddress.Parse(testAddressString0);

        private const string testAddressString1 = "192.160.0.1";
        private static readonly IPAddress testAddress1 = IPAddress.Parse(testAddressString1);
        private const int testBitMask1 = 32;
        private static readonly string testBlock1 = testAddressString1 + "/" + testBitMask1;

        private const string testAddressString2 = "192.160.0.2";
        private static readonly IPAddress testAddress2 = IPAddress.Parse(testAddressString2);

        private const string testAddressString3 = "192.160.0.3";
        private static readonly IPAddress testAddress3 = IPAddress.Parse(testAddressString3);

        [TestMethod]
        public void ParseSuccessfulFullBlock()
        {
            var block = CIDRBlock.Parse(testBlock1);
            Assert.AreEqual(testBlock1, block.ToString());
            Assert.AreEqual(block.Start.ToString(), testAddressString1);
            Assert.AreEqual(block.BitmaskLength, testBitMask1);
        }

        [TestMethod]
        public void ParseSuccessfulAddressOnly()
        {
            var block = CIDRBlock.Parse(testAddressString1);
            Assert.AreEqual(testBlock1, block.ToString());
            Assert.AreEqual(block.Start.ToString(), testAddressString1);
            Assert.AreEqual(block.BitmaskLength, testBitMask1);
        }

        [TestMethod]
        public void ParseFailOnNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => CIDRBlock.Parse(null));
        }

        [TestMethod]
        public void ParseFailEmpty()
        {
            Assert.ThrowsException<FormatException>(() => CIDRBlock.Parse(string.Empty));
        }

        [TestMethod]
        public void ParseFailMultipleSlashes()
        {
            Assert.ThrowsException<FormatException>(() => CIDRBlock.Parse(testBlock1 + "/32"));
        }

        [TestMethod]
        public void ParseFailNoAddress()
        {
            Assert.ThrowsException<FormatException>(() => CIDRBlock.Parse("/32"));
        }

        [TestMethod]
        public void ParseFailJunkAddress()
        {
            Assert.ThrowsException<FormatException>(() => CIDRBlock.Parse("shoobidywhoobidy/32"));
        }

        [TestMethod]
        public void ParseFailNoBitmaskLength()
        {
            Assert.ThrowsException<FormatException>(() => CIDRBlock.Parse("shoobidywhoobidy/"));
        }

        [TestMethod]
        public void ParseFailJunkBitmaskLength()
        {
            Assert.ThrowsException<FormatException>(() => CIDRBlock.Parse("shoobidy/whoobidy"));
        }

        [TestMethod]
        public void TryParseSuccessfulFullBlock()
        {
            Assert.IsTrue(CIDRBlock.TryParse(testBlock1, out var block));
            Assert.AreEqual(testBlock1, block.ToString());
            Assert.AreEqual(block.Start.ToString(), testAddressString1);
            Assert.AreEqual(block.BitmaskLength, testBitMask1);
        }

        [TestMethod]
        public void TryParseSuccessfulAddressOnly()
        {
            Assert.IsTrue(CIDRBlock.TryParse(testAddressString1, out var block));
            Assert.AreEqual(testBlock1, block.ToString());
            Assert.AreEqual(block.Start.ToString(), testAddressString1);
            Assert.AreEqual(block.BitmaskLength, testBitMask1);
        }

        [TestMethod]
        public void TryParseFailOnNull()
        {
            Assert.IsFalse(CIDRBlock.TryParse(null, out _));
        }

        [TestMethod]
        public void TryParseFailEmpty()
        {
            Assert.IsFalse(CIDRBlock.TryParse(string.Empty, out _));
        }

        [TestMethod]
        public void TryParseFailMultipleSlashes()
        {
            Assert.IsFalse(CIDRBlock.TryParse(testBlock1 + "/32", out _));
        }

        [TestMethod]
        public void TryParseFailNoAddress()
        {
            Assert.IsFalse(CIDRBlock.TryParse("/32", out _));
        }

        [TestMethod]
        public void TryParseFailJunkAddress()
        {
            Assert.IsFalse(CIDRBlock.TryParse("shoobidywhoobidy/32", out _));
        }

        [TestMethod]
        public void TryParseFailNoBitmaskLength()
        {
            Assert.IsFalse(CIDRBlock.TryParse("shoobidywhoobidy/", out _));
        }

        [TestMethod]
        public void TryParseFailJunkBitmaskLength()
        {
            Assert.IsFalse(CIDRBlock.TryParse("shoobidy/whoobidy", out _));
        }

        [TestMethod]
        public void CompareAddressLessThan()
        {
            Assert.IsTrue(CIDRBlock.CompareAddresses(testAddress1, testAddress2) < 0);
        }

        [TestMethod]
        public void CompareAddressEqual()
        {
            Assert.IsTrue(CIDRBlock.CompareAddresses(testAddress1, testAddress1) == 0);
        }

        [TestMethod]
        public void CompareAddressGreaterThan()
        {
            Assert.IsTrue(CIDRBlock.CompareAddresses(testAddress2, testAddress1) > 0);
        }

        [TestMethod]
        public void SortAddresses()
        {
            var addresses = new List<IPAddress>
            {
                testAddress2,
                testAddress1
            };

            addresses.Sort(CIDRBlock.CompareAddresses);

            Assert.AreEqual(testAddress1, addresses[0]);
            Assert.AreEqual(testAddress2, addresses[1]);
        }

        [TestMethod]
        public void CombineBlocks()
        {
            var block1 = new CIDRBlock(testAddress1);
            var block2 = new CIDRBlock(testAddress2);
            var block3 = block1 + block2;
            Assert.AreEqual(block1.Start, block3.Start);
            Assert.AreEqual(block2.End, block3.End);
            Assert.IsTrue(block3.Overlaps(block1));
            Assert.IsTrue(block3.Overlaps(block3));
            Assert.AreEqual(2, block3.Count);
            Assert.AreEqual(30, block3.BitmaskLength);
        }

        [TestMethod]
        public void InputEndMatchesOutputEnd1()
        {
            var block1 = new CIDRBlock(testAddress0, testAddress1);
            Assert.AreEqual(2, block1.Count);
            Assert.AreEqual(testAddress0, block1.Start);
            Assert.AreEqual(testAddress1, block1.End);

            var block2 = new CIDRBlock(block1.Start, block1.BitmaskLength);
            Assert.AreEqual(2, block2.Count);
            Assert.AreEqual(testAddress0, block2.Start);
            Assert.AreEqual(testAddress1, block2.End);
        }

        [TestMethod]
        public void InputEndMatchesOutputEnd2()
        {
            var block1 = new CIDRBlock(testAddress0, testAddress2);
            Assert.AreEqual(3, block1.Count);
            Assert.AreEqual(testAddress0, block1.Start);
            Assert.AreEqual(testAddress2, block1.End);

            var block2 = new CIDRBlock(block1.Start, block1.BitmaskLength);
            Assert.AreEqual(4, block2.Count);
            Assert.AreEqual(testAddress0, block2.Start);
            Assert.AreEqual(testAddress3, block2.End);
        }

        [TestMethod]
        public void Count1()
        {
            var block = new CIDRBlock(testAddress1, 32);
            Assert.AreEqual(1, block.Count);
        }

        [TestMethod]
        public void Count2()
        {
            var block = new CIDRBlock(testAddress0, 31);
            Assert.AreEqual(2, block.Count);
        }

        [TestMethod]
        public void Count3()
        {
            var block = new CIDRBlock(testAddress1, 31);
            Assert.AreEqual(block.Start, block.End);
            Assert.AreEqual(1, block.Count);
        }

        [TestMethod]
        public void DistanceTo1()
        {
            var block1 = new CIDRBlock(testAddress1);
            var block2 = new CIDRBlock(testAddress2);
            var distance = block1.DistanceTo(block2);
            Assert.AreEqual(1f, distance, 0.00001f);
        }

        [TestMethod]
        public void ListAddresses()
        {
            var block = new CIDRBlock(testAddress0, 31);
            var addresses = block.ToArray();
            Assert.AreEqual(2, addresses.Length);
            Assert.AreEqual(testAddress0, addresses[0]);
            Assert.AreEqual(testAddress1, addresses[1]);
        }
    }
}
