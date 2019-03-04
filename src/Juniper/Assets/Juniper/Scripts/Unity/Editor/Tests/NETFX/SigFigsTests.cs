using NUnit.Framework;

namespace System
{
    public class SigFigsTests
    {
        [Test]
        public void ThrowsOnNegativeDigits()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 5f.SigFig(-1));
        }

        [Test]
        public void ThrowsOnZeroDigits()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 5f.SigFig(0));
        }

        [Test]
        public void Zero1()
        {
            Test(0f, 1, "0");
        }

        [Test]
        public void Zero2()
        {
            Test(0f, 2, "0");
        }

        [Test]
        public void Zero3()
        {
            Test(0f, 3, "0");
        }

        [Test]
        public void NegZero1()
        {
            Test(-0f, 1, "0");
        }

        [Test]
        public void NegZero2()
        {
            Test(-0f, 2, "0");
        }

        [Test]
        public void NegZero3()
        {
            Test(-0f, 3, "0");
        }

        [Test]
        public void One1()
        {
            Test(1f, 1, "1");
        }

        [Test]
        public void One2()
        {
            Test(1f, 2, "1.0");
        }

        [Test]
        public void One3()
        {
            Test(1f, 3, "1.00");
        }

        [Test]
        public void NegOne1()
        {
            Test(-1f, 1, "-1");
        }

        [Test]
        public void NegOne2()
        {
            Test(-1f, 2, "-1.0");
        }

        [Test]
        public void NegOne3()
        {
            Test(-1f, 3, "-1.00");
        }

        [Test]
        public void ExtendSmall()
        {
            Test(1.2f, 4, "1.200");
        }

        [Test]
        public void TruncateSmall()
        {
            Test(1.234f, 2, "1.2");
        }

        [Test]
        public void TruncateSmallAndRound()
        {
            Test(4.567f, 2, "4.6");
        }

        [Test]
        public void TruncateLarge()
        {
            Test(1234, 2, "1200");
        }

        [Test]
        public void TruncateLargeAndRound()
        {
            Test(4567, 2, "4600");
        }

        [Test]
        public void NegExtendSmall()
        {
            Test(-1.2f, 4, "-1.200");
        }

        [Test]
        public void NegTruncateSmall()
        {
            Test(-1.234f, 2, "-1.2");
        }

        [Test]
        public void NegTruncateSmallAndRound()
        {
            Test(-4.567f, 2, "-4.6");
        }

        [Test]
        public void NegTruncateLarge()
        {
            Test(-1234, 2, "-1200");
        }

        [Test]
        public void NegTruncateLargeAndRound()
        {
            Test(-4567, 2, "-4600");
        }

        [Test]
        public void NaN1()
        {
            Test(float.NaN, 1, "NaN");
        }

        [Test]
        public void NaN2()
        {
            Test(float.NaN, 2, "NaN");
        }

        [Test]
        public void NaN3()
        {
            Test(float.NaN, 3, "NaN");
        }

        [Test]
        public void Infinity1()
        {
            Test(float.PositiveInfinity, 1, "∞");
        }

        [Test]
        public void Infinity2()
        {
            Test(float.PositiveInfinity, 2, "∞");
        }

        [Test]
        public void Infinity3()
        {
            Test(float.PositiveInfinity, 3, "∞");
        }

        [Test]
        public void NegInfinity1()
        {
            Test(float.NegativeInfinity, 1, "-∞");
        }

        [Test]
        public void NegInfinity2()
        {
            Test(float.NegativeInfinity, 2, "-∞");
        }

        [Test]
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
