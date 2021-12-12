using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2021 {
    public class Day08 : Exercise {
        private static readonly int[] UniqueLengths = {2, 3, 4, 7};

        public long ExecutePart1(string[] lines) {
            return lines.Select(x => x.Split('|')[1])
                 .SelectMany(x => x.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries))
                 .Count(x => UniqueLengths.Contains(x.Length));
        }

        public long ExecutePart2(string[] lines) {
            return lines.Select(ResolveLine).Sum();
        }

        internal static int ResolveLine(string line) {
            string[] tokens = line.Split('|');
            return ResolveDigits(tokens[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries),
                tokens[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }

        public static int ResolveDigits(string[] input, string[] output)
        {
            var one = ToLetter(input.First(x => x.Length == 2));
            var four = ToLetter(input.First(x => x.Length == 4));
            var seven = ToLetter(input.First(x => x.Length == 3));
            var eight = ToLetter(input.First(x => x.Length == 7));

            var top = seven ^ one;

            var eightXorOne = eight & ~one & LetterMask;

            var sixLetterWords = input.Where(x => x.Length == 6).Select(ToLetter).ToList();
            var six = sixLetterWords.First(x => (x & ~one) == eightXorOne);
            var topRight = (Letter)(eight - six);
            var bottomRight = (Letter)(one - topRight);

            var tmp = sixLetterWords.Aggregate(NoLetter, ((letter, s) => letter | ~s));
            var bottomLeft = tmp & ~four & LetterMask;

            var bottom = eight & ~four & ~top & ~bottomLeft & LetterMask;
            var middle = tmp & ~bottomLeft & ~topRight & LetterMask;
            var topLeft = four & ~one & ~middle & LetterMask;

            Dictionary<Letter, int> digits = new Dictionary<Letter, int>() {
                { top | topLeft | topRight | bottomLeft | bottomRight | bottom, 0 },
                { topRight | bottomRight, 1 },
                { top | topRight | middle | bottomLeft | bottom, 2 },
                { top | topRight | middle | bottomRight | bottom, 3 },
                { topLeft | topRight | middle | bottomRight, 4 },
                { top | topLeft | middle | bottomRight | bottom, 5 },
                { top | topLeft | middle | bottomLeft | bottomRight | bottom, 6 },
                { top | topRight | bottomRight, 7 },
                { top | topLeft | topRight | middle | bottomLeft | bottomRight | bottom, 8 },
                { top | topLeft | topRight | middle | bottomRight | bottom, 9 },
            };

            return output.Select(x => digits[ToLetter(x)])
                .Aggregate(0, (res, d) => res * 10 + d);
        }

        private static Letter ToLetter(string text)
        {
            return text.Aggregate((Letter) 0, (letter, c) => letter | (Letter) (1 << (c - 'a')));
        }


        private static Letter NoLetter => 0;
        private const Letter LetterMask = (Letter)((1 << 7) - 1);
        
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
    }
}
