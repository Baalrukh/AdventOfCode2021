using System;
using System.Diagnostics;
using System.IO;

namespace AdventOfCode2021
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var stopwatch = Stopwatch.StartNew();
            var lines = File.ReadAllLines("Input/day1.txt");
            var result = Day1.Execute(lines);
            Console.WriteLine($"Executed in {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Result: {result}");
            stopwatch.Restart();
            var advancedResult = Day1.ExecuteAdvanced(lines);
            Console.WriteLine($"Advanced executed in {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Result: {advancedResult}");
        }
    }
}