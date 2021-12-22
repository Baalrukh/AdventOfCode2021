using NUnit.Framework;

namespace AdventOfCode2021.Test {
    [TestFixture]
    public class Day21Tests {
        private static readonly string[] _sampleLines = new[] {
            "Player 1 starting position: 4",
            "Player 2 starting position: 8"
        };

        [Test]
        public void TestPart1() {
            Assert.AreEqual(739785, new Day21().ExecutePart1(_sampleLines));
        }

        [Test]
        public void TestPart2() {
            Assert.AreEqual(444356092776315L, new Day21().ExecutePart2(_sampleLines));
        }
    }
}
