using NUnit.Framework;

namespace AdventOfCode2021.Test {
    [TestFixture]
    public class Day14Tests {
        private static readonly string[] _sampleLines = new[] {
            "NNCB",
            "",
            "CH -> B",
            "HH -> N",
            "CB -> H",
            "NH -> C",
            "HB -> C",
            "HC -> B",
            "HN -> C",
            "NN -> C",
            "BH -> H",
            "NC -> B",
            "NB -> B",
            "BN -> B",
            "BB -> N",
            "BC -> B",
            "CC -> N",
            "CN -> C",
        };

        [Test]
        public void TestPart1() {
            Assert.AreEqual(1588, new Day14().ExecutePart1(_sampleLines));
        }

        [Test]
        public void TestPart2() {
            Assert.AreEqual(2188189693529L, new Day14().ExecutePart2(_sampleLines));
        }
    }
}
