using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2021 {
    public class Day8 : Exercise {
        private int[] UniqueLengths = new[] {2, 3, 4, 7};

        public long ExecutePart1(string[] lines) {
            return lines.Select(x => x.Split('|')[1])
                 .SelectMany(x => x.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries))
                 .Count(x => UniqueLengths.Contains(x.Length));
        }

        public long ExecutePart2(string[] lines) {
            return -2;
        }

        public static int ResolveDigits(string[] input, string[] output)
        {
            var inputLetters = input.Select(ToLetter).ToList();
            var mapping = new Dictionary<Letter, DigitBars>();


            var one = ToLetter(input.First(x => x.Length == 2));
            var four = ToLetter(input.First(x => x.Length == 4));
            var seven = ToLetter(input.First(x => x.Length == 3));
            var eight = ToLetter(input.First(x => x.Length == 7));

            var topBar = (Letter)(seven ^ one);

            var eightXorOne = eight & ~one;
            // foreach (var inputLetter in inputLetters)
            // {
            //     if (inputLetter == eight)
            //     {
            //         continue;
            //     }
            //     var letter = inputLetter & ~one;
            //     if (letter == eightXorOne)
            //     {
            //         Letter six1 = inputLetter;
            //     }
            // }

            var six = inputLetters.First(x => (x != eight) && (x & ~one) == eightXorOne);
            var topRight = (Letter)(eight - six);
            var bottomRight = (Letter)(one - topRight);

            var tmp = input.Where(x => x.Length == 6).Aggregate(NoLetter, ((letter, s) => letter | ~ToLetter(s)));
            var bottomLeft = tmp & ~four;

            // tmp = input.Where(x => x.Length == 5).Aggregate(NoLetter, ((letter, s) => letter & ToLetter(s)));
            var bottom = eight & ~four & ~topBar & ~bottomLeft;
            var middle = tmp & ~bottomLeft & ~topRight;
            return -1;
        }

        private static Letter ToLetter(string text)
        {
            // Letter letter = 0;
            // foreach (var c in text)
            // {
            //     var newLetter = (Letter)(1 << (c - 'a'));
            //     letter = letter | newLetter;
            // }
            //
            // return letter;

            return text.Aggregate((Letter) 0, (letter, c) => letter | (Letter) (1 << (c - 'a')));
        }


        private static Letter NoLetter => 0;
        
        [Flags]
        public enum Letter
        {
            A = 1 << 0,
            B = 1 << 1,
            C = 1 << 2,
            D = 1 << 3,
            E = 1 << 4,
            F = 1 << 5,
            G = 1 << 6
        }

        public const DigitBars DIGIT_0 = DigitBars.TOP | DigitBars.TOP_LEFT | DigitBars.TOP_RIGHT | DigitBars.BOTTOM_LEFT | DigitBars.BOTTOM_RIGHT | DigitBars.BOTTOM;
        public const DigitBars DIGIT_1 = DigitBars.TOP_RIGHT | DigitBars.BOTTOM_RIGHT;
        public const DigitBars DIGIT_2 = DigitBars.TOP | DigitBars.TOP_RIGHT | DigitBars.MIDDLE | DigitBars.BOTTOM_LEFT | DigitBars.BOTTOM;
        public const DigitBars DIGIT_3 = DigitBars.TOP | DigitBars.TOP_RIGHT | DigitBars.MIDDLE | DigitBars.BOTTOM_RIGHT | DigitBars.BOTTOM;
        public const DigitBars DIGIT_4 = DigitBars.TOP_LEFT | DigitBars.TOP_RIGHT | DigitBars.MIDDLE | DigitBars.BOTTOM_RIGHT;
        public const DigitBars DIGIT_5 = DigitBars.TOP | DigitBars.TOP_LEFT | DigitBars.MIDDLE | DigitBars.BOTTOM_RIGHT | DigitBars.BOTTOM;
        public const DigitBars DIGIT_6 = DigitBars.TOP | DigitBars.TOP_LEFT | DigitBars.MIDDLE | DigitBars.BOTTOM_LEFT | DigitBars.BOTTOM_RIGHT | DigitBars.BOTTOM;
        public const DigitBars DIGIT_7 = DigitBars.TOP | DigitBars.TOP_RIGHT | DigitBars.BOTTOM_RIGHT;
        public const DigitBars DIGIT_8 = DigitBars.TOP | DigitBars.TOP_LEFT | DigitBars.TOP_RIGHT | DigitBars.MIDDLE | DigitBars.BOTTOM_LEFT | DigitBars.BOTTOM_RIGHT | DigitBars.BOTTOM;
        public const DigitBars DIGIT_9 = DigitBars.TOP | DigitBars.TOP_LEFT | DigitBars.TOP_RIGHT | DigitBars.MIDDLE | DigitBars.BOTTOM_RIGHT | DigitBars.BOTTOM;

        [Flags]
        public enum DigitBars
        {
            TOP = 1 << 0,
            TOP_LEFT = 1 << 1,
            TOP_RIGHT = 1 << 2,
            MIDDLE = 1 << 3,
            BOTTOM_LEFT = 1 << 4,
            BOTTOM_RIGHT = 1 << 5,
            BOTTOM = 1 << 6,

        }
    }
}
