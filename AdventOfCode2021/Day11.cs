using System.Collections.Generic;
using System.Linq;
using AdventOfCode2021.Utils;

namespace AdventOfCode2021 {
    public class Day11 : Exercise {
        private const int StepCount = 100;
        
        public long ExecutePart1(string[] lines) {
            CaveMap caveMap = CaveMap.Parse(lines);

            return Enumerable.Range(0, StepCount).Sum(x => caveMap.FlashAllOctopuses());
        }

        public long ExecutePart2(string[] lines) {
            CaveMap caveMap = CaveMap.Parse(lines);
            int i = 1;
            while (true) {
                if ((caveMap.FlashAllOctopuses() == 100)
                    && caveMap.AreAllReset()) {
                    return i;
                }

                i++;
            }
        }


        public class CaveMap {
            private Map2D<Octopus> _map;

            public CaveMap(Map2D<Octopus> map) {
                _map = map;
            }

            public static CaveMap Parse(string[] lines) {
                return new CaveMap(Map2D<Octopus>.Parse(lines, x => new Octopus(x), () => new Octopus(int.MinValue)));
            }

            public override string ToString() {
                return _map.ToString();
            }

            public void Energize() {
                for (int y = 0; y < _map.Height; y++) {
                    for (int x = 0; x < _map.Width; x++) {
                        _map[x, y].Energize();
                    }
                }
            }

            public IEnumerable<IntVector2> GetFlashingOctopusPositions() {
                for (int y = 0; y < _map.Height; y++) {
                    for (int x = 0; x < _map.Width; x++) {
                        if (_map[x, y].Energize()) {
                            yield return new IntVector2(x, y);
                        }
                    }
                }
            }

            public int FlashAllOctopuses() {
                var flashingOctopuses = new Queue<IntVector2>(GetFlashingOctopusPositions());

                List<IntVector2> allFlashingPositions = new List<IntVector2>(flashingOctopuses);

                while (flashingOctopuses.Count > 0) {
                    foreach (IntVector2 position in GetNeighbours(flashingOctopuses.Dequeue())) {
                        if (_map[position].Energize()) {
                            allFlashingPositions.Add(position);
                            flashingOctopuses.Enqueue(position);
                        }
                    }
                }

                foreach (IntVector2 position in allFlashingPositions) {
                    _map[position].Reset();
                }

                return allFlashingPositions.Count;
            }


            public bool AreAllReset() {
                for (int y = 0; y < _map.Height; y++) {
                    for (int x = 0; x < _map.Width; x++) {
                        if (!_map[x, y].IsReset) {
                            return false;
                        }
                    }
                }

                return true;
            }

            private IEnumerable<IntVector2> GetNeighbours(IntVector2 position) {
                yield return new IntVector2(position.X, position.Y - 1);
                yield return new IntVector2(position.X + 1, position.Y - 1);
                yield return new IntVector2(position.X + 1, position.Y);
                yield return new IntVector2(position.X + 1, position.Y + 1);
                yield return new IntVector2(position.X, position.Y + 1);
                yield return new IntVector2(position.X - 1, position.Y + 1);
                yield return new IntVector2(position.X - 1, position.Y);
                yield return new IntVector2(position.X - 1, position.Y - 1);
            }
        }

        public class Octopus {
            public const int FlashingLevel = 10;
            private int _energyLevel;

            public Octopus(int energyLevel) {
                _energyLevel = energyLevel;
            }

            public override string ToString() {
                return _energyLevel.ToString();
            }

            public bool Energize() {
                if (CanFlash()) {
                    return false;
                }
                _energyLevel++;
                return CanFlash();
            }

            public bool CanFlash() {
                return _energyLevel >= FlashingLevel;
            }

            public void Reset() {
                _energyLevel = 0;
            }

            public bool IsReset => _energyLevel == 0;
        }
    }
}
