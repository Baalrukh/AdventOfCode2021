using System.Collections.Generic;
using System.Linq;
using AdventOfCode2021.Utils;

namespace AdventOfCode2021 {
    public class Day9 : Exercise {
        public long ExecutePart1(string[] lines) {
            Map map = Map.Parse(lines);
            List<IntVector2> lowPoints = map.GetLowPoints().ToList();

            return lowPoints.Sum(x => map[x] + 1);
        }

        public long ExecutePart2(string[] lines) {
            Map map = Map.Parse(lines);
            List<IntVector2> lowPoints = map.GetLowPoints().ToList();

            return lowPoints.Select(x => new Basin(x).ComputeSize(map))
                .OrderByDescending(x => x).Take(3)
                .Aggregate(1, (res, value) => res * value);
        }

        public class Map {
            private const int MaxHeight = 10;
            private readonly int[,] _heights;
            private readonly int _width;
            private readonly int _height;


            private Map(int[,] heights, int width, int height) {
                _heights = heights;
                _width = width;
                _height = height;
            }

            public int this[IntVector2 position] => _heights[position.X + 1, position.Y + 1];

            public IEnumerable<IntVector2> GetLowPoints() {
                for (int y = 1; y < _height + 1; y++) {
                    for (int x = 1; x < _width + 1; x++) {
                        int centerHeight = _heights[x, y];
                        if ((_heights[x - 1, y] > centerHeight)
                         && (_heights[x + 1, y] > centerHeight)
                         && (_heights[x, y - 1] > centerHeight)
                         && (_heights[x, y + 1] > centerHeight)) {
                            yield return new IntVector2(x - 1, y - 1);
                        }
                    }
                }
            }

            public static Map Parse(string[] lines) {
                int width = lines[0].Length;
                int height = lines.Length;

                int[,] map = new int[width+2, height+2];
                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {
                        map[x + 1, y + 1] = lines[y][x] - '0';
                    }
                }

                for (int y = 0; y < height; y++) {
                    map[0, y + 1] = MaxHeight;
                    map[width + 1, y + 1] = MaxHeight;
                }

                for (int x = 0; x < width; x++) {
                    map[x + 1, 0] = MaxHeight;
                    map[x + 1, height + 1] = MaxHeight;
                }

                return new Map(map, width, height);
            }
        }

        public class Basin
        {
            private readonly IntVector2 _lowPoint;

            public Basin(IntVector2 lowPoint)
            {
                _lowPoint = lowPoint;
            }

            public int ComputeSize(Map map)
            {
                HashSet<IntVector2> allPoints = new HashSet<IntVector2>() {_lowPoint};
                Queue<IntVector2> points = new Queue<IntVector2>();
                points.Enqueue(_lowPoint);

                while (points.Count > 0)
                {
                    var newPoints = EnumerateNeighbors(points.Dequeue()).Where(p => map[p] < 9)
                        .Where(x => !allPoints.Contains(x))
                        .ToList();

                    foreach (var pt in newPoints)
                    {
                        allPoints.Add(pt);
                        points.Enqueue(pt);
                    }
                }

                return allPoints.Count;
            }

            private IEnumerable<IntVector2> EnumerateNeighbors(IntVector2 position)
            {
                yield return new IntVector2(position.X - 1, position.Y);
                yield return new IntVector2(position.X + 1, position.Y);
                yield return new IntVector2(position.X, position.Y - 1);
                yield return new IntVector2(position.X, position.Y + 1);
            }
        }
    }
}
