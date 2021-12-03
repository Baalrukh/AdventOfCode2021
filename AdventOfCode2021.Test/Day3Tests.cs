using AdventOfCode2021.Utils;
using NUnit.Framework;

namespace AdventOfCode2021.Test
{
    [TestFixture]
    public class Day3Tests {
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
            Assert.AreEqual(198, new Day3().Execute(_sampleLines));
        }

        [Test]
        public void TestGetGammaRate()
        {
            Assert.AreEqual(22, Day3.GetGammaRate(_sampleLines));
        }

        [Test]
        public void TestGetComplementary()
        {
            Assert.AreEqual(9, Day3.GetComplementary(22, 5));
        }

        [Test]
        public void TestAdvancedSample()
        {
            Assert.AreEqual(new IntVector2(15, 60), Day2.GetFinalPositionAdvanced(_sampleLines));
        }
    }
}