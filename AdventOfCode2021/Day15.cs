using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode2021.Utils;

namespace AdventOfCode2021 {
    public class Day15 : Exercise {
        private static readonly IntVector2 UP = new IntVector2(0, -1);
        private static readonly IntVector2 RIGHT = new IntVector2(1, 0);
        private static readonly IntVector2 DOWN = new IntVector2(0, 1);
        private static readonly IntVector2 LEFT = new IntVector2(-1, 0);
        private static readonly IntVector2[] DIRECTIONS = new[] { UP, RIGHT, DOWN, LEFT };

        public long ExecutePart1(string[] lines) {
            Map2D<Cell> map = Map2D<Cell>.Parse(lines, x => new Cell(x, int.MaxValue), () => new Cell(0, -1));
            map[0, 0].BestTravelCost = 0;

            return FindBestCost(map);
        }

        private static long FindBestCost(Map2D<Cell> map) {
            Queue<IntVector2> positionsToProcess = new Queue<IntVector2>();
            positionsToProcess.Enqueue(new IntVector2(0, 0));
            HashSet<IntVector2> processedPoints = new HashSet<IntVector2>();

            while (positionsToProcess.Count > 0) {
                IntVector2 position = positionsToProcess.Dequeue();
                int currentCost = map[position].BestTravelCost;

                foreach (IntVector2 direction in DIRECTIONS) {
                    IntVector2 nextPosition = position + direction;
                    Cell nextCell = map[nextPosition];
                    int cost = nextCell.Value + currentCost;
                    if (cost < nextCell.BestTravelCost) {
                        nextCell.BestTravelCost = cost;
                        nextCell.BestIncomingDirection = direction;
                        positionsToProcess.Enqueue(nextPosition);
                        processedPoints.Remove(nextPosition);
                    }
                }
            }

            return map[map.Width - 1, map.Height - 1].BestTravelCost;
        }

        public long ExecutePart2(string[] lines) {
            return 0;
        }


        public class Cell {
            public readonly int Value;
            public int BestTravelCost;
            public IntVector2 BestIncomingDirection;

            public Cell(int value, int bestTravelCost) {
                Value = value;
                BestTravelCost = bestTravelCost;
            }
        }
    }
}
