using System;
using System.Linq;

namespace AdventOfCode2021 {
    public class Day3 : Exercise {
        public int Execute(string[] lines) {
            int gammaRate = GetGammaRate(lines);
            int epsilonRate =GetComplementary(gammaRate, lines[0].Length);
            return gammaRate * epsilonRate;
        }

        public int ExecuteAdvanced(string[] lines) {
            return 0;
        }

        public static int GetComplementary(int value, int bitCount) {
            int mask = (1 << bitCount) - 1;
            value = ~value & mask;

            return value;
        }

        public static int GetGammaRate(string[] lines) {
            int[] oneCounts = new int[GetStringLength(lines)];
            oneCounts = lines.Aggregate(oneCounts, CountOnes);
            return oneCounts.Aggregate(0, (result, i) => (result << 1) + (i > lines.Length / 2 ? 1 : 0));
        }

        private static int GetStringLength(string[] lines) {
            return lines[0].Length;
        }

        private static int[] CountOnes(int[] oneCounts, string line) {
            for (int i = 0; i < line.Length; i++) {
                if (line[i] == '1') {
                    oneCounts[i]++;
                }
            }

            return oneCounts;
        }
    }
}