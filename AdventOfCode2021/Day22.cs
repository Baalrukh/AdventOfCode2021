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
                .Where(x => x.IsWithin(-50, 50)).ToList();

            return LightCubes(cubeOperations);
        }

        public long ExecutePart2(string[] lines) {
            var cubeOperations = lines.Select(CubeOperation.Parse).ToList();
            return LightCubes(cubeOperations);
        }

        private long LightCubes(List<CubeOperation> cubeOperations) {
            long onCubes = 0;
            
            int minZ = cubeOperations.Min(co => co.ZInterval.Start);
            int maxZ = cubeOperations.Max(co => co.ZInterval.End);

            var zIntervals = cubeOperations.Select(x => x.ZInterval).OrderBy(x => x.Start).ToList();

            int z = minZ;
            while (z <= maxZ) {
                var operations = cubeOperations.Where(x => x.ZInterval.IsInside(z)).ToList();

                var nextIntervalStart = zIntervals.SkipWhile(x => x.Start <= z).Select(x => (int?)x.Start).FirstOrDefault().GetValueOrDefault(int.MaxValue);
                var currentIntervalEnd = operations.Select(x => x.ZInterval.End).OrderBy(x => x).First();

                int rangeEnd = Math.Min(nextIntervalStart - 1, currentIntervalEnd);

                int bandWidth = rangeEnd - z + 1;
                // onCubes += ExecuteOnSlice2(z, operations) * bandWidth;
                onCubes += ExecuteOnZSlice(operations) * bandWidth;
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
                onCubes += ExecuteOnYZRow(operations) * bandWidth;
                y = rangeEnd + 1;
            }

            return onCubes;
        }

        private long ExecuteOnYZRow(List<CubeOperation> cubeOperations) {
            LineIntervals lineIntervals = new LineIntervals();
            foreach (CubeOperation cubeOperation in cubeOperations) {
                cubeOperation.ApplyOnYZ(lineIntervals);
            }

            return lineIntervals.LengthSum;
        }


        private long ExecuteOnSlice2(int z, List<CubeOperation> cubeOperations)
        {
            long subCount = 0;

            LineIntervals impactedIntervals = new LineIntervals();
            foreach (Interval interval in cubeOperations.Select(x => x.YInterval)) {
                impactedIntervals.Add(interval);
            }

            foreach (Interval interval in impactedIntervals.Intervals) {
                for (int y = interval.Start; y <= interval.End; y++)
                {
                    var operations = cubeOperations.Where(co => co.YInterval.IsInside(y)).ToList();
                    if (operations.Count > 0)
                    {
                        LineIntervals lineIntervals = new LineIntervals();
                        foreach (var cubeOperation in operations)
                        {
                            cubeOperation.ApplyOnYZ(lineIntervals);
                        }

                        subCount += lineIntervals.LengthSum;
                    }
                }
            }

            //
            // int minY = cubeOperations.Min(co => co.YInterval.Start);
            // int maxY = cubeOperations.Max(co => co.YInterval.End);
            //
            // long subCount = 0;
            // for (int y = minY; y <= maxY; y++)
            // {
            //     var operations = cubeOperations.Where(co => co.YInterval.IsInside(y)).ToList();
            //     if (operations.Count > 0)
            //     {
            //         LineIntervals lineIntervals = new LineIntervals();
            //         foreach (var cubeOperation in operations)
            //         {
            //             cubeOperation.ApplyOnYZ(lineIntervals);
            //         }
            //
            //         subCount += lineIntervals.LengthSum;
            //     }
            // }

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

            public void ApplyOnYZ(LineIntervals lineIntervals)
            {
                // if (!ZInterval.IsInside(z) || !YInterval.IsInside(y))
                // {
                //     return;
                // }

                if (SetOn)
                {
                    lineIntervals.Add(XInterval);
                }
                else
                {
                    lineIntervals.Remove(XInterval);
                }
                // Func<int, bool> cubesOperation = SetOn ? (Func<int, bool>)sliceOnCubes.Add : sliceOnCubes.Remove;
                // for (int x = XInterval.Start; x <= XInterval.End; x++)
                // {
                //     cubesOperation(x);
                // }
            }

            public bool IsWithin(int min, int max) {
                return XInterval.IsWithin(min, max)
                    && YInterval.IsWithin(min, max)
                    && ZInterval.IsWithin(min, max);
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

            public int Length => End - Start + 1;

            public bool IsInside(int value)
            {
                return (Start <= value) && (value <= End);
            }

            public override string ToString()
            {
                return $"[{Start}/{End}]";
            }

            public bool IsWithin(int min, int max) {
                return (Start >= min) && (End <= max);
            }
        }

        public class LineIntervals
        {
            private readonly List<Interval> _intervals = new List<Interval>();
            public long LengthSum => _intervals.Sum(x => (long)x.Length);

            public IReadOnlyList<Interval> Intervals => _intervals;

            public void Add(Interval interval)
            {
                if (_intervals.Count == 0)
                {
                    _intervals.Add(interval);
                    return;
                }

                var startIndex = _intervals.FindIndex(x => x.End >= interval.Start);
                var endIndex = _intervals.FindLastIndex(x => x.Start <= interval.End);

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
                    if (startIndex == endIndex) {
                        var newInterval = new Interval(Math.Min(_intervals[startIndex].Start, interval.Start),
                            Math.Max(_intervals[startIndex].End, interval.End));
                        _intervals[startIndex] = newInterval;
                    } else if (endIndex < startIndex) {
                        _intervals.Insert(startIndex, interval);
                    } else {
                        var newInterval = new Interval(Math.Min(_intervals[startIndex].Start, interval.Start),
                            Math.Max(_intervals[endIndex].End, interval.End));
                        _intervals[startIndex] = newInterval;
                        _intervals.RemoveRange(startIndex + 1, endIndex - startIndex);
                    }
                }
            }

            public override string ToString()
            {
                return string.Join("", _intervals);
            }

            public void Remove(Interval interval) {
                if (_intervals.Count == 0)
                {
                    return;
                }
                var startIndex = _intervals.FindLastIndex(x => x.End < interval.Start);
                var endIndex = _intervals.FindIndex(x => x.Start > interval.End);
                if (endIndex == startIndex + 1) { // remove empty block between 2 intervals
                    return;
                }
                if (startIndex == -1) {
                    if (endIndex == 0) {
                        return; // block before first one
                    }
                }
                if (endIndex == -1) {
                    if (startIndex == _intervals.Count - 1) {
                        return; // block after last one
                    }
                    endIndex = _intervals.Count - 1;
                } else {
                    endIndex--;
                }
                startIndex++;
                
                
                if (startIndex == endIndex) {
                    Interval existingInterval = _intervals[startIndex];
                    if (interval.Start <= existingInterval.Start) {
                        if (interval.End >= existingInterval.End) {
                            _intervals.RemoveAt(startIndex);
                        } else {
                            _intervals[startIndex] = new Interval(interval.End + 1, existingInterval.End);
                        }
                    } else if (interval.End >= existingInterval.End) {
                        _intervals[startIndex] = new Interval(existingInterval.Start, interval.Start - 1);
                    }
                    else {
                        _intervals[startIndex] = new Interval(existingInterval.Start, interval.Start - 1);
                        _intervals.Insert(startIndex + 1, new Interval(interval.End + 1, existingInterval.End));
                    }
                }
                else {
                    if (endIndex < startIndex) {
                        int idsfas = 0;
                    }
                    _intervals[startIndex] = new Interval(_intervals[startIndex].Start, interval.Start - 1);
                    _intervals[endIndex] = new Interval(interval.End + 1, _intervals[endIndex].End);
                    _intervals.RemoveRange(startIndex + 1, endIndex - startIndex - 1);
                }
                
            }
        }
    }
}
