using NUnit.Framework;

namespace AdventOfCode2021.Test {
    [TestFixture]
    public class Day23Tests {
        private static readonly string[] _sampleLines = new[] {
            "#############",
            "#...........#",
            "###B#C#B#D###",
            "  #A#D#C#A#",
            "  #########",
        };

        [Test]
        public void TestPart1() {
            Assert.AreEqual(12521, new Day23().ExecutePart1(_sampleLines));
        }

        [Test]
        public void TestPart2() {
            Assert.AreEqual(-20, new Day23().ExecutePart2(_sampleLines));
        }
    }
}
