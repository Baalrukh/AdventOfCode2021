using AdventOfCode2021.Utils;
using NUnit.Framework;

namespace AdventOfCode2021.Test {
    [TestFixture]
    public class Day09Tests {
        private static readonly string[] _sampleLines = new[] {
            "2199943210",
            "3987894921",
            "9856789892",
            "8767896789",
            "9899965678"
        };

        [Test]
        public void TestPart1() {
            Assert.AreEqual(15, new Day09().ExecutePart1(_sampleLines));
        }

        [Test]
        public void TestComputeBasinSize()
        {
            var map = Day09.Map.Parse(_sampleLines);
            var basin = new Day09.Basin(new IntVector2(1, 0));
            Assert.AreEqual(3, basin.ComputeSize(map));
        }

        [Test]
        public void TestPart2() {
            Assert.AreEqual(1134, new Day09().ExecutePart2(_sampleLines));
        }
    }
}
