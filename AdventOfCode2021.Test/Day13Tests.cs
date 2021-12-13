using NUnit.Framework;

namespace AdventOfCode2021.Test {
    [TestFixture]
    public class Day13Tests
    {
        private static readonly string[] _sampleLines = new[]
        {
            "6,10",
            "0,14",
            "9,10",
            "0,3",
            "10,4",
            "4,11",
            "6,0",
            "6,12",
            "4,1",
            "0,13",
            "10,12",
            "3,4",
            "3,0",
            "8,4",
            "1,10",
            "2,14",
            "8,10",
            "9,0",
            "",
            "fold along y=7",
            "fold along x=5"
        };

        [Test]
        public void TestPart1() {
            Assert.AreEqual(17, new Day13().ExecutePart1(_sampleLines));
        }

        [Test]
        public void TestPart2() {
            Assert.AreEqual(16, new Day13().ExecutePart2(_sampleLines));
        }
    }
}
