using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Tests
{
    [TestClass]
    public class SigFigsTests
    {
        [TestMethod]
        public void ThrowsOnNegativeDigits()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => 5f.SigFig(-1));
        }

        [TestMethod]
        public void ThrowsOnZeroDigits()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => 5f.SigFig(0));
        }

        [TestMethod]
        public void Zero1()
        {
            Test(0f, 1, "0");
        }

        [TestMethod]
        public void Zero2()
        {
            Test(0f, 2, "0");
        }

        [TestMethod]
        public void Zero3()
        {
            Test(0f, 3, "0");
        }

        [TestMethod]
        public void NegZero1()
        {
            Test(-0f, 1, "0");
        }

        [TestMethod]
        public void NegZero2()
        {
            Test(-0f, 2, "0");
        }

        [TestMethod]
        public void NegZero3()
        {
            Test(-0f, 3, "0");
        }

        [TestMethod]
        public void One1()
        {
            Test(1f, 1, "1");
        }

        [TestMethod]
        public void One2()
        {
            Test(1f, 2, "1.0");
        }

        [TestMethod]
        public void One3()
        {
            Test(1f, 3, "1.00");
        }

        [TestMethod]
        public void NegOne1()
        {
            Test(-1f, 1, "-1");
        }

        [TestMethod]
        public void NegOne2()
        {
            Test(-1f, 2, "-1.0");
        }

        [TestMethod]
        public void NegOne3()
        {
            Test(-1f, 3, "-1.00");
        }

        [TestMethod]
        public void ExtendSmall()
        {
            Test(1.2f, 4, "1.200");
        }

        [TestMethod]
        public void TruncateSmall()
        {
            Test(1.234f, 2, "1.2");
        }

        [TestMethod]
        public void TruncateSmallAndRound()
        {
            Test(4.567f, 2, "4.6");
        }

        [TestMethod]
        public void TruncateLarge()
        {
            Test(1234, 2, "1200");
        }

        [TestMethod]
        public void TruncateLargeAndRound()
        {
            Test(4567, 2, "4600");
        }

        [TestMethod]
        public void NegExtendSmall()
        {
            Test(-1.2f, 4, "-1.200");
        }

        [TestMethod]
        public void NegTruncateSmall()
        {
            Test(-1.234f, 2, "-1.2");
        }

        [TestMethod]
        public void NegTruncateSmallAndRound()
        {
            Test(-4.567f, 2, "-4.6");
        }

        [TestMethod]
        public void NegTruncateLarge()
        {
            Test(-1234, 2, "-1200");
        }

        [TestMethod]
        public void NegTruncateLargeAndRound()
        {
            Test(-4567, 2, "-4600");
        }

        [TestMethod]
        public void NaN1()
        {
            Test(float.NaN, 1, "NaN");
        }

        [TestMethod]
        public void NaN2()
        {
            Test(float.NaN, 2, "NaN");
        }

        [TestMethod]
        public void NaN3()
        {
            Test(float.NaN, 3, "NaN");
        }

        [TestMethod]
        public void Infinity1()
        {
            Test(float.PositiveInfinity, 1, "∞");
        }

        [TestMethod]
        public void Infinity2()
        {
            Test(float.PositiveInfinity, 2, "∞");
        }

        [TestMethod]
        public void Infinity3()
        {
            Test(float.PositiveInfinity, 3, "∞");
        }

        [TestMethod]
        public void NegInfinity1()
        {
            Test(float.NegativeInfinity, 1, "-∞");
        }

        [TestMethod]
        public void NegInfinity2()
        {
            Test(float.NegativeInfinity, 2, "-∞");
        }

        [TestMethod]
        public void NegInfinity3()
        {
            Test(float.NegativeInfinity, 3, "-∞");
        }

        private void Test(float value, int digits, string output)
        {
            Assert.AreEqual(output, value.SigFig(digits));
        }
    }
}
