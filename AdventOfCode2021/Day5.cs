using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode2021.Utils;

namespace AdventOfCode2021 {
    public class Day5 : Exercise {
        public int ExecutePart1(string[] lines) {
            return GetDangerousPointCountForHorizontalAndVertical(lines);
        }

        public int ExecutePart2(string[] lines) {
            return GetAllDangerousPointCount(lines);
        }

        public static int GetDangerousPointCountForHorizontalAndVertical(string[] lines) {
            return lines
                .Select(Line.Parse)
                .Where(x => x.IsHorizontal || x.IsVertical)
                .SelectMany(x => x.EnumeratePositions())
                .GroupBy(x => x)
                .Count(x => x.Count() >= 2);
        }

        public static int GetAllDangerousPointCount(string[] lines) {
            return lines
                .Select(Line.Parse)
                .SelectMany(x => x.EnumeratePositions())
                .GroupBy(x => x)
                .Count(x => x.Count() >= 2);
        }

        public static void PlotAllPoints(List<IntVector2> points, int width) {
            int[,] map = new int[width, width];
            foreach (IntVector2 point in points) {
                map[point.X, point.Y]++;
            }

            StringBuilder builder = new StringBuilder();
            for (int y = 0; y < width; y++) {
                for (int x = 0; x < width; x++) {
                    builder.Append(map[x, y] == 0 ? '.' : (char)(map[x, y] + '0'));
                }

                builder.AppendLine();
            }
            Console.WriteLine(builder.ToString());
            
        }
        
        public class Line {
            public readonly IntVector2 StartPosition;
            public readonly IntVector2 EndPosition;

            internal Line(int x0, int y0, int x1, int y1) {
                StartPosition = new IntVector2(x0, y0);
                EndPosition = new IntVector2(x1, y1);
            }
            
            public Line(IntVector2 startPosition, IntVector2 endPosition) {
                StartPosition = startPosition;
                EndPosition = endPosition;
            }

            public bool IsHorizontal => StartPosition.Y == EndPosition.Y;
            public bool IsVertical => StartPosition.X == EndPosition.X;
            public bool IsDiagonal {
                get {
                    IntVector2 vector = EndPosition - StartPosition;
                    return Math.Abs(vector.X) == Math.Abs(vector.Y);
                }
            }

            public IEnumerable<IntVector2> EnumeratePositions() {
                // if (!IsHorizontal && !IsVertical && !IsDiagonal) {
                //     yield break;
                // }
                var vector = EndPosition - StartPosition;
                int length = Math.Abs(vector.X == 0 ? vector.Y : vector.X);
                var increment = vector / length;
                IntVector2 position = StartPosition;
                yield return position;
                while (position != EndPosition) {
                    position += increment;
                    yield return position;
                }
            }
            
            protected bool Equals(Line other) {
                return StartPosition.Equals(other.StartPosition) && EndPosition.Equals(other.EndPosition);
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Line)obj);
            }

            public override int GetHashCode() {
                unchecked {
                    return (StartPosition.GetHashCode() * 397) ^ EndPosition.GetHashCode();
                }
            }

            public static Line Parse(string text) {
                int[] values = text.Split(new[] { ',', '-', '>', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => int.Parse(x)).ToArray();
                return new Line(new IntVector2(values[0], values[1]), new IntVector2(values[2], values[3]));
            }
        }
    }
}