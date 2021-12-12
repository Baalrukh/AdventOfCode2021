using NUnit.Framework;

namespace AdventOfCode2021.Test {
    [TestFixture]
    public class Day07Tests {
        private static readonly string[] _sampleLines = new[] {
            "16,1,2,0,4,2,7,1,2,14"
        };

        [Test]
        public void TestPart1() {
            Assert.AreEqual(37, new Day07().ExecutePart1(_sampleLines));
        }

        [Test]
        public void TestPart2() {
            Assert.AreEqual(168, new Day07().ExecutePart2(_sampleLines));
        }
    }
}
