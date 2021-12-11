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
            private readonly Map2D<int> _map;

            public Map(Map2D<int> map) {
                _map = map;
            }

            public int this[IntVector2 position] => _map[position];
            
            public IEnumerable<IntVector2> GetLowPoints() {
                for (int y = 0; y < _map.Height; y++) {
                    for (int x = 0; x < _map.Width; x++) {
                        int centerHeight = _map[x, y];
                        if ((_map[x - 1, y] > centerHeight)
                         && (_map[x + 1, y] > centerHeight)
                         && (_map[x, y - 1] > centerHeight)
                         && (_map[x, y + 1] > centerHeight)) {
                            yield return new IntVector2(x, y);
                        }
                    }
                }
            }

            public static Map Parse(string[] lines) {
                return new Map(Map2D<int>.Parse(lines, x => x, () => MaxHeight));
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
