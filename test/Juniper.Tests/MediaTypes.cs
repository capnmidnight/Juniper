using NUnit.Framework;

namespace Juniper
{
    [TestFixture]
    public class MediaTypeTests
    {
        [Test]
        public void Parse1()
        {
            var input = "audio/webm";
            var output = MediaType.Parse(input);
            Assert.AreSame(MediaType.Audio_Webm, output);
        }

        [Test]
        public void MatchAny()
        {
            var input = "audio/webm";
            var output = MediaType.Audio.AnyAudio.Matches(input);
            Assert.IsTrue(output);
        }
    }
}