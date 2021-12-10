using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2021 {
    public class Day10 : Exercise {


        public long ExecutePart1(string[] lines) {
            return lines.Sum(GetInvalidScore);
        }

        private int GetInvalidScore(string line)
        {
            var (status, invalidChar) = Parse(line);
            if (status == SyntaxStatus.ILLEGAL) {
                return InvalidClosingCharScore[invalidChar];
            }

            return 0;
        }

        public long ExecutePart2(string[] lines)
        {
            var list = lines.Select(Parse2)
                .Where(x => string.IsNullOrEmpty(x.remainingChars) && (x.blocks.Count > 0))
                .Select(x => GetClosingTextScore(GetClosingText(x.blocks)))
                .OrderBy(x => x)
                .ToList();
            return list[list.Count / 2];
        }


        internal static (Stack<Block> blocks, string remainingChars) Parse2(string line)
        {
            Stack<Block> openBlocks = new Stack<Block>();
            for (var i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (_blocks.TryGetValue(c, out var block))
                {
                    openBlocks.Push(block);
                }
                else if (openBlocks.Count > 0)
                {
                    if (openBlocks.Peek().IsClosingChar(c))
                    {
                        openBlocks.Pop();
                    }
                    else
                    {
                        // invalid
                        return (openBlocks, line.Substring(i));
                    }
                }
                else
                {
                    return (openBlocks, line.Substring(i));
                }
            }

            return (openBlocks, "");
        }


        public static (SyntaxStatus status, char invalidChar) Parse(string line)
        {
            var (blocks, remainingChars) = Parse2(line);
            if (string.IsNullOrEmpty(remainingChars))
            {
                return (blocks.Count == 0 ? SyntaxStatus.VALID : SyntaxStatus.INCOMPLETE, (char) 0);
            }

            return (SyntaxStatus.ILLEGAL, remainingChars[0]);
        }

        internal static string GetClosingText(Stack<Block> blocks)
        {
            var stringBuilder = new StringBuilder();
            foreach (var block in blocks)
            {
                stringBuilder.Append(block.ClosingChar);
            }

            return stringBuilder.ToString();
        }

        public static long GetClosingTextScore(string text)
        {
            return text.Aggregate(0L, (score, c) => score * 5 + ClosingCharScore[c]);
        }

        public enum SyntaxStatus {
            VALID,
            INCOMPLETE,
            ILLEGAL
        }

        private static Dictionary<char, int> InvalidClosingCharScore = new Dictionary<char, int>() {
            {')', 3},
            {']', 57},
            {'}', 1197},
            {'>', 25137},
        };

        private static Dictionary<char, int> ClosingCharScore = new Dictionary<char, int>() {
            {')', 1},
            {']', 2},
            {'}', 3},
            {'>', 4},
        };

        private static Dictionary<char, Block> _blocks = new Dictionary<char, Block>() {
            { '(', new Block('(', ')') },
            { '[', new Block('[', ']') },
            { '{', new Block('{', '}') },
            { '<', new Block('<', '>') },
        };

        internal class Block {
            private char _openingChar;
            public char ClosingChar { get; }

            public Block(char openingChar, char closingChar) {
                _openingChar = openingChar;
                ClosingChar = closingChar;
            }

            public bool IsClosingChar(char c) {
                return c == ClosingChar;
            }
        }
    }
}
