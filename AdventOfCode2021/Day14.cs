using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2021 {
    public class Day14 : Exercise
    {
        private const int Part1IterationCount = 10;
        private const int Part2IterationCount = 40;

        public long ExecutePart1(string[] lines)
        {
            return Execute(lines, Part1IterationCount);
        }

        public long ExecutePart2(string[] lines) {
            return Execute(lines, Part2IterationCount);
        }

        private static long Execute(string[] lines, int iterationCount)
        {
            var charCounts = GetCharCounts_new(lines, iterationCount);
            return charCounts[charCounts.Count - 1].Count - charCounts[0].Count;
        }

        private static List<CharInfo> GetCharCounts_new(string[] lines, int iterationCount)
        {
            var molecule = lines[0];
            var creationMap = ParseCreationMap(lines);

            var occurrences = GetPairOccurrences(creationMap, molecule, iterationCount);

            var valueTuples = occurrences.Select(x => (Pair:x.Key[1], Count:x.Value)).ToList();
            valueTuples.Add((molecule[0], 1));
            return valueTuples.GroupBy(x => x.Item1)
                .Select(x => new CharInfo(x.Key, x.Sum(c => c.Count)))
                .OrderBy(x => x.Count)
                .ToList();
        }

        private static Dictionary<string, long> GetPairOccurrences(Dictionary<string, (string, string)> creationMap, string molecule, int iterationCount)
        {
            var occurrences = creationMap.Keys.ToDictionary(x => x, x => 0L);
            for (int i = 0; i < molecule.Length - 1; i++)
            {
                string key = $"{molecule[i]}{molecule[i + 1]}";
                occurrences[key]++;
            }

            for (int i = 0; i < iterationCount; i++)
            {
                var newOccurrences = creationMap.Keys.ToDictionary(x => x, x => 0L);

                foreach (var pair in occurrences.Where(x => x.Value > 0))
                {
                    var (key1, key2) = creationMap[pair.Key];
                    newOccurrences[key1] += pair.Value;
                    newOccurrences[key2] += pair.Value;
                }

                occurrences = newOccurrences;
            }

            return occurrences;
        }

        private static Dictionary<string, (string, string)> ParseCreationMap(string[] lines)
        {
            return lines.Skip(2)
                .Select(x => x.Split(new[] {" -> "}, StringSplitOptions.None))
                .ToDictionary(x => x[0], x => ($"{x[0][0]}{x[1][0]}", $"{x[1][0]}{x[0][1]}"));
        }

        public readonly struct CharInfo
        {
            public readonly char Char;
            public readonly long Count;

            public CharInfo(char c, long count)
            {
                Char = c;
                Count = count;
            }
        }

    }
}
