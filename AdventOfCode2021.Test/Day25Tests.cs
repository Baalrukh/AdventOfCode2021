using NUnit.Framework;

namespace AdventOfCode2021.Test {
    [TestFixture]
    public class Day25Tests {
        private static readonly string[] _sampleLines = new[] {
            "v...>>.vv>",
            ".vv>>.vv..",
            ">>.>v>...v",
            ">>v>>.>.v.",
            "v>v.vv.v..",
            ">.>>..v...",
            ".vv..>.>v.",
            "v.v..>>v.v",
            "....v..v.>",
        };


        [Test]
        public void TestSample_Step1() {
            Day25.Area area = Day25.Area.Parse(_sampleLines);
            area.Step();

            string[] expected = {
                "....>.>v.>",
                "v.v>.>v.v.",
                ">v>>..>v..",
                ">>v>v>.>.v",
                ".>v.v...v.",
                "v>>.>vvv..",
                "..v...>>..",
                "vv...>>vv.",
                ">.v.v..v.v",
            };
            string map = area.Plot();

            Assert.AreEqual(string.Join("\r\n", expected) + "\r\n", map);
        }

        [Test]
        public void TestSample_Step2() {
            Day25.Area area = Day25.Area.Parse(_sampleLines);
            area.Step();
            area.Step();

            string[] expected = {
                ">.v.v>>..v",
                "v.v.>>vv..",
                ">v>.>.>.v.",
                ">>v>v.>v>.",
                ".>..v....v",
                ".>v>>.v.v.",
                "v....v>v>.",
                ".vv..>>v..",
                "v>.....vv.",
            };
            string map = area.Plot();

            Assert.AreEqual(string.Join("\r\n", expected) + "\r\n", map);
        }
        [Test]
        public void TestPart1() {
            Assert.AreEqual(58, new Day25().ExecutePart1(_sampleLines));
        }

        [Test]
        public void TestPart2() {
            Assert.AreEqual(-20, new Day25().ExecutePart2(_sampleLines));
        }
    }
}
