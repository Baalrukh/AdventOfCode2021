using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2021.Utils;

namespace AdventOfCode2021 {
    public class Day22 : Exercise
    {
        private const int Part1MinCoord = -50;
        private const int Part1MaxCoord = 50;

        public long ExecutePart1(string[] lines)
        {
            var cubeOperations = lines.Select(CubeOperation.Parse)
                .Where(x => x.IsWithin(Part1MinCoord, Part1MaxCoord)).ToList();

            return LightCubes(cubeOperations);
        }

        public long ExecutePart2(string[] lines) {
            var cubeOperations = lines.Select(CubeOperation.Parse).ToList();
            return LightCubes(cubeOperations);
        }

        private long LightCubes(List<CubeOperation> allOperations) {
            long onCubes = 0;
            
            int minZ = allOperations.Min(co => co.ZInterval.Start);
            int maxZ = allOperations.Max(co => co.ZInterval.End);

            var zIntervals = allOperations.Select(x => x.ZInterval).OrderBy(x => x.Start).ToList();

            int z = minZ;
            while (z <= maxZ) {
                var operationsOnZ = allOperations.Where(x => x.ZInterval.IsInside(z)).ToList();

                var nextIntervalStart = zIntervals.SkipWhile(x => x.Start <= z).Select(x => (int?)x.Start).FirstOrDefault().GetValueOrDefault(int.MaxValue);
                var currentIntervalEnd = operationsOnZ.Select(x => x.ZInterval.End).OrderBy(x => x).First();

                int rangeEnd = Math.Min(nextIntervalStart - 1, currentIntervalEnd);

                int bandWidth = rangeEnd - z + 1;
                long cubesOnSlice = ExecuteOnZSlice(operationsOnZ);
                onCubes += cubesOnSlice * bandWidth;
                z = rangeEnd + 1;
            }

            return onCubes;
        }

        private long ExecuteOnZSlice(List<CubeOperation> cubeOperations) {
            long onCubes = 0;
            
            int minY = cubeOperations.Min(co => co.YInterval.Start);
            int maxY = cubeOperations.Max(co => co.YInterval.End);

            var yIntervals = cubeOperations.Select(x => x.YInterval).OrderBy(x => x.Start).ToList();

            int y = minY;
            while (y <= maxY) {
                var operations = cubeOperations.Where(x => x.YInterval.IsInside(y)).ToList();

                var nextIntervalStart = yIntervals.SkipWhile(x => x.Start <= y).Select(x => (int?)x.Start).FirstOrDefault().GetValueOrDefault(int.MaxValue);
                if (operations.Count == 0) {
                    y = nextIntervalStart;
                    continue;
                }
                var currentIntervalEnd = operations.Select(x => x.YInterval.End).OrderBy(x => x).First();

                int rangeEnd = Math.Min(nextIntervalStart - 1, currentIntervalEnd);

                int bandWidth = rangeEnd - y + 1;
                long cubeCount = ExecuteOnYZRow(operations);

                onCubes += cubeCount * bandWidth;
                y = rangeEnd + 1;
            }

            return onCubes;
        }

        private long ExecuteOnYZRow(List<CubeOperation> cubeOperations) {
            IntervalGroup intervalGroup = new IntervalGroup();
            foreach (CubeOperation cubeOperation in cubeOperations) {
                cubeOperation.ApplyOnYZ(intervalGroup);
            }

            return intervalGroup.LengthSum;
        }


        public class CubeOperation
        {
            public readonly bool SetOn;
            public readonly Interval XInterval;
            public readonly Interval YInterval;
            public readonly Interval ZInterval;

            public CubeOperation(bool setOn, int minX, int maxX, int minY, int maxY, int minZ, int maxZ)
            {
                SetOn = setOn;
                XInterval = new Interval(minX, maxX);
                YInterval = new Interval(minY, maxY);
                ZInterval = new Interval(minZ, maxZ);
            }

            public override string ToString() {
                return (SetOn ? "ON" : "OFF") + $" {XInterval}{YInterval}{ZInterval}";
            }

            public static CubeOperation Parse(string line)
            {
                bool setOn = line.StartsWith("on");
                var values = line.Substring(setOn ? 3 : 4)
                    .Split(new[] {'x', 'y', 'z', '=', '.', ','}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse).ToList();
                return new CubeOperation(setOn, values[0], values[1], values[2], values[3], values[4], values[5]);
            }


            public void ApplyOnYZ(IntervalGroup intervalGroup) {
                if (SetOn) {
                    intervalGroup.Add(XInterval);
                } else {
                    intervalGroup.Remove(XInterval);
                }
            }

            public bool IsWithin(int min, int max) {
                return XInterval.IsWithin(min, max)
                    && YInterval.IsWithin(min, max)
                    && ZInterval.IsWithin(min, max);
            }
        }
    }
}
