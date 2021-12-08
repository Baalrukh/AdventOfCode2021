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
    }
}
