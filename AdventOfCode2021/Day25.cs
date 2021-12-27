using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode2021.Utils;

namespace AdventOfCode2021 {
    public class Day25 : Exercise {
        public long ExecutePart1(string[] lines) {
            Area area = Area.Parse(lines);

            int i = 1;
            while (area.Step()) {
                i++;
            }

            // string plot = area.Plot();
            return i;
        }

        public long ExecutePart2(string[] lines) {
            return -2;
        }


        public class SeaCucumber {
            public IntVector2 Position;

            public SeaCucumber(IntVector2 position) {
                Position = position;
            }
        }

        public class Area {
            public List<SeaCucumber>[] EastBoundSeaCucumbers;
            public List<SeaCucumber>[] SouthBoundSeaCucumbers;
            private HashSet<IntVector2> AllSeaCucumbers;

            public readonly int Width;
            public readonly int Height;

            public Area(List<SeaCucumber>[] eastBoundSeaCucumbers, List<SeaCucumber>[] southBoundSeaCucumbers, int width,
                        int height) {
                EastBoundSeaCucumbers = eastBoundSeaCucumbers;
                SouthBoundSeaCucumbers = southBoundSeaCucumbers;

                AllSeaCucumbers = EastBoundSeaCucumbers.SelectMany(x => x).Select(x => x.Position)
                                                       .Concat(SouthBoundSeaCucumbers.SelectMany(x => x).Select(x => x.Position))
                                                       .ToHashSet();
                Width = width;
                Height = height;
            }

            public bool Step() {
                bool foundMove = false;
                for (int y = 0; y < Height; y++) {
                    List<SeaCucumber> seaCucumbers = EastBoundSeaCucumbers[y];
                    List<SeaCucumber> movingCucumbers = seaCucumbers
                                                  .Where(s => !AllSeaCucumbers.Contains(new IntVector2((s.Position.X + 1) % Width, y)))
                                                  .ToList();
                    foundMove |= movingCucumbers.Count > 0;
                    foreach (SeaCucumber seaCucumber in movingCucumbers) {
                        int nextPosition = (seaCucumber.Position.X + 1) % Width;
                        AllSeaCucumbers.Remove(seaCucumber.Position);
                        seaCucumber.Position = new IntVector2(nextPosition, y);
                        AllSeaCucumbers.Add(seaCucumber.Position);
                        if (nextPosition == 0) {
                            seaCucumbers.RemoveAt(seaCucumbers.Count - 1);
                            seaCucumbers.Insert(0, seaCucumber);
                            break;
                        }
                    }
                }

                for (int x = 0; x < Width; x++) {
                    List<SeaCucumber> seaCucumbers = SouthBoundSeaCucumbers[x];
                    List<SeaCucumber> movingCucumbers = seaCucumbers
                                                        .Where(s => !AllSeaCucumbers.Contains(new IntVector2(x, (s.Position.Y + 1) % Height)))
                                                        .ToList();
                    foundMove |= movingCucumbers.Count > 0;
                    foreach (SeaCucumber seaCucumber in movingCucumbers) {
                        int nextPosition = (seaCucumber.Position.Y + 1) % Height;
                        AllSeaCucumbers.Remove(seaCucumber.Position);
                        seaCucumber.Position = new IntVector2(x, nextPosition);
                        AllSeaCucumbers.Add(seaCucumber.Position);
                        if (nextPosition == 0) {
                            seaCucumbers.RemoveAt(seaCucumbers.Count - 1);
                            seaCucumbers.Insert(0, seaCucumber);
                            break;
                        }
                    }
                }

                return foundMove;
            }

            public string Plot() {
                char[,] map = new char[Width, Height];
                for (int y = 0; y < Height; y++) {
                    for (int x = 0; x < Width; x++) {
                        map[x, y] = '.';
                    }
                }

                foreach (SeaCucumber seaCucumber in EastBoundSeaCucumbers.SelectMany(x => x)) {
                    map[seaCucumber.Position.X, seaCucumber.Position.Y] = '>';
                }

                foreach (SeaCucumber seaCucumber in SouthBoundSeaCucumbers.SelectMany(x => x)) {
                    map[seaCucumber.Position.X, seaCucumber.Position.Y] = 'v';
                }

                StringBuilder stringBuilder = new StringBuilder();
                for (int y = 0; y < Height; y++) {
                    for (int x = 0; x < Width; x++) {
                        stringBuilder.Append(map[x, y]);
                    }
                    stringBuilder.AppendLine();
                }

                return stringBuilder.ToString();
            }

            public static Area Parse(string[] lines) {
                List<SeaCucumber>[] eastBoundSeaCucumbers =
                    Enumerable.Range(0, lines.Length).Select(x => new List<SeaCucumber>()).ToArray();
                List<SeaCucumber>[] southBoundSeaCucumbers =
                    Enumerable.Range(0, lines[0].Length).Select(x => new List<SeaCucumber>()).ToArray();

                for (int y = 0; y < lines.Length; y++) {
                    string line = lines[y];
                    for (int x = 0; x < line.Length; x++) {
                        if (line[x] == '>') {
                            eastBoundSeaCucumbers[y].Add(new SeaCucumber(new IntVector2(x, y)));
                        } else if (line[x] == 'v') {
                            southBoundSeaCucumbers[x].Add(new SeaCucumber(new IntVector2(x, y)));
                        }
                    }
                }

                return new Area(eastBoundSeaCucumbers, southBoundSeaCucumbers, lines[0].Length, lines.Length);
            }
        }
    }
}
