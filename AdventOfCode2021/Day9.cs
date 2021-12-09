using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode2021.Utils;

namespace AdventOfCode2021 {
    public class Day9 : Exercise {
        public long ExecutePart1(string[] lines) {
            Map map = Map.Parse(lines);
            List<IntVector2> lowPoints = map.GetLowPoints().ToList();

            return lowPoints.Sum(x => map.GetHeight(x) + 1);
        }

        public long ExecutePart2(string[] lines) {
            return -2;
        }

        public class Map {
            private const int MaxHeight = 10;
            private int[,] _heights;
            private readonly int _width;
            private int _height;


            public Map(int[,] heights, int width, int height) {
                _heights = heights;
                _width = width;
                _height = height;
            }

            public int GetHeight(int x, int y) {
                // if ((x < 0) || (x >= _width)
                //             || (y < 0) || (y >= _height)) {
                //     return MaxHeight;
                // }

                return _heights[x + 1, y+ 1];
            }

            public int GetHeight(IntVector2 pos) {
                return _heights[pos.X + 1, pos.Y+ 1];
            }

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
    }
}
