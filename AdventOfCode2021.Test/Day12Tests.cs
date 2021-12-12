using System.Linq;
using NUnit.Framework;

namespace AdventOfCode2021.Test {
    [TestFixture]
    public class Day12Tests {
        private static readonly string[] _sampleLines_Small = new[] {
            "start-A", "start-b", "A-c", "A-b", "b-d", "A-end", "b-end"
        };
        private static readonly string[] _sampleLines_Medium = new[] {
            "dc-end", "HN-start", "start-kj", "dc-start", "dc-HN", "LN-dc", "HN-end", "kj-sa", "kj-HN", "kj-dc"
        };
        private static readonly string[] _sampleLines_Large = new[] {
            "fs-end", "he-DX", "fs-he", "start-DX", "pj-DX", "end-zg", "zg-sl", "zg-pj", "pj-he", "RW-he", "fs-DX",
            "pj-RW", "zg-RW", "start-pj", "he-WI", "zg-he", "pj-fs", "start-RW"
        };

        [Test]
        public void TestParse() {
            var nodes = Day12.Parse(_sampleLines_Small);
            Assert.AreEqual(6, nodes.Count);
            Day12.Node expected = new Day12.Node("A", Day12.NodeType.Large);
            expected.Neighbours.AddRange(new [] { nodes["start"], nodes["c"], nodes["b"], nodes["end"], });
            Assert.AreEqual(expected, nodes["A"]);
        }

        [Test]
        public void TestEnumerateAllPath() {
            var nodes = Day12.Parse(_sampleLines_Small);
            var paths = Day12.EnumerateAllPaths(nodes, 0).Select(x => string.Join(",", x.Select(n => n.Name))).ToList();
            CollectionAssert.AreEquivalent(new [] {
                "start,A,b,A,c,A,end",
                "start,A,b,A,end",
                "start,A,b,end",
                "start,A,c,A,b,A,end",
                "start,A,c,A,b,end",
                "start,A,c,A,end",
                "start,A,end",
                "start,b,A,c,A,end",
                "start,b,A,end",
                "start,b,end",
            }, paths);
        }
        
        [Test]
        public void TestPart1() {
            Assert.AreEqual(10, new Day12().ExecutePart1(_sampleLines_Small));
            Assert.AreEqual(19, new Day12().ExecutePart1(_sampleLines_Medium));
            Assert.AreEqual(226, new Day12().ExecutePart1(_sampleLines_Large));
        }

        [Test]
        public void TestPart2() {
            Assert.AreEqual(36, new Day12().ExecutePart2(_sampleLines_Small));
            Assert.AreEqual(103, new Day12().ExecutePart2(_sampleLines_Medium));
            Assert.AreEqual(3509, new Day12().ExecutePart2(_sampleLines_Large));
        }
    }
}
