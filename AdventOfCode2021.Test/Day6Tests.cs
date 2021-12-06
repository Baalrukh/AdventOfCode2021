using NUnit.Framework;

namespace AdventOfCode2021.Test {
    [TestFixture]
    public class Day6Tests {
        private static readonly string[] _sampleLines = new[] {
            "3,4,3,1,2"
        };

        [Test]
        public void TestFishEcoSystem2_EvolveReducesTimeByOne()
        {
            var ecoSystem = new Day6.FishEcoSystem(2, 7, new long[] {0, 1, 2, 3, 4, 5, 6});
            ecoSystem.Evolve(1);

            CollectionAssert.AreEqual(new[] {1, 2, 3, 4, 5, 6, 0, 0, 0}, ecoSystem.Population);
        }

        [Test]
        public void TestFishEcoSystem2_EvolveCreatesNewBornAndShiftsBirthingToBirthDelay()
        {
            var ecoSystem = new Day6.FishEcoSystem(2, 7, new long[] {1, 0, 0, 0, 0, 0, 0});
            ecoSystem.Evolve(1);

            CollectionAssert.AreEqual(new[] {0, 0, 0, 0, 0, 0, 1, 0, 1}, ecoSystem.Population);
        }

        [Test]
        public void TestFishEcoSystem2_EvolveCumulatesNewBirthWithBirthing()
        {
            var ecoSystem = new Day6.FishEcoSystem(2, 7, new long[] {1, 1, 1, 0, 0, 0, 0});
            ecoSystem.Evolve(3);

            CollectionAssert.AreEqual(new[] {0, 0, 0, 0, 1, 1, 2, 1, 1}, ecoSystem.Population);
        }


        [Test]
        public void TestPart1() {
            Assert.AreEqual(5934, new Day6().ExecutePart1(_sampleLines));
        }

        [Test]
        public void TestPart2() {
            Assert.AreEqual(26984457539L, new Day6().ExecutePart2(_sampleLines));
        }
    }
}
