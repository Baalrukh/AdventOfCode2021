using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2021 {
    public class Day7 : Exercise {
        public long ExecutePart1(string[] lines)
        {
            return ComputeAlignmentCost(lines, x => x);
        }

        private static long ComputeAlignmentCost(string[] lines, Func<int, int> motionCostComputer)
        {
            List<int> positions = lines.Single().Split(',').Select(int.Parse).ToList();

            int min = positions.Min();
            int max = positions.Max();

            return Enumerable.Range(1, max - min - 2)
                .Min(i => positions.Sum(x => motionCostComputer(Math.Abs(x - i))));
        }

        public long ExecutePart2(string[] lines) {
            return ComputeAlignmentCost(lines, x => x * (x + 1) / 2);
        }
    }
}
