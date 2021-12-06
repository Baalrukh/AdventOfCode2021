using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2021 {
    public class Day6 : Exercise
    {
        private const int Part1Duration = 80;
        private const int Part2Duration = 256;
        private const int BirthCycleDuration = 7;
        private const int DelayBeforeBirth = 2;

        public long ExecutePart1(string[] lines)
        {
            return GetFishCountAfterDuration(lines, Part1Duration);
        }

        public long ExecutePart2(string[] lines)
        {

            return GetFishCountAfterDuration(lines, Part2Duration);
        }

        private static long GetFishCountAfterDuration(string[] lines, int duration)
        {
            var list = lines.Single().Split(',').Select(int.Parse).ToList();
            var fishCountByAge = list.GroupBy(x => x)
                .ToDictionary(x => x.Key, x => x.Count());

            long[] population = new long[BirthCycleDuration];
            foreach (var pair in fishCountByAge)
            {
                population[pair.Key] = pair.Value;
            }

            var ecoSystem = new FishEcoSystem(DelayBeforeBirth, BirthCycleDuration, population);
            ecoSystem.Evolve(duration);

            return ecoSystem.TotalFishCount;
        }

        internal class FishEcoSystem
        {
            private readonly int _delayBeforeBirthCycle;
            private readonly int _birthCycleDuration;

            private readonly long[] _population;

            public FishEcoSystem(int delayBeforeBirthCycle, int birthCycleDuration, long[] population)
            {
                _delayBeforeBirthCycle = delayBeforeBirthCycle;
                _birthCycleDuration = birthCycleDuration;
                _population = new long[birthCycleDuration + delayBeforeBirthCycle];
                for (int i = 0; i < birthCycleDuration; i++)
                {
                    _population[i] = population[i];
                }
            }

            public void Evolve(int duration)
            {
                for (int d = 0; d < duration; d++)
                {
                    long birthing = _population[0];
                    for (int i = 0; i < _delayBeforeBirthCycle + _birthCycleDuration - 1; i++)
                    {
                        _population[i] = _population[i + 1];
                    }

                    _population[_birthCycleDuration - 1] += birthing;
                    _population[_birthCycleDuration + _delayBeforeBirthCycle - 1] = birthing;
                }
            }

            public IReadOnlyList<long> Population => _population;

            public long TotalFishCount => _population.Sum();
        }
    }
}
