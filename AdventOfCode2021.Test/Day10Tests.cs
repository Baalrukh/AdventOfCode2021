using NUnit.Framework;

namespace AdventOfCode2021.Test {
    [TestFixture]
    public class Day10Tests {
        private static readonly string[] _sampleLines = new[] {
            "[({(<(())[]>[[{[]{<()<>>",
            "[(()[<>])]({[<{<<[]>>(",
            "{([(<{}[<>[]}>{[]{[(<()>",
            "(((({<>}<{<{<>}{[]{[]{}",
            "[[<[([]))<([[{}[[()]]]",
            "[{[{({}]{}}([{[{{{}}([]",
            "{<[[]]>}<{[{[{[]{()[[[]",
            "[<(<(<(<{}))><([]([]()",
            "<{([([[(<>()){}]>(<<{{",
            "<{([{{}}[<[[[<>{}]]]>[]]"
        };

        [Test]
        public void TestParseValidLine() {
            Assert.AreEqual(Day10.SyntaxStatus.VALID, Day10.Parse("()[]<>{}", out var c));
            Assert.AreEqual(Day10.SyntaxStatus.VALID, Day10.Parse("([<{}>])", out c));
        }

        [Test]
        public void TestParseIncompleteLine() {
            Assert.AreEqual(Day10.SyntaxStatus.INCOMPLETE, Day10.Parse("()[]<>{", out var c));
            Assert.AreEqual(Day10.SyntaxStatus.INCOMPLETE, Day10.Parse("([<{}>]", out c));
        }

        [Test]
        public void TestParseCorruptLine() {
            Assert.AreEqual(Day10.SyntaxStatus.ILLEGAL, Day10.Parse("(])[]<>{}", out var c));
            Assert.AreEqual(']', c);
            Assert.AreEqual(Day10.SyntaxStatus.ILLEGAL, Day10.Parse("([<}{}>]}", out c));
            Assert.AreEqual('}', c);
            Assert.AreEqual(Day10.SyntaxStatus.ILLEGAL, Day10.Parse(">([<{}>]}", out c));
            Assert.AreEqual('>', c);
        }

        [Test]
        public void TestPart1() {
            Assert.AreEqual(26397, new Day10().ExecutePart1(_sampleLines));
        }

        [Test]
        public void TestPart2() {
            Assert.AreEqual(-20, new Day10().ExecutePart2(_sampleLines));
        }
    }
}
