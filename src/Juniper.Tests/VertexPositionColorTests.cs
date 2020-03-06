using System.Collections.Generic;
using System.Numerics;

using NUnit.Framework;

using Veldrid;

namespace Juniper.VeldridIntegration.Tests
{
    [TestFixture]
    public class VertexPositionColorTests
    {
        [Test]
        public void ListIndexOf()
        {
            var a = new VertexPositionColor(new Vector3(-0.5f, 0.5f, 0), RgbaFloat.Red);
            var b = new VertexPositionColor(new Vector3(-0.5f, 0.5f, 0), RgbaFloat.Red);

            var l = new List<VertexPositionColor>
            {
                a
            };

            Assert.AreEqual(0, l.IndexOf(b));
        }
    }
}
