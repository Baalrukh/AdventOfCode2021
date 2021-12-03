using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2021 {
    public class Day3 : Exercise
    {
        public int Execute(string[] lines)
        {
            var values = ParseValues(lines);
            var bitCount = lines[0].Length;
            int gammaRate = GetGammaRate(values, bitCount);
            int epsilonRate = GetComplementary(gammaRate, bitCount);
            return gammaRate * epsilonRate;
        }

        public static int[] ParseValues(string[] lines)
        {
            var values = lines.Select(x => Convert.ToInt32(x, 2)).ToArray();
            return values;
        }

        public int ExecuteAdvanced(string[] lines)
        {
            var (oxy, co2) = GetOxygenAndCO2Ratings(lines);
            return oxy * co2;
        }

        public static (int oxy, int oc2) GetOxygenAndCO2Ratings(string[] lines)
        {
            var values = lines.Select(x => new ReportInfo(x)).ToList();
            var bitCount = lines[0].Length;
            return GetOxygenAndCO2Ratings(values, bitCount);
        }

        public static (int oxy, int oc2) GetOxygenAndCO2Ratings(IReadOnlyList<ReportInfo> values, int bitCount)
        {
            var oxy = Count(values, bitCount, 1);
            var co2 = Count(values, bitCount, 0);
            return (oxy, co2);
        }

        private static int Count(IReadOnlyList<ReportInfo> values, int bitCount, int countReference)
        {
            List<ReportInfo> filtered = new List<ReportInfo>(values);
            for (int i = 0; i < bitCount; i++)
            {
                int index = bitCount - i - 1;
                var sum = filtered.Count(x => x[index] == countReference);
                int expectedValue;
                if (sum == filtered.Count - sum)
                {
                    expectedValue = countReference;
                }
                else
                {
                    expectedValue = sum >= filtered.Count - sum ? 1 : 0;
                }

                filtered = filtered.Where(x => x[index] == expectedValue).ToList();
                if (filtered.Count == 1)
                {
                    return filtered[0].IntValue;
                }
            }

            return -1;
        }

        public static int GetComplementary(int value, int bitCount)
        {
            int mask = (1 << bitCount) - 1;
            value = ~value & mask;

            return value;
        }

        public static int GetGammaRate(IReadOnlyList<int> values, int bitCount)
        {
            var oneCounts = CountOnBitsPerColumn(values, bitCount);

            return oneCounts.Aggregate(0, (result, i) => (result << 1) + (i > values.Count / 2 ? 1 : 0));
        }

        private static int[] CountOnBitsPerColumn(IEnumerable<int> values, int bitCount)
        {
            int[] oneCounts = new int[bitCount];
            foreach (var value in values)
            {
                CountOnes(oneCounts, value, bitCount);
            }

            return oneCounts;
        }

        private static void CountOnes(int[] oneCounts, int value, int bitCount)
        {
            for (int i = 0; i < bitCount; i++)
            {
                oneCounts[i] += (value >> (bitCount - i - 1)) & 1;
            }
        }

        public class ReportInfo
        {
            public readonly string TextValue;
            public readonly int IntValue;
            public int this[int i] => (IntValue >> i) & 0x1;

            public ReportInfo(string textValue)
            {
                TextValue = textValue;
                IntValue = Convert.ToInt32(textValue, 2);
            }

            public override string ToString()
            {
                return TextValue;
            }
        }
    }
}