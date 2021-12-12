using NUnit.Framework;

namespace AdventOfCode2021.Test
{
    [TestFixture]
    public class Day03Tests {
        private static readonly string[] _sampleLines = new[] {
            "00100",
            "11110",
            "10110",
            "10111",
            "10101",
            "01111",
            "00111",
            "11100",
            "10000",
            "11001",
            "00010",
            "01010",
        };

        [Test]
        public void TestBaseSample() {
            Assert.AreEqual(198, new Day03().ExecutePart1(_sampleLines));
        }

        [Test]
        public void TestParseValues()
        {
            CollectionAssert.AreEqual(new [] {4, 30}, Day03.ParseValues(new [] {"00100", "11110"}));
        }

        [Test]
        public void TestGetGammaRate()
        {
            Assert.AreEqual(22, Day03.GetGammaRate(Day03.ParseValues(_sampleLines), 5));
        }

        [Test]
        public void TestGetComplementary()
        {
            Assert.AreEqual(9, Day03.GetComplementary(22, 5));
        }

        [Test]
        public void TestAdvancedSample()
        {
            Assert.AreEqual((23, 10), Day03.GetOxygenAndCO2Ratings(_sampleLines));
        }
    }
}