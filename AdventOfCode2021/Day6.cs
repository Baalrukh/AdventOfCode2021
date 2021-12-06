using System.Linq;

namespace AdventOfCode2021 {
    public class Day6 : Exercise
    {
        private const int Part1Duration = 80;
        private const int Part2Duration = 256;

        public int ExecutePart1(string[] lines) {
            return GetFishCountAfterDuration(lines, Part1Duration);
        }

        public int ExecutePart2(string[] lines) {
            return GetFishCountAfterDuration(lines, Part2Duration);
        }

        private static int GetFishCountAfterDuration(string[] lines, int duration)
        {
            var list = lines.Single().Split(',').Select(int.Parse).ToList();
            var fishCountByAge = list.GroupBy(x => x)
                .ToDictionary(x => x.Key, x => x.Count());

            var ecoSystem = new FishEcoSystem(2, 7);
            var total = fishCountByAge.Sum(p => ecoSystem.GetFishCount(p.Key, duration) * p.Value);
            return total;
        }


        internal class FishEcoSystem
        {
            private readonly int _delayBeforeBirthCycle;
            private readonly int _birthCycleDuration;

            public FishEcoSystem(int delayBeforeBirthCycle, int birthCycleDuration)
            {
                _delayBeforeBirthCycle = delayBeforeBirthCycle;
                _birthCycleDuration = birthCycleDuration;
            }

            public int GetFishCount(int daysBeforeBirth, int dayCount)
            {
                int count = 1;
                while ((dayCount > 0) && (daysBeforeBirth < dayCount))
                {
                    count += GetFishCount(_birthCycleDuration - 1, dayCount - daysBeforeBirth - 1 - _delayBeforeBirthCycle);
                    dayCount -= _birthCycleDuration;
                }
                return count;
            }
        }

    }
}
