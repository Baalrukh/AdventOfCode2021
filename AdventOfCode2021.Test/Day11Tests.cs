using AdventOfCode2021.Utils;
using NUnit.Framework;

namespace AdventOfCode2021.Test {
    [TestFixture]
    public class Day11Tests {
        private static readonly string[] _sampleLines = new[] {
            "5483143223",
            "2745854711",
            "5264556173",
            "6141336146",
            "6357385478",
            "4167524645",
            "2176841721",
            "6882881134",
            "4846848554",
            "5283751526"
        };

        private static readonly string[] _smallMap = {
            "11111",
            "19991",
            "19191",
            "19991",
            "11111",
        };
        
        [Test]
        public void TestEnergizeIncreaseEachCellByOne() {
            Day11.CaveMap caveMap = Day11.CaveMap.Parse(new[] { "123", "456", "789", });
            caveMap.Energize();
            Assert.AreEqual("234\n567\n8910\n", caveMap.ToString());
        }

        [Test]
        public void TestGetFlashingOctopusPositions() {
            Day11.CaveMap caveMap = Day11.CaveMap.Parse(new []{ "193", "456", "789", });
            CollectionAssert.AreEqual(new[] {new IntVector2(1, 0), new IntVector2(2, 2)}, caveMap.GetFlashingOctopusPositions());
        }

        [Test]
        public void TestFlashAllOctopuses() {
            Day11.CaveMap caveMap = Day11.CaveMap.Parse(_smallMap);
            int flashedOctopuses = caveMap.FlashAllOctopuses();
            Assert.AreEqual("34543\n40004\n50005\n40004\n34543\n", caveMap.ToString());
            Assert.AreEqual(9, flashedOctopuses);
        }
        
        [Test]
        public void TestPart1() {
            Assert.AreEqual(1656, new Day11().ExecutePart1(_sampleLines));
        }

        [Test]
        public void TestPart2() {
            Assert.AreEqual(195, new Day11().ExecutePart2(_sampleLines));
        }
    }
}
