using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2021 {
    public class Day10 : Exercise {


        public long ExecutePart1(string[] lines) {
            return lines.Sum(GetInvalidScore);
        }

        private int GetInvalidScore(string line) {
            if (Parse(line, out var c) == SyntaxStatus.ILLEGAL) {
                return _closingCharScore[c];
            }

            return 0;
        }

        public long ExecutePart2(string[] lines) {
            return -2;
        }

        public static SyntaxStatus Parse(string line, out char invalidChar) {
            Stack<Block> openBlocks = new Stack<Block>();
            foreach (char c in line) {
                if (_blocks.TryGetValue(c, out var block)) {
                    openBlocks.Push(block);
                } else if (openBlocks.Count > 0) {
                    if (openBlocks.Peek().IsClosingChar(c)) {
                        openBlocks.Pop();
                    } else {
                        invalidChar = c;
                        return SyntaxStatus.ILLEGAL;
                        // invalid
                    }
                } else {
                    invalidChar = c;
                    return SyntaxStatus.ILLEGAL;
                }
            }

            invalidChar = (char)0;
            if (openBlocks.Count > 0) {
                return SyntaxStatus.INCOMPLETE;
            }
            return SyntaxStatus.VALID;
        }

        public enum SyntaxStatus {
            VALID,
            INCOMPLETE,
            ILLEGAL
        }

        private static Dictionary<char, int> _closingCharScore = new Dictionary<char, int>() {
            {')', 3},
            {']', 57},
            {'}', 1197},
            {'>', 25137},
        };
        private static Dictionary<char, Block> _blocks = new Dictionary<char, Block>() {
            { '(', new Block('(', ')', 3) },
            { '[', new Block('[', ']', 57) },
            { '{', new Block('{', '}', 1197) },
            { '<', new Block('<', '>', 25137) },
        };

        internal class Block {
            private char OpeningChar;
            private char ClosingChar;
            private int Score;

            public Block(char openingChar, char closingChar, int score) {
                OpeningChar = openingChar;
                ClosingChar = closingChar;
                Score = score;
            }

            public bool IsClosingChar(char c) {
                return c == ClosingChar;
            }
        }
    }
}
