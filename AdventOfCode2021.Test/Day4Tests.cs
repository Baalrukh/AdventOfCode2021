using System.Linq;
using AdventOfCode2021.Utils;
using NUnit.Framework;

namespace AdventOfCode2021.Test {
    [TestFixture]
    public class Day4Tests {
        private static readonly string[] _sampleLines = new[] {
            "7,4,9,5,11,17,23,2,0,14,21,24,10,16,13,6,15,25,12,22,18,20,8,19,3,26,1",
            "",
            "22 13 17 11  0",
            "8  2 23  4 24",
            "21  9 14 16  7",
            "6 10  3 18  5",
            "1 12 20 15 19",
            "",
            "3 15  0  2 22",
            "9 18 13 17  5",
            "19  8  7 25 23",
            "20 11 10 24  4",
            "14 21 16 12  6",
            "",
            "14 21 17 24  4",
            "10 16 15  9 19",
            "18  8 23 26 20",
            "22 11 13  6  5",
            "2  0 12  3  7",
        };


        [Test]
        public void TestParseInput() {
            Assert.AreEqual(new Day4.Bingo(
                    new int[] {
                        7, 4, 9, 5, 11, 17, 23, 2, 0, 14, 21, 24, 10, 16, 13, 6, 15, 25, 12, 22, 18, 20, 8, 19, 3, 26, 1
                    },
                    new Day4.Grid[] {
                        new Day4.Grid(new int[] {
                            22, 13, 17, 11, 0, 8, 2, 23, 4, 24, 21, 9, 14, 16, 7, 6, 10, 3, 18, 5, 1, 12, 20, 15, 19
                        }),
                        new Day4.Grid(new int[] {
                            3, 15, 0, 2, 22, 9, 18, 13, 17, 5, 19, 8, 7, 25, 23, 20, 11, 10, 24, 4, 14, 21, 16, 12, 6
                        }),
                        new Day4.Grid(new int[] {
                            14, 21, 17, 24, 4, 10, 16, 15, 9, 19, 18, 8, 23, 26, 20, 22, 11, 13, 6, 5, 2, 0, 12, 3, 7
                        })
                    }),
                Day4.Bingo.Parse(_sampleLines));
        }

        [Test]
        public void TestGridLineComplete() {
            Day4.Grid grid = CreateTestGrid();
            
            Assert.IsFalse(grid.IsLineComplete(1));
            grid.Mark(0, 1);
            grid.Mark(1, 1);
            grid.Mark(2, 1);
            grid.Mark(3, 1);
            Assert.IsFalse(grid.IsLineComplete(1));
            grid.Mark(4, 1);
            Assert.IsTrue(grid.IsLineComplete(1));
        }

        [Test]
        public void TestGridColumnComplete() {
            Day4.Grid grid = CreateTestGrid();
            
            Assert.IsFalse(grid.IsColumnComplete(1));
            grid.Mark(1, 0);
            grid.Mark(1, 1);
            grid.Mark(1, 2);
            grid.Mark(1, 3);
            Assert.IsFalse(grid.IsColumnComplete(1));
            grid.Mark(1, 4);
            Assert.IsTrue(grid.IsColumnComplete(1));
        }

        [Test]
        public void TestGridDiagonalCompleteReturnsFalseIfPositionNotOnDiagonal() {
            Day4.Grid grid = CreateTestGrid();
            
            Assert.IsFalse(grid.IsDiagonalComplete(0, 1));
        }

        [Test]
        public void TestGridDiagonalCompleteForTopLeftToBottomRight() {
            Day4.Grid grid = CreateTestGrid();
            
            Assert.IsFalse(grid.IsDiagonalComplete(1, 1));
            grid.Mark(0, 0);
            grid.Mark(1, 1);
            grid.Mark(2, 2);
            grid.Mark(3, 3);
            Assert.IsFalse(grid.IsDiagonalComplete(1, 1));
            grid.Mark(4, 4);
            Assert.IsTrue(grid.IsDiagonalComplete(1, 1));
        }

        [Test]
        public void TestGridDiagonalCompleteForTopRightToBottomLeft() {
            Day4.Grid grid = CreateTestGrid();
            
            Assert.IsFalse(grid.IsDiagonalComplete(1, 3));
            grid.Mark(0, 4);
            grid.Mark(1, 3);
            grid.Mark(2, 2);
            grid.Mark(3, 1);
            Assert.IsFalse(grid.IsDiagonalComplete(1, 3));
            grid.Mark(4, 0);
            Assert.IsTrue(grid.IsDiagonalComplete(1, 3));
        }

        [Test]
        public void TestMarkValue() {
            Day4.Grid grid = CreateTestGrid();
            CollectionAssert.AreEqual(new [] {new IntVector2(3, 0)}, grid.MarkValue(3));
            Assert.AreEqual("   X                     ", grid.GetMarkedGrid());
            CollectionAssert.AreEqual(new [] {new IntVector2(0, 0), new IntVector2(0, 4)}, grid.MarkValue(20));
            Assert.AreEqual("X  X                X    ", grid.GetMarkedGrid());
        }

        [Test]
        public void TestGetGridScoreReturnSumOfAllUnmarkedValues() {
            Day4.Grid grid = CreateTestGrid();
            grid.MarkValue(3).ToArray();
            grid.MarkValue(20).ToArray();
            int expectedSum = Enumerable.Range(0, 25).Sum() - 20 - 3;
            Assert.AreEqual(expectedSum, grid.GetGridScore());
        }
        
        private static Day4.Grid CreateTestGrid() {
            Day4.Grid grid = new Day4.Grid(new[]
                { 20, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 });
            return grid;
        }

        [Test]
        public void TestBingoPart1() {
            Day4.Bingo bingo = Day4.Bingo.Parse(_sampleLines);
            Assert.AreEqual(4512, bingo.GetWinningScore());
        }

        [Test]
        public void TestBingoPart2() {
            Day4.Bingo bingo = Day4.Bingo.Parse(_sampleLines);
            Assert.AreEqual(1924, bingo.GetLastWinningScore());
        }
    }
}