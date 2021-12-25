using System;
using System.Linq;
using AdventOfCode2021.Utils;

namespace AdventOfCode2021 {
    public class Day20 : Exercise {
        public long ExecutePart1(string[] lines) {
            const int iterationCount = 2;
            return CountLitElementsAfterNIterations(lines, iterationCount);
        }

        private long CountLitElementsAfterNIterations(string[] lines, int iterationCount) {
            var zoomMapping = lines[0].Select(x => x == '#' ? 1 : 0).ToList();

            var map = Map2D<byte>.Parse(lines.Skip(2).ToArray(), c => c == '#' ? 1 : 0, () => 0);

            int margin = iterationCount * 2;
            var map2 = Map2D<int>.Create(map.Width + margin * 2, map.Height + margin * 2, p => 0, () => 0);

            map.CopyTo(map2, margin, margin);
            map = map2;

            // PlotMap(map);

            for (int i = 0; i < iterationCount; i++) {
                Console.WriteLine("Iteration " + i);
                var newMap = Map2D<int>.Create(map.Width, map.Height, p => 0, () => 0);

                for (int y = 0; y < map.Height; y++) {
                    for (int x = 0; x < map.Width; x++) {
                        int value = GetAreaValue(map, x, y);
                        var newPixel = zoomMapping[value];
                        newMap[x, y] = newPixel;
                    }
                }

                map = newMap;

                // PlotMap(map);
            }

            int count = 0;
            int minX = margin - iterationCount;
            int maxX = map.Width - margin + iterationCount;
            int minY = margin - iterationCount;
            int maxY = map.Height - margin + iterationCount;
            for (int y = minY; y < maxY; y++) {
                for (int x = minX; x < maxX; x++) {
                    if (map[x, y] == 1) {
                        count++;
                    }
                }
            }

            return count;
        }

        private static void PlotMap(Map2D<int> map)
        {
            Console.WriteLine("------------------");

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    Console.Write(map[x, y] == 1 ? '#' : '.');
                }

                Console.WriteLine();
            }
        }

        private static readonly IntVector2[] SampleOffsets = {
            new IntVector2(-1, -1), new IntVector2(0, -1), new IntVector2(1, -1),
            new IntVector2(-1, 0), new IntVector2(0, 0), new IntVector2(1, 0),
            new IntVector2(-1, 1), new IntVector2(0, 1), new IntVector2(1, 1),
        };

        private int GetAreaValue(Map2D<int> map2D, int x, int y)
        {
            var center = new IntVector2(x, y);
            int res = 0;
            foreach (var offset in SampleOffsets)
            {
                res = (res << 1) | map2D[center + offset];
            }

            return res;
        }

        public long ExecutePart2(string[] lines) {
            const int iterationCount = 50;
            return CountLitElementsAfterNIterations(lines, iterationCount);
        }
    }
}
