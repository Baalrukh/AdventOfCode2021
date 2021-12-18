using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AdventOfCode2021.Utils;

namespace AdventOfCode2021 {
    public class Day17 : Exercise {
        public long ExecutePart1(string[] lines) {
            var targetArea = ParseTargetArea(lines[0]);

            int vy0 = 0;

            int bestVy0 = -1;
            while (vy0 < -targetArea.MinY + 1) {
                int y = 0;
                int t = vy0 * 2 + 1; // time to reach back y = 0 on the descent
                while (y > targetArea.MaxY) {
                    y = GetYPos(0, vy0, t);
                    t++;
                }

                if (y >= targetArea.MinY) {
                    bestVy0 = vy0;
                }
                vy0++;
            }

            return GetYPos(0, bestVy0, bestVy0);
        }

        public long ExecutePart2(string[] lines) {
            var targetArea = ParseTargetArea(lines[0]);
            // var dictionary = Enumerable.Range(0, targetArea.MaxX).Select(vx => (vx, GetTimesInRange(vx, targetArea))).Where(x => x.Item2 != null)
            //     .ToDictionary(x => x.vx, x => x.Item2);
            //
            // int minVy = targetArea.MinY;
            //
            // int foundCount = 0;
            // int vy0 = minVy;
            // while (vy0 < -targetArea.MinY + 1) {
            //     var count = dictionary.Where(rangePair => CanReachArea(vy0, rangePair.Value, targetArea))
            //         .Select(rangePair => (rangePair.Key, vy0))
            //         .Count();
            //     foundCount += count;
            //
            //     vy0++;
            // }
            //
            //
            // return foundCount;

            
            // Brute force wins, even with large upper bounds :(
            List<IntVector2> foundSpeeds = new List<IntVector2>();
            int foundCount = 0;
            for (int vy = targetArea.MinY; vy < 1000; vy++) {
                for (int vx = 0; vx <= targetArea.MaxX; vx++) {
                    if ((vx == 6) && (vy == 9)) {
                        int d = 0;
                    }
                    for (int t = 0; t < 1000; t++) {
                        int xPos = GetXPos(0, vx, t);
                        if (xPos > targetArea.MaxX) {
                            break;
                        }

                        if (xPos < targetArea.MinX) {
                            continue;
                        }

                        int yPos = GetYPos(0, vy, t);
                        if (yPos < targetArea.MinY) {
                            break;
                        }

                        if (yPos <= targetArea.MaxY) {
                            foundSpeeds.Add(new IntVector2(vx, vy));
                            foundCount++;
                            break;
                        }
                    }
                    
                }
            }
            return foundCount;
        }

        // private bool CanReachArea(int vy0, Range timeRangeInAreaForX, TargetArea targetArea) {
        //     for (int time = timeRangeInAreaForX.Min; time <= timeRangeInAreaForX.Max; time++) {
        //         int yPos = GetYPos(0, vy0, time);
        //         if (yPos < targetArea.MinY) {
        //             return false;
        //         }
        //
        //         if (yPos < targetArea.MaxY) {
        //             return true;
        //         }
        //     }
        //
        //     return false;
        // }

        private Range GetTimesInRange(int vx, TargetArea targetArea) {
            int x = 0;
            int t = 0;
            while ((x < targetArea.MinX) && (vx > 0)) {
                x += vx;
                vx--;
                t++;
            }

            if (vx == 0) {
                return null;
            }

            int min = t;
            while ((x <= targetArea.MaxX) && (vx > 0)) {
                x += vx;
                vx--;
                t++;
            }

            return new Range(min, vx > 0 ? t : int.MaxValue);
        }

        public class Range {
            public readonly int Min;
            public readonly int Max;

            public Range(int min, int max) {
                Min = min;
                Max = max;
            }
        }

        private double? GetTimeToY(int vy0, int targetY) {
            // int a = 1;
            int b = 2 * vy0 + 1;
            int c = 2 * targetY; // - 2 * y0
            int delta = b * b - 4 * c; // * a 
            if (delta < 0) {
                return null;
            }

            double sqrt = Math.Sqrt(delta);

            // double tMax0 = (-b - sqrt) / 2; // negative one
            double tMax1 = (-b + sqrt) / 2;
            return tMax1;
        }
        
        public static int GetXPos(int x0, int vx0, int time)
        {
            time = Math.Min(time, vx0);
            return x0 + time * vx0 - (time - 1) * time / 2;
        }

        public static int GetYPos(int y0, int vy0, int time)
        {
            return y0 + time * vy0 - (time - 1) * time / 2;
        }


        public static TargetArea ParseTargetArea(string text)
        {
            var regex = new Regex("target area: x=(-?\\d+)\\.\\.(-?\\d+), y=(-?\\d+)\\.\\.(-?\\d+)");
            var match = regex.Match(text);
            int minX = int.Parse(match.Groups[1].Value);
            int maxX = int.Parse(match.Groups[2].Value);
            int minY = int.Parse(match.Groups[3].Value);
            int maxY = int.Parse(match.Groups[4].Value);
            return new TargetArea(minX, maxX, minY, maxY);
        }

        public struct TargetArea
        {
            public readonly int MinX;
            public readonly int MaxX;
            public readonly int MinY;
            public readonly int MaxY;

            public TargetArea(int minX, int maxX, int minY, int maxY)
            {
                MinX = minX;
                MaxX = maxX;
                MinY = minY;
                MaxY = maxY;
            }

            public bool Equals(TargetArea other)
            {
                return MinX == other.MinX && MaxX == other.MaxX && MinY == other.MinY && MaxY == other.MaxY;
            }

            public override bool Equals(object obj)
            {
                return obj is TargetArea other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = MinX;
                    hashCode = (hashCode * 397) ^ MaxX;
                    hashCode = (hashCode * 397) ^ MinY;
                    hashCode = (hashCode * 397) ^ MaxY;
                    return hashCode;
                }
            }
        }
    }
}
