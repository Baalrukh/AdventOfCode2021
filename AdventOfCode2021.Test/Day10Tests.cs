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
            Assert.AreEqual(Day10.SyntaxStatus.VALID, Day10.Parse("()[]<>{}").status);
            Assert.AreEqual(Day10.SyntaxStatus.VALID, Day10.Parse("([<{}>])").status);
        }

        [Test]
        public void TestParseIncompleteLine() {
            Assert.AreEqual(Day10.SyntaxStatus.INCOMPLETE, Day10.Parse("()[]<>{").status);
            Assert.AreEqual(Day10.SyntaxStatus.INCOMPLETE, Day10.Parse("([<{}>]").status);
        }

        [Test]
        public void TestParseCorruptLine() {
            Assert.AreEqual((Day10.SyntaxStatus.ILLEGAL, ']'), Day10.Parse("(])[]<>{}"));
            Assert.AreEqual((Day10.SyntaxStatus.ILLEGAL, '}'), Day10.Parse("([<}{}>]}"));
            Assert.AreEqual((Day10.SyntaxStatus.ILLEGAL, '>'), Day10.Parse(">([<{}>]}"));
        }

        [Test]
        public void TestGetClosingText()
        {
            var blocks = Day10.Parse2("<{([{{}}[<[[[<>{}]]]>[]]").blocks;
            Assert.AreEqual("])}>", Day10.GetClosingText(blocks));
        }

        [Test]
        public void TestGetClosingTextScore()
        {
            Assert.AreEqual(294, Day10.GetClosingTextScore("])}>"));
        }

        [Test]
        public void TestPart2OnSingleLine()
        {
            Assert.AreEqual(288957, new Day10().ExecutePart2(new [] {"[({(<(())[]>[[{[]{<()<>>"}));
            Assert.AreEqual(5566, new Day10().ExecutePart2(new [] {"[(()[<>])]({[<{<<[]>>("}));
            Assert.AreEqual(1480781, new Day10().ExecutePart2(new [] {"(((({<>}<{<{<>}{[]{[]{}"}));
            Assert.AreEqual(995444, new Day10().ExecutePart2(new [] {"{<[[]]>}<{[{[{[]{()[[[]"}));
            Assert.AreEqual(294, new Day10().ExecutePart2(new [] {"<{([{{}}[<[[[<>{}]]]>[]]"}));
        }

        [Test]
        public void TestPart1() {
            Assert.AreEqual(26397, new Day10().ExecutePart1(_sampleLines));
        }

        [Test]
        public void TestPart2() {
            Assert.AreEqual(288957, new Day10().ExecutePart2(_sampleLines));
        }
    }
}
