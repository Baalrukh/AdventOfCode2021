using AdventOfCode2021.Utils;
using NUnit.Framework;

namespace AdventOfCode2021.Test {
    [TestFixture]
    public class Day5Tests {
        private static readonly string[] _sampleLines = new[] {
            "0,9 -> 5,9",
            "8,0 -> 0,8",
            "9,4 -> 3,4",
            "2,2 -> 2,1",
            "7,0 -> 7,4",
            "6,4 -> 2,0",
            "0,9 -> 2,9",
            "3,4 -> 1,4",
            "0,0 -> 8,8",
            "5,5 -> 8,2",
        };

        [Test]
        public void TestParseLine() {
            Assert.AreEqual(new Day5.Line(0, 9,5, 9), Day5.Line.Parse("0,9 -> 5,9"));
        }

        [Test]
        public void TestIsHorizontal() {
            Assert.IsFalse(new Day5.Line(0, 9,5, 8).IsHorizontal);
            Assert.IsTrue(new Day5.Line(0, 9,5, 9).IsHorizontal);
        }

        [Test]
        public void TestIsVertical() {
            Assert.IsFalse(new Day5.Line(0, 9,5, 2).IsVertical);
            Assert.IsTrue(new Day5.Line(0, 9,0, 2).IsVertical);
        }

        [Test]
        public void TestEnumeratePositionsOnHorizontalLine_Increasing() {
            Day5.Line line = new Day5.Line(0, 9, 3, 9);
            CollectionAssert.AreEqual(
                new[] { new IntVector2(0, 9), new IntVector2(1, 9), new IntVector2(2, 9), new IntVector2(3, 9) },
                line.EnumeratePositions());
        }

        [Test]
        public void TestEnumeratePositionsOnHorizontalLine_Decreasing() {
            Day5.Line line = new Day5.Line(3, 9, 0, 9);
            CollectionAssert.AreEqual(
                new[] { new IntVector2(3, 9), new IntVector2(2, 9), new IntVector2(1, 9), new IntVector2(0, 9) },
                line.EnumeratePositions());
        }

        [Test]
        public void TestEnumeratePositionsOnVerticalLine_Increasing() {
            Day5.Line line = new Day5.Line(9, 0, 9, 3);
            CollectionAssert.AreEqual(
                new[] { new IntVector2(9, 0), new IntVector2(9, 1), new IntVector2(9, 2), new IntVector2(9, 3) },
                line.EnumeratePositions());
        }

        [Test]
        public void TestEnumeratePositionsOnVerticalLine_Decreasing() {
            Day5.Line line = new Day5.Line(9, 3, 9, 0);
            CollectionAssert.AreEqual(
                new[] { new IntVector2(9, 3), new IntVector2(9, 2), new IntVector2(9, 1), new IntVector2(9, 0) },
                line.EnumeratePositions());
        }
        
        [Test]
        public void TestPart1() {
            Assert.AreEqual(5, Day5.GetDangerousPointCountForHorizontalAndVertical(_sampleLines));
        }
        
        [Test]
        public void TestPart2() {
            Assert.AreEqual(12, Day5.GetAllDangerousPointCount(_sampleLines));
        }
        
    }
}