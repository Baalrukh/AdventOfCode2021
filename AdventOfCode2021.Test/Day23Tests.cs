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
        public void TestCanGoHomeCorridor_LeftToRight() {
            Day23.AWorldState worldState = Day23.AWorldState.Parse("B   D  ", "  ", "  ", "  ", "  ");
            Assert.IsTrue(worldState.CanGoHomeFromCorridor(0, 2.5f));
            Assert.IsFalse(worldState.CanGoHomeFromCorridor(0, 4.5f));
        }

        [Test]
        public void TestCanGoHomeFromCorridor_RightToLeft() {
            Day23.AWorldState worldState = Day23.AWorldState.Parse("  D   B", "  ", "  ", "  ", "  ");
            Assert.IsTrue(worldState.CanGoHomeFromCorridor(6, 4.5f));
            Assert.IsFalse(worldState.CanGoHomeFromCorridor(6, 1.5f));
        }

        [Test]
        public void TestCanGoHomeFromHome() {
            Day23.AWorldState worldState = Day23.AWorldState.Parse("    A  ", "  ", "  ", "  ", "  ");
            Assert.IsTrue(worldState.CanGoHomeFromHome(1.5f, 2.5f));
            Assert.IsFalse(worldState.CanGoHomeFromHome(1.5f, 4.5f));
            Assert.IsTrue(worldState.CanGoHomeFromHome(2.5f, 1.5f));
            Assert.IsFalse(worldState.CanGoHomeFromHome(4.5f, 1.5f));
        }

        [Test]
        public void TestCostCorridorToHome_HomeEmpty() {
            Day23.AWorldState worldState = Day23.AWorldState.Parse("       ", "  ", "  ", "  ", "  ");
            Assert.AreEqual(60, worldState.GetCostCorridorToHome(0, 2.5f, Day23.AmphipodFamily.B));
            Assert.AreEqual(50, worldState.GetCostCorridorToHome(1, 2.5f, Day23.AmphipodFamily.B));
            Assert.AreEqual(30, worldState.GetCostCorridorToHome(3, 2.5f, Day23.AmphipodFamily.B));
            Assert.AreEqual(80, worldState.GetCostCorridorToHome(6, 2.5f, Day23.AmphipodFamily.B));
        }

        [Test]
        public void TestCostCorridorToHome_HomeNotEmpty() {
            Day23.AWorldState worldState = Day23.AWorldState.Parse("       ", "  ", "  ", " C", "  ");
            Assert.AreEqual(700, worldState.GetCostCorridorToHome(0, 3.5f, Day23.AmphipodFamily.C));
        }

        [Test]
        public void TestGetMoveCostHomeToHome() {
            Assert.AreEqual(700, Day23.AWorldState.GetMoveCostHomeToHome(Day23.AmphipodFamily.C, 1, 0, 1.5f, 3.5f));
        }

        [Test]
        public void TestGetCostHomeToCorridor() {
            Day23.AWorldState worldState = Day23.AWorldState.Parse("  B    ", "BA", " D", "CC", "DA");
            Assert.AreEqual(3000, worldState.GetCostHomeToCorridor(3, 2.5f, Day23.AmphipodFamily.D, Day23.AmphipodFamily.B));
        }

        [Test]
        public void TestPart1() {
            Assert.AreEqual(12521, new Day23().ExecutePart1(_sampleLines));
        }

        [Test]
        public void TestPart2() {
            Assert.AreEqual(44169, new Day23().ExecutePart2(_sampleLines));
        }
    }
}
