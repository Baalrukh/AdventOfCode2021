using NUnit.Framework;

namespace AdventOfCode2021.Test {
    [TestFixture]
    public class Day9Tests {
        private static readonly string[] _sampleLines = new[] {
            "2199943210",
            "3987894921",
            "9856789892",
            "8767896789",
            "9899965678"
        };

        [Test]
        public void TestPart1() {
            Assert.AreEqual(15, new Day9().ExecutePart1(_sampleLines));
        }

        [Test]
        public void TestPart2() {
            Assert.AreEqual(-20, new Day9().ExecutePart2(_sampleLines));
        }
    }
}
