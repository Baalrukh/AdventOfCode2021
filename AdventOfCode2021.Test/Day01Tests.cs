using NUnit.Framework;

namespace AdventOfCode2021.Test
{
    [TestFixture]
    public class Day01Tests
    {
         private static readonly string[] _sampleLines = new[]
            {
                "199",
                "200",
                "208",
                "210",
                "200",
                "207",
                "240",
                "269",
                "260",
                "263",
            };

        [Test]
        public void TestBaseSample()
        {
            Assert.AreEqual(7, new Day01().ExecutePart1(_sampleLines));
        }

        [Test]
        public void TestAdvancedSample()
        {
            Assert.AreEqual(5, new Day01().ExecutePart2(_sampleLines));
        }
    }
}