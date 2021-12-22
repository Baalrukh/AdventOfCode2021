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
            HashSet<IntVector3> onCubes = new HashSet<IntVector3>();
            foreach (var line in lines)
            {
                ApplyLine(line, onCubes);
            }


            return onCubes.Count;
        }

        private void ApplyLine(string line, HashSet<IntVector3> onCubes)
        {
            int substringStart;
            Func<IntVector3, bool> cubesOperation;
            if (line.StartsWith("on"))
            {
                cubesOperation = onCubes.Add;
                substringStart = 3;
            }
            else
            {
                cubesOperation = onCubes.Remove;
                substringStart = 4;
            }

            var values = line.Substring(substringStart)
                .Split(new[] {'x', 'y', 'z', '=', '.', ','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse).ToList();

            if (values.Any(x => (x < Part1MinCoord) || (x > Part1MaxCoord)))
            {
                return;
            }

            for (int z = values[4]; z <= values[5]; z++)
            {
                for (int y = values[2]; y <= values[3]; y++)
                {
                    for (int x = values[0]; x <= values[1]; x++)
                    {
                        cubesOperation(new IntVector3(x, y, z));
                    }
                }
            }
        }

        public long ExecutePart2(string[] lines)
        {
            var cubeOperations = lines.Select(CubeOperation.Parse).ToList();
            int minZ = cubeOperations.Min(co => co.ZInterval.Start);
            int maxZ = cubeOperations.Max(co => co.ZInterval.End);
            int minY = cubeOperations.Min(co => co.YInterval.Start);
            int maxY = cubeOperations.Max(co => co.YInterval.End);

            long onCubes = 0;

            for (int z = minZ; z <= maxZ; z++)
            {
                if ((z % 100) == 0)
                {
                    Console.WriteLine(z);
                }

                // onCubes += ExecuteOnSlice(z, cubeOperations);
                var operations = cubeOperations.Where(co => co.ZInterval.IsInside(z)).ToList();
                if (operations.Count > 0)
                {
                    onCubes += ExecuteOnSlice2(z, operations);
                }
            }

            return onCubes;
        }

        private long ExecuteOnSlice2(int z, List<CubeOperation> cubeOperations)
        {
            int minY = cubeOperations.Min(co => co.YInterval.Start);
            int maxY = cubeOperations.Max(co => co.YInterval.End);

            long subCount = 0;
            for (int y = minY; y <= maxY; y++)
            {
                var operations = cubeOperations.Where(co => co.YInterval.IsInside(y)).ToList();
                if (operations.Count > 0)
                {
                    HashSet<int> sliceOnCubes = new HashSet<int>();
                    foreach (var cubeOperation in cubeOperations)
                    {
                        cubeOperation.ApplyOnYZ(sliceOnCubes);
                    }

                    subCount += sliceOnCubes.Count;
                }
            }

            return subCount;
        }

        private long ExecuteOnSlice(int z, List<CubeOperation> cubeOperations)
        {
            HashSet<IntVector2> sliceOnCubes = new HashSet<IntVector2>();
            foreach (var cubeOperation in cubeOperations)
            {
                cubeOperation.ApplyOnZ(z, sliceOnCubes);
            }

            return sliceOnCubes.Count;
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

            public static CubeOperation Parse(string line)
            {
                bool setOn = line.StartsWith("on");
                var values = line.Substring(setOn ? 3 : 4)
                    .Split(new[] {'x', 'y', 'z', '=', '.', ','}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse).ToList();
                return new CubeOperation(setOn, values[0], values[1], values[2], values[3], values[4], values[5]);
            }

            public void ApplyOnZ(int z, HashSet<IntVector2> sliceOnCubes)
            {
                if (!ZInterval.IsInside(z))
                {
                    return;
                }

                Func<IntVector2, bool> cubesOperation = SetOn ? (Func<IntVector2, bool>)sliceOnCubes.Add : sliceOnCubes.Remove;

                for (int y = YInterval.Start; y <= YInterval.End; y++)
                {
                    for (int x = XInterval.Start; x <= XInterval.End; x++)
                    {
                        cubesOperation(new IntVector2(x, y));
                    }
                }
            }

            public void ApplyOnYZ(HashSet<int> sliceOnCubes)
            {
                // if (!ZInterval.IsInside(z) || !YInterval.IsInside(y))
                // {
                //     return;
                // }

                if (SetOn)
                {
                    for (int x = XInterval.Start; x <= XInterval.End; x++)
                    {
                        sliceOnCubes.Add(x);
                    }
                }
                else
                {
                    for (int x = XInterval.Start; x <= XInterval.End; x++)
                    {
                        sliceOnCubes.Remove(x);
                    }
                }
                // Func<int, bool> cubesOperation = SetOn ? (Func<int, bool>)sliceOnCubes.Add : sliceOnCubes.Remove;
                // for (int x = XInterval.Start; x <= XInterval.End; x++)
                // {
                //     cubesOperation(x);
                // }
            }
        }

        public readonly struct Interval
        {
            public readonly int Start;
            public readonly int End;

            public Interval(int start, int end)
            {
                Start = start;
                End = end;
            }

            public bool IsInside(int value)
            {
                return (Start <= value) && (value <= End);
            }

            public override string ToString()
            {
                return $"[{Start}/{End}]";
            }
        }

        public class LineIntervals
        {
            private List<Interval> _intervals = new List<Interval>();

            public void Add(Interval interval)
            {
                if (_intervals.Count == 0)
                {
                    _intervals.Add(interval);
                    return;
                }

                var startIndex = _intervals.FindIndex(x => x.End >= interval.Start);
                var endIndex = _intervals.FindIndex(x => x.Start <= interval.End);

                if (startIndex == -1) // after
                {
                    _intervals.Add(interval);
                }
                else if (endIndex == -1) // before
                {
                    _intervals.Insert(0, interval);
                }
                else
                {
                    if (startIndex == endIndex)
                    {

                    }
                    var newInterval = new Interval(_intervals[startIndex].Start, _intervals[endIndex].End);
                    _intervals.RemoveRange(startIndex, endIndex);
                    _intervals.Insert(startIndex, newInterval);
                }
            }

            public override string ToString()
            {
                return string.Join(",", _intervals);
            }
        }
    }
}
