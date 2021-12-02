using NUnit.Framework;

namespace AdventOfCode2021.Test
{
    [TestFixture]
    public class Day2Tests
    {
        private static readonly string[] _sampleLines = new[]
        {
            "forward 5",
            "down 5",
            "forward 8",
            "up 3",
            "down 8",
            "forward 2",
        };

        [Test]
        public void TestBaseSample()
        {
            Assert.AreEqual(new IntVector2(15, 10), Day2.GetFinalPosition(_sampleLines));
        }

        [Test]
        public void TestAdvancedSample()
        {
            Assert.AreEqual(new IntVector2(15, 60), Day2.GetFinalPositionAdvanced(_sampleLines));
        }
    }
}