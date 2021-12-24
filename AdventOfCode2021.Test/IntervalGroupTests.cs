using AdventOfCode2021.Utils;
using NUnit.Framework;

namespace AdventOfCode2021.Test {
    [TestFixture]
    public class IntervalGroupTests {

        [Test]
        public void TestAddWhenEmpty()
        {
            var intervals = new IntervalGroup();
            intervals.Add(new Interval(2, 8));
            Assert.AreEqual("[2/8]", intervals.ToString());
        }

        [Test]
        public void TestAddBefore()
        {
            var intervals = new IntervalGroup();
            intervals.Add(new Interval(2, 8));
            intervals.Add(new Interval(-5, -1));
            Assert.AreEqual("[-5/-1][2/8]", intervals.ToString());
        }

        [Test]
        public void TestAddAfter()
        {
            var intervals = new IntervalGroup();
            intervals.Add(new Interval(2, 8));
            intervals.Add(new Interval(10, 11));
            Assert.AreEqual("[2/8][10/11]", intervals.ToString());
        }

        [Test]
        public void TestAddInside()
        {
            var intervals = new IntervalGroup();
            intervals.Add(new Interval(2, 8));
            intervals.Add(new Interval(3, 5));
            Assert.AreEqual("[2/8]", intervals.ToString());
        }

        [Test]
        public void TestAddEmbracing()
        {
            var intervals = new IntervalGroup();
            intervals.Add(new Interval(2, 8));
            intervals.Add(new Interval(1, 10));
            Assert.AreEqual("[1/10]", intervals.ToString());
        }

        [Test]
        public void TestAddOver2Intervals()
        {
            var intervals = new IntervalGroup();
            intervals.Add(new Interval(2, 5));
            intervals.Add(new Interval(7, 10));
            intervals.Add(new Interval(3, 9));
            Assert.AreEqual("[2/10]", intervals.ToString());
        }

        [Test]
        public void TestAddBeween2Intervals()
        {
            var intervals = new IntervalGroup();
            intervals.Add(new Interval(2, 5));
            intervals.Add(new Interval(10, 15));
            intervals.Add(new Interval(7, 9));
            Assert.AreEqual("[2/5][7/9][10/15]", intervals.ToString());
        }

        // [Test]
        // public void TestAddOver2IntervalsTest()
        // {
        //     var intervals = new Day22.IntervalGroup();
        //     intervals.Add(new Day22.Interval(-3, -2));
        //     intervals.Add(new Day22.Interval(2, 8));
        //     intervals.Add(new Day22.Interval(20, 21));
        //     intervals.Add(new Day22.Interval(1, 10));
        //     Assert.AreEqual("[-3/-2][1/10][20/21]", intervals.ToString());
        // }

        [Test]
        public void TestRemoveLeft() {
            var intervals = new IntervalGroup();
            intervals.Add(new Interval(-3, -1));
            intervals.Add(new Interval(2, 10));
            intervals.Add(new Interval(20, 21));
            intervals.Remove(new Interval(0, 5));
            Assert.AreEqual("[-3/-1][6/10][20/21]", intervals.ToString());
        }

        [Test]
        public void TestRemoveRight() {
            var intervals = new IntervalGroup();
            intervals.Add(new Interval(-3, -1));
            intervals.Add(new Interval(2, 10));
            intervals.Add(new Interval(20, 21));
            intervals.Remove(new Interval(5, 15));
            Assert.AreEqual("[-3/-1][2/4][20/21]", intervals.ToString());
        }

        [Test]
        public void TestRemoveEmbracing() {
            var intervals = new IntervalGroup();
            intervals.Add(new Interval(-3, -1));
            intervals.Add(new Interval(2, 10));
            intervals.Add(new Interval(20, 21));
            intervals.Remove(new Interval(1, 15));
            Assert.AreEqual("[-3/-1][20/21]", intervals.ToString());
        }

        [Test]
        public void TestRemoveInside() {
            var intervals = new IntervalGroup();
            intervals.Add(new Interval(-3, -1));
            intervals.Add(new Interval(2, 10));
            intervals.Add(new Interval(20, 21));
            intervals.Remove(new Interval(5, 8));
            Assert.AreEqual("[-3/-1][2/4][9/10][20/21]", intervals.ToString());
        }

        [Test]
        public void TestRemoveOverlapping() {
            var intervals = new IntervalGroup();
            intervals.Add(new Interval(-3, -1));
            intervals.Add(new Interval(2, 10));
            intervals.Add(new Interval(20, 21));
            intervals.Remove(new Interval(-2, 8));
            Assert.AreEqual("[-3/-3][9/10][20/21]", intervals.ToString());
        }

        [Test]
        public void TestRemoveOverlappingAndEmbracing() {
            var intervals = new IntervalGroup();
            intervals.Add(new Interval(-3, -1));
            intervals.Add(new Interval(2, 10));
            intervals.Add(new Interval(20, 21));
            intervals.Remove(new Interval(-2, 20));
            Assert.AreEqual("[-3/-3][21/21]", intervals.ToString());
        }

        [Test]
        public void TestRemoveEmptyOnLeft() {
            var intervals = new IntervalGroup();
            intervals.Add(new Interval(-20, 26));
            intervals.Add(new Interval(28, 29));
            intervals.Remove(new Interval(-48, -32));
            Assert.AreEqual("[-20/26][28/29]", intervals.ToString());
        }

        [Test]
        public void TestRemoveEmptyOnRight() {
            var intervals = new IntervalGroup();
            intervals.Add(new Interval(-20, 26));
            intervals.Add(new Interval(28, 29));
            intervals.Remove(new Interval(32, 50));
            Assert.AreEqual("[-20/26][28/29]", intervals.ToString());
        }

        [Test]
        public void TestRemoveEmptyMiddle() {
            var intervals = new IntervalGroup();
            intervals.Add(new Interval(-20, 26));
            intervals.Add(new Interval(40, 50));
            intervals.Remove(new Interval(32, 35));
            Assert.AreEqual("[-20/26][40/50]", intervals.ToString());
        }

        [Test]
        public void TestIntervalGroup() {
            var intervals = new IntervalGroup();
            intervals.Add(new Interval(-20, 26));
            intervals.Remove(new Interval(-48, -32));
            intervals.Add(new Interval(20, 21));
            intervals.Remove(new Interval(-16, 35));
            Assert.AreEqual("[-20/-17]", intervals.ToString());
        }

        [Test]
        public void TestRemoveRange() {
            var intervals = new IntervalGroup();
            intervals.Add(new Interval(-83015, -9461));
            intervals.Add(new Interval(-1060,8475));
            intervals.Add(new Interval(79289, 80757));
            intervals.Remove(new Interval(-37810, 49457));
            Assert.AreEqual("[-83015/-37811][79289/80757]", intervals.ToString());
        }

        //[-83015/-9461][-1060/8475][79289/80757]
        //[-37810/49457]
        
    }
}