using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AdventOfCode2021
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var referenceType = typeof(Day1);
            int currentDay = FindLastDay(referenceType);

            Console.WriteLine("Running Day" + currentDay);
            
            var stopwatch = Stopwatch.StartNew();
            var lines = File.ReadAllLines($"Input/day{currentDay}.txt");

            var exerciseType = referenceType.Assembly.GetType($"{referenceType.Namespace}.Day{currentDay}");
            var exercise = (Exercise)Activator.CreateInstance(exerciseType);

            var result = exercise.ExecutePart1(lines);
            Console.WriteLine($"Executed in {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Result: {result}");
            stopwatch.Restart();
            var advancedResult = exercise.ExecutePart2(lines);
            Console.WriteLine($"Advanced executed in {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Result: {advancedResult}");
        }

        private static int FindLastDay(Type referenceType)
        {
            return Enumerable.Range(1, 24).Last(i => referenceType.Assembly.GetType($"{referenceType.Namespace}.Day{i}") != null);
        }
    }
}