using NUnit.Framework;

namespace AdventOfCode2021.Test {
    [TestFixture]
    public class Day15Tests {
        private static readonly string[] _sampleLines = new[] {
            "1163751742",
            "1381373672",
            "2136511328",
            "3694931569",
            "7463417111",
            "1319128137",
            "1359912421",
            "3125421639",
            "1293138521",
            "2311944581",
        };

        [Test]
        public void TestPart1() {
            Assert.AreEqual(40, new Day15().ExecutePart1(_sampleLines));
        }

        [Test]
        public void TestPart2() {
            Assert.AreEqual(315, new Day15().ExecutePart2(_sampleLines));
        }
    }
}
