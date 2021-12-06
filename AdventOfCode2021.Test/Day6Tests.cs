using NUnit.Framework;

namespace AdventOfCode2021.Test {
    [TestFixture]
    public class Day6Tests {
        private static readonly string[] _sampleLines = new[] {
            "3,4,3,1,2"
        };

        private Day6.FishEcoSystem _ecoSystem = new Day6.FishEcoSystem(2, 7);

        [Test]
        public void TestGetGetFishCount_BeforeDuplication()
        {
            Assert.AreEqual(1, _ecoSystem.GetFishCount(5, 3));
        }

        [Test]
        public void TestGetGVetFishCount_At1Duplication()
        {
            Assert.AreEqual(2, _ecoSystem.GetFishCount(3, 4));
        }

        [Test]
        public void TestGetGVetFishCount_After1Duplication()
        {
            Assert.AreEqual(2, _ecoSystem.GetFishCount(5, 7));
        }

        [Test]
        public void TestGetGVetFishCount_After2Duplications()
        {
            Assert.AreEqual(1, _ecoSystem.GetFishCount(5, 5));
            Assert.AreEqual(2, _ecoSystem.GetFishCount(5, 6));
            Assert.AreEqual(2, _ecoSystem.GetFishCount(5, 12));
            Assert.AreEqual(3, _ecoSystem.GetFishCount(5, 13));
            Assert.AreEqual(3, _ecoSystem.GetFishCount(5, 14));
            Assert.AreEqual(4, _ecoSystem.GetFishCount(5, 15));
            Assert.AreEqual(4, _ecoSystem.GetFishCount(5, 16));
        }

        [Test]
        public void TestPart1() {
            Assert.AreEqual(5934, new Day6().ExecutePart1(_sampleLines));
        }

        [Test]
        public void TestPart2() {
            Assert.Fail();
            // Assert.AreEqual(26984457539, new Day6().ExecutePart2(_sampleLines));
        }
    }
}
