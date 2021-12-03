using System.Linq;

namespace AdventOfCode2021
{
    public class Day1 : Exercise
    {
        public int ExecutePart1(string[] lines)
        {
            var depths = lines.Select(int.Parse).ToList();

            return Enumerable.Range(1, lines.Length - 1).Count(i => depths[i] > depths[i - 1]);
        }

        public int ExecutePart2(string[] lines)
        {
            var depths = lines.Select(int.Parse).ToList();

            return Enumerable.Range(3, lines.Length - 3).Count(i => depths[i] > depths[i - 3]);
        }
    }
}