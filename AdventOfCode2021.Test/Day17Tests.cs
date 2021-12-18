using NUnit.Framework;

namespace AdventOfCode2021.Test {
    [TestFixture]
    public class Day17Tests {
        private static readonly string[] _sampleLines = new[] {
            "target area: x=20..30, y=-10..-5"
        };

        [Test]
        public void TestParseArea()
        {
            Assert.AreEqual(new Day17.TargetArea(20, 30, -10, -5), Day17.ParseTargetArea(_sampleLines[0]));
        }

        [Test]
        public void TestXTrajectory()
        {
            int x0 = 0;
            int vx0 = 7;

            Assert.AreEqual(7, Day17.GetXPos(x0, vx0, 1));
            Assert.AreEqual(13, Day17.GetXPos(x0, vx0, 2));
            Assert.AreEqual(28, Day17.GetXPos(x0, vx0, 7));
            Assert.AreEqual(28, Day17.GetXPos(x0, vx0, 8));
        }

        [Test]
        public void TestYTrajectory()
        {
            int y0 = 0;
            int vy0 = 2;

            Assert.AreEqual(2, Day17.GetYPos(y0, vy0, 1));
            Assert.AreEqual(3, Day17.GetYPos(y0, vy0, 2));
            Assert.AreEqual(3, Day17.GetYPos(y0, vy0, 3));
            Assert.AreEqual(2, Day17.GetYPos(y0, vy0, 4));
            Assert.AreEqual(0, Day17.GetYPos(y0, vy0, 5));
            Assert.AreEqual(-3, Day17.GetYPos(y0, vy0, 6));

            Assert.AreEqual(0, Day17.GetYPos(0, 45, 91));


        }

        [Test]
        public void TestPart1() {
            Assert.AreEqual(45, new Day17().ExecutePart1(_sampleLines));
        }

        [Test]
        public void TestPart2() {
            Assert.AreEqual(112, new Day17().ExecutePart2(_sampleLines));
        }
    }
}