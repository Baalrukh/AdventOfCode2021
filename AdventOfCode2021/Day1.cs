using System.Linq;

namespace AdventOfCode2021
{
    public static class Day1
    {
        public static int Execute(string[] lines)
        {
            var depths = lines.Select(int.Parse).ToList();

            return Enumerable.Range(1, lines.Length - 1).Count(i => depths[i] > depths[i - 1]);
        }

        public static int ExecuteAdvanced(string[] lines)
        {
            var depths = lines.Select(int.Parse).ToList();

            return Enumerable.Range(3, lines.Length - 3).Count(i => depths[i] > depths[i - 3]);
        }
    }
}