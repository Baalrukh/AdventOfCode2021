using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2021 {
    public class Day7 : Exercise {
        public long ExecutePart1(string[] lines) {
            List<int> positions = lines.Single().Split(',').Select(int.Parse).ToList();

            int min = positions.Min();
            int max = positions.Max();

            int bestCost = int.MaxValue;
            for (int i = min + 1; i < max; i++) {
                int cost = positions.Sum(x => Math.Abs(x - i));
                if (cost < bestCost) {
                    bestCost = cost;
                }
            }

            return bestCost;
        }

        public long ExecutePart2(string[] lines) {
            return -2;
        }
    }
}
