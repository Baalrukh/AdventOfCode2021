using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode2021.Utils;

namespace AdventOfCode2021 {
    public class Day23 : Exercise {
        private const int HomesXStartPos = 3;
        private const int HomeYBack = 3;
        private const int HomeYFront = 2;
        private const int CorridorY = 1;
        private static int[] CorridorXPositions = { 1, 2, 4, 6, 8, 10, 11 };
        private static int[] TravelCostPerFamily = { 1, 10, 100, 1000 };

        public long ExecutePart1(string[] lines) {
            PaddLines(lines);
            AmphipodMap map = new AmphipodMap(Map2D<char>.Parse(lines, c => c, () => '#'));

            List<Amphipod> amphipods = Enumerable.Range(0, 4).SelectMany(i => LocateAmphipods(map, i)).ToList();

            int cost = FindMovesToHome(amphipods, map, int.MaxValue / 2);
            return cost;
        }

        private void PaddLines(string[] lines) {
            int lineLength = lines[0].Length;
            for (int i = 1; i < lines.Length; i++) {
                while (lines[i].Length < lineLength) {
                    lines[i] += " ";
                }
            }
        }

        private int FindMovesToHome(List<Amphipod> amphipods, AmphipodMap map, int maxCost) {
            List<Amphipod> moveableAmphipods = amphipods.Where(x => x.CanMove(map)).ToList();
            if (moveableAmphipods.Count == 0) {
                if (amphipods.All(x => x.IsHome(map))) {
                    return 0;
                }
                return int.MaxValue / 2;
            }

            int bestCost = maxCost;
            foreach (Amphipod moveableAmphipod in moveableAmphipods) {
                List<AmphipodMove> moves = moveableAmphipod.EnumerateMoves(map).ToList();
                foreach (AmphipodMove move in moves) {
                    if (move.Cost > bestCost) {
                        continue;
                    }
                    move.Apply(map, moveableAmphipod);
                    int pathCost = FindMovesToHome(amphipods, map, bestCost);
                    move.UnApply(map, moveableAmphipod);

                    pathCost += move.Cost;
                    if (pathCost < bestCost) {
                        bestCost = pathCost;
                    }
                }
            }

            return bestCost;
        }


        private static IEnumerable<Amphipod> LocateAmphipods(AmphipodMap map2D, int index) {
            return map2D.Map.EnumeratePositions(c => c == 'A' + index)
                        .Select(pos => new Amphipod(AmphipodFamily.A + index, pos, HomesXStartPos + 2 * index));
        }

        public long ExecutePart2(string[] lines) {
            return -2;
        }

        public enum AmphipodFamily {
            A, B, C, D
        }

        public class Amphipod {
            public readonly AmphipodFamily Family;
            public IntVector2 Position;
            public char Letter => (char)(Family + 'A');

            private readonly int _homeX;

            public Amphipod(AmphipodFamily family, IntVector2 position, int homeX) {
                Family = family;
                Position = position;
                _homeX = homeX;
            }

            public bool IsHome(AmphipodMap map) {
                if (Position.X != _homeX) {
                    return false;
                }

                if (Position.Y == HomeYBack) {
                    return true;
                }
                if (Position.Y == HomeYFront) {
                    return map[Position.X, HomeYBack] == Letter;
                }

                return false;
            }

            public bool CanMove(AmphipodMap map) {
                if (IsHome(map)) {
                    return false;
                }

                if (Position.Y == CorridorY) {
                    return CanGoToHome(map);
                }

                if (Position.Y == HomeYBack) {
                    if (!map.IsFree(Position.X, HomeYFront)) {
                        return false;
                    }
                }
                
                return map.IsFree(Position.X - 1, CorridorY)
                    || map.IsFree(Position.X + 1, CorridorY);
            }

            private bool CanGoToHome(AmphipodMap map) {
                if (!map.IsHomeFree(Family)) {
                    return false;
                }

                return map.CanMoveHorizontally(_homeX, Position.X);
            }

            public IEnumerable<AmphipodMove> EnumerateMoves(AmphipodMap map) {
                if (Position.Y == CorridorY) {
                    int dstY = map.IsFree(_homeX, HomeYBack) ? HomeYBack : HomeYFront;
                    yield return new AmphipodMove(Family, Position, new IntVector2(_homeX, dstY));
                } else {
                    foreach (int x in CorridorXPositions.Where(x => x < Position.X).Reverse()) {
                        if (map.IsFree(x, CorridorY)) {
                            yield return new AmphipodMove(Family, Position, new IntVector2(x, CorridorY));
                        } else {
                            break;
                        }
                    }
                    foreach (int x in CorridorXPositions.Where(x => x > Position.X)) {
                        if (map.IsFree(x, CorridorY)) {
                            yield return new AmphipodMove(Family, Position, new IntVector2(x, CorridorY));
                        } else {
                            break;
                        }
                    }
                }
            }
        }

        public class AmphipodMap {
            public readonly Map2D<char> Map;

            public AmphipodMap(Map2D<char> map) {
                Map = map;
            }

            public bool IsHomeFree(AmphipodFamily family) {
                int x = HomesXStartPos + (int)family * 2;
                return (Map[x, HomeYFront] == '.')
                    && ((Map[x, HomeYBack] == '.') || (Map[x, HomeYBack] == (char)('A' + family)));
            }

            public char this[int x, int y] => Map[x, y];


            public bool CanMoveHorizontally(int dst, int src) {
                int deltaX = dst - src;
                int increment = deltaX > 0 ? 1 : -1;
                int x = src + increment;
                while (x != dst) {
                    if (this[x, CorridorY] != '.') {
                        return false;
                    }

                    x += increment;
                }

                return true;
            }

            public bool IsFree(int x, int y) {
                return this[x, y] == '.';
            }
        }

        public struct AmphipodMove {
            public readonly AmphipodFamily Family;
            public readonly IntVector2 Source;
            public readonly IntVector2 Destination;

            public AmphipodMove(AmphipodFamily family, IntVector2 source, IntVector2 destination) {
                Family = family;
                Source = source;
                Destination = destination;
            }

            public int Cost => (Destination - Source).ManhattanDistance * TravelCostPerFamily[(int)Family];

            public void Apply(AmphipodMap map, Amphipod amphipod) {
                char c = (char)(Family + 'A');
                if (map.Map[Source] != c) {
                    throw new Exception("Inconsistency detected - source");
                }
                if (!map.IsFree(Destination.X, Destination.Y)) {
                    throw new Exception("Inconsistency detected - destination");
                }

                map.Map[Source] = '.';
                map.Map[Destination] = c;
                amphipod.Position = Destination;
            }

            public void UnApply(AmphipodMap map, Amphipod amphipod) {
                char c = (char)(Family + 'A');
                if (map.Map[Destination] != c) {
                    throw new Exception("Inconsistency detected - source");
                }
                if (!map.IsFree(Source.X, Source.Y)) {
                    throw new Exception("Inconsistency detected - destination");
                }

                map.Map[Destination] = '.';
                map.Map[Source] = c;
                amphipod.Position = Source;
            }

            public override string ToString() {
                return $"{Family}: {Source} -> {Destination}";
            }
        }
    }
}
