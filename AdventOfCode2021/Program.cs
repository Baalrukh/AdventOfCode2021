using System;
using System.Diagnostics;
using System.IO;

namespace AdventOfCode2021
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            const int currentDay = 2;

            var stopwatch = Stopwatch.StartNew();
            var lines = File.ReadAllLines($"Input/day{currentDay}.txt");

            var referenceType = typeof(Day1);
            var exerciseType = referenceType.Assembly.GetType($"{referenceType.Namespace}.Day{currentDay}");
            var exercise = (Exercise)Activator.CreateInstance(exerciseType);

            var result = exercise.Execute(lines);
            Console.WriteLine($"Executed in {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Result: {result}");
            stopwatch.Restart();
            var advancedResult = exercise.ExecuteAdvanced(lines);
            Console.WriteLine($"Advanced executed in {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Result: {advancedResult}");
        }
    }
}