using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2021 {
    public class Day23 : Exercise {
        public long ExecutePart1(string[] lines) {
            AWorldState initialWorldState = new AWorldState(ParseHome(lines, 3), ParseHome(lines, 5),
                                                            ParseHome(lines, 7), ParseHome(lines, 9));

            List<AWorldState> queue = initialWorldState.EnumerateMoves().ToList();
            Dictionary<long, int> scoreByHash = new Dictionary<long, int>();
            queue.Sort((a, b) => b.Cost.CompareTo(a.Cost));
            while (!queue[queue.Count - 1].IsFinished()) {
                AWorldState worldState = queue[queue.Count - 1];
                queue.RemoveAt(queue.Count - 1);
                List<AWorldState> aWorldStates = worldState.EnumerateMoves().ToList();
                foreach (AWorldState state in aWorldStates) {
                    if (scoreByHash.TryGetValue(state.Hash, out var score)) {
                        if (state.Cost < score) {
                            InsertState(queue, state, scoreByHash);
                        }
                    } else {
                        InsertState(queue, state, scoreByHash);
                    }
                }
            }

            return queue[queue.Count - 1].Cost;
        }

        private void InsertState(List<AWorldState> queue, AWorldState state, Dictionary<long, int> scoreByHash) {
            Insert(queue, state);
            scoreByHash[state.Hash] = state.Cost;
        }


        private void Insert(List<AWorldState> states, AWorldState newState) {
            int index = states.FindIndex(x => x.Cost < newState.Cost);
            if (index == -1) {
                states.Add(newState);
            } else {
                states.Insert(index, newState);
            }
        }

        private AmphipodFamily[] ParseHome(string[] lines, int x) {
            AmphipodFamily[] home = new AmphipodFamily[2];
            home[0] = lines[2][x] - 'A' + AmphipodFamily.A;
            home[1] = lines[3][x] - 'A' + AmphipodFamily.A;
            return home;
        }

        public long ExecutePart2(string[] lines) {
            return -2;
        }


        public enum AmphipodFamily {
            None, A, B, C, D
        }

        private static readonly IReadOnlyDictionary<AmphipodFamily, int> CostPerFamily =
            new Dictionary<AmphipodFamily, int>() {
                { AmphipodFamily.A, 1 },
                { AmphipodFamily.B, 10 },
                { AmphipodFamily.C, 100 },
                { AmphipodFamily.D, 1000 },
            };

        public struct AWorldState {
            private static readonly IReadOnlyDictionary<AmphipodFamily, float> HomeIndices =
                new Dictionary<AmphipodFamily, float>() {
                    { AmphipodFamily.A, 1.5f },
                    { AmphipodFamily.B, 2.5f },
                    { AmphipodFamily.C, 3.5f },
                    { AmphipodFamily.D, 4.5f },
                };

            private const int CorridorLength = 7;
            public readonly AmphipodFamily[] Corridor;
            public readonly AmphipodFamily[] AHome;
            public readonly AmphipodFamily[] BHome;
            public readonly AmphipodFamily[] CHome;
            public readonly AmphipodFamily[] DHome;

            public readonly int Cost;
            public long Hash;

            private IEnumerable<AmphipodFamily> AllSlots => Corridor.Concat(AHome).Concat(BHome).Concat(CHome).Concat(DHome);

            public AWorldState(AmphipodFamily[] aHome, AmphipodFamily[] bHome, AmphipodFamily[] cHome, AmphipodFamily[] dHome) {
                Corridor = new AmphipodFamily[CorridorLength];
                AHome = aHome;
                BHome = bHome;
                CHome = cHome;
                DHome = dHome;
                Cost = 0;
                Hash = 0;
            }

            private AWorldState(AmphipodFamily[] corridor, AmphipodFamily[] aHome, AmphipodFamily[] bHome,
                                AmphipodFamily[] cHome, AmphipodFamily[] dHome, int cost) {
                Corridor = corridor;
                AHome = aHome;
                BHome = bHome;
                CHome = cHome;
                DHome = dHome;
                Cost = cost;
                Hash = 0;
            }

            private AWorldState(AWorldState other, int moveCost) {
                Corridor = CopyArray(other.Corridor);
                AHome = CopyArray(other.AHome);
                BHome = CopyArray(other.BHome);
                CHome = CopyArray(other.CHome);
                DHome = CopyArray(other.DHome);
                Cost = other.Cost + moveCost;
                Hash = 0;
            }

            private void ComputeHash() {
                Hash = AllSlots.Aggregate(0L, (res, family) => (res * 7) ^ (int)family);
            }

            private static AmphipodFamily[] CopyArray(AmphipodFamily[] otherArray) {
                int size = otherArray.Length;
                AmphipodFamily[] array = new AmphipodFamily[size];
                Array.Copy(otherArray, array, size);
                return array;
            }


            private AmphipodFamily[] GetHome(AmphipodFamily family) {
                switch (family) {
                    case AmphipodFamily.A: return AHome;
                    case AmphipodFamily.B: return BHome;
                    case AmphipodFamily.C: return CHome;
                    case AmphipodFamily.D: return DHome;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(family), family, null);
                }
            }

            public IEnumerable<AWorldState> EnumerateMoves() {
                for (int i = 0; i < 7; i++) {
                    AmphipodFamily amphipod = Corridor[i];
                    if (amphipod != AmphipodFamily.None) {
                        float homeIndex = HomeIndices[amphipod];
                        AmphipodFamily[] home = GetHome(amphipod);
                        if (IsHomeFree(home, amphipod) && CanGoHomeFromCorridor(i, homeIndex)) {
                            yield return MoveFromCorridorToHome(amphipod, i, homeIndex);
                        }
                    }
                }

                for (AmphipodFamily sourceHomeFamily = AmphipodFamily.A; sourceHomeFamily <= AmphipodFamily.D; sourceHomeFamily++) {
                    AmphipodFamily[] sourceHome = GetHome(sourceHomeFamily);
                    if (IsHomeFinished(sourceHome, sourceHomeFamily)) {
                        continue;
                    }
                    AmphipodFamily amphipod = sourceHome.FirstOrDefault(x => x != AmphipodFamily.None);
                    if (amphipod == AmphipodFamily.None) {
                        continue;
                    }

                    AmphipodFamily[] destinationHome = GetHome(amphipod);
                    float sourceHomeIndex = HomeIndices[sourceHomeFamily];
                    if ((amphipod != sourceHomeFamily) && IsHomeFree(destinationHome, amphipod) && CanGoHomeFromHome(sourceHomeIndex, HomeIndices[amphipod])) {
                        yield return MoveFromHomeToHome(amphipod, sourceHome, sourceHomeFamily, destinationHome);
                    } else {
                        for (int i = (int)sourceHomeIndex; i >= 0; i--) {
                            if (Corridor[i] != AmphipodFamily.None) {
                                break;
                            }

                            yield return MoveHomeToCorridor(amphipod, i, sourceHomeFamily);
                        }
                        for (int i = (int)sourceHomeIndex + 1; i < CorridorLength; i++) {
                            if (Corridor[i] != AmphipodFamily.None) {
                                break;
                            }
                            yield return MoveHomeToCorridor(amphipod, i, sourceHomeFamily);
                        }
                    }
                }
            }

            private AWorldState MoveHomeToCorridor(AmphipodFamily amphipod, int corriodrIndex, AmphipodFamily sourceHome) {
                float homeIndex = HomeIndices[sourceHome];
                int cost = GetCostHomeToCorridor(corriodrIndex, homeIndex, amphipod, sourceHome);

                AWorldState worldState = new AWorldState(this, cost);
                worldState.Corridor[corriodrIndex] = amphipod;
                AmphipodFamily[] home = worldState.GetHome(sourceHome);

                int index = Array.FindIndex(home, x => x == amphipod);
                home[index] = AmphipodFamily.None;
                worldState.ComputeHash();
                return worldState;
            }

            private AWorldState MoveFromHomeToHome(AmphipodFamily amphipod, AmphipodFamily[] sourceHome, AmphipodFamily sourceHomeFamily, AmphipodFamily[] destinationHome) {
                float sourceIndex = HomeIndices[amphipod];
                float dstIndex = HomeIndices[sourceHomeFamily];

                int src = Array.FindIndex(sourceHome, x => x != AmphipodFamily.None);
                int dst = Array.FindLastIndex(destinationHome, x => x == AmphipodFamily.None);

                int cost = GetMoveCostHomeToHome(amphipod, src, dst, sourceIndex, dstIndex);

                AWorldState state = new AWorldState(this, cost);
                state.GetHome(sourceHomeFamily)[src] = AmphipodFamily.None;
                state.GetHome(amphipod)[dst] = amphipod;
                state.ComputeHash();
                return state;
            }

            public static int GetMoveCostHomeToHome(AmphipodFamily amphipod, int indexInHomeSource,
                                                     int indexInHomeDest, float sourceIndex, float dstIndex) {
                int moveCount = (int)Math.Round(Math.Abs(sourceIndex - dstIndex) * 2);
                moveCount += indexInHomeSource + indexInHomeDest + 2;
                int cost = moveCount * CostPerFamily[amphipod];
                return cost;
            }


            private AWorldState MoveFromCorridorToHome(AmphipodFamily family, int index, float homeIndex) {
                AWorldState worldState = new AWorldState(this, GetCostCorridorToHome(index, homeIndex, family));
                worldState.Corridor[index] = AmphipodFamily.None;

                AmphipodFamily[] home = worldState.GetHome(family);
                for (int i = home.Length - 1; i >= 0; i--) {
                    if (home[i] == AmphipodFamily.None) {
                        home[i] = family;
                        break;
                    }
                }
                worldState.ComputeHash();

                return worldState;
            }

            public int GetCostCorridorToHome(int corridorIndex, float homeIndex, AmphipodFamily family) {
                int moveCount = 0;
                if (corridorIndex == 0) {
                    corridorIndex++;
                    moveCount++;
                } else if (corridorIndex == CorridorLength - 1) {
                    corridorIndex--;
                    moveCount++;
                }

                moveCount += 2 * (int)Math.Abs(corridorIndex - homeIndex) + 1;
                AmphipodFamily[] home = GetHome(family);
                moveCount += home.Count(x => x == AmphipodFamily.None);
                return moveCount * CostPerFamily[family];
            }

            public int GetCostHomeToCorridor(int corridorIndex, float homeIndex, AmphipodFamily family, AmphipodFamily sourceHome) {
                int moveCount = 0;
                if (corridorIndex == 0) {
                    corridorIndex++;
                    moveCount++;
                } else if (corridorIndex == CorridorLength - 1) {
                    corridorIndex--;
                    moveCount++;
                }

                moveCount += 2 * (int)Math.Abs(corridorIndex - homeIndex) + 1;
                AmphipodFamily[] home = GetHome(sourceHome);
                moveCount += home.Count(x => x == AmphipodFamily.None) + 1;
                return moveCount * CostPerFamily[family];
            }

            public bool IsHomeFree(AmphipodFamily[] home, AmphipodFamily amphipod) {
                return home.All(x => (x == AmphipodFamily.None) || (x == amphipod));
            }

            public bool CanGoHomeFromHome(float sourceHomeIndex, float dstHomeIndex) {
                if (sourceHomeIndex > dstHomeIndex) {
                    (dstHomeIndex, sourceHomeIndex) = (sourceHomeIndex, dstHomeIndex);
                }

                for (int i = (int)Math.Ceiling(sourceHomeIndex); i < dstHomeIndex; i++) {
                    if (Corridor[i] != AmphipodFamily.None) {
                        return false;
                    }
                }
                return true;
            }

            public bool CanGoHomeFromCorridor(int index, float homeIndex) {
                if (homeIndex > index) {
                    index++;
                    while (index < homeIndex) {
                        if (Corridor[index] != AmphipodFamily.None) {
                            return false;
                        }

                        index++;
                    }
                } else {
                    index--;
                    while (index > homeIndex) {
                        if (Corridor[index] != AmphipodFamily.None) {
                            return false;
                        }

                        index--;
                    }
                }

                return true;
            }

            public bool IsFinished() {
                return AHome.All(x => x == AmphipodFamily.A)
                    && BHome.All(x => x == AmphipodFamily.B)
                    && CHome.All(x => x == AmphipodFamily.C)
                    && DHome.All(x => x == AmphipodFamily.D);
            }

            public static bool IsHomeFinished(AmphipodFamily[] home, AmphipodFamily family) {
                return home.All(x => x == family);
            }

            public static AWorldState Parse(string corridorTxt, string aHomeText, string bHomeText, string cHomeText,
                                            string dHomeText) {
                return new AWorldState(ParseArray(corridorTxt),
                                       ParseArray(aHomeText),
                                       ParseArray(bHomeText),
                                       ParseArray(cHomeText),
                                       ParseArray(dHomeText),
                                       0);
            }

            private static AmphipodFamily[] ParseArray(string corridorTxt) {
                return corridorTxt.Select(c => c == ' ' ? AmphipodFamily.None : c - 'A' + AmphipodFamily.A).ToArray();
            }

            private static char GetChar(AmphipodFamily family) {
                switch (family) {
                    case AmphipodFamily.None: return ' ';
                    case AmphipodFamily.A: return 'A';
                    case AmphipodFamily.B: return 'B';
                    case AmphipodFamily.C: return 'C';
                    case AmphipodFamily.D: return 'D';
                    default:
                        throw new ArgumentOutOfRangeException(nameof(family), family, null);
                }
            }

            public override string ToString() {
                return new string(Corridor.Select(GetChar).ToArray())
                     + "/" + new string(AHome.Select(GetChar).ToArray())
                     + "/" + new string(BHome.Select(GetChar).ToArray())
                     + "/" + new string(CHome.Select(GetChar).ToArray())
                     + "/" + new string(DHome.Select(GetChar).ToArray())
                     + "|" + Cost;
            }
        }
    }
}
