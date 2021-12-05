using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AdventOfCode2021.Utils;

namespace AdventOfCode2021 {
    public class Day4 : Exercise {
        public int ExecutePart1(string[] lines) {
            return Bingo.Parse(lines).GetWinningScore();
        }

        public int ExecutePart2(string[] lines) {
            return Bingo.Parse(lines).GetLastWinningScore();
        }

        public class Grid {
            private const int Width = 5;
            
            private readonly int[] _values;
            private readonly bool[] _marks = new bool[Width * Width];
            public bool Finished { get; private set; }

            public Grid(int[] values) {
                if (values.Length != Width * Width) {
                    throw new ArgumentException("Invalid grid size");
                }

                _values = values;
            }

            public IEnumerable<IntVector2> MarkValue(int value) {
                int index = 0;
                for (int y = 0; y < Width; y++) {
                    for (int x = 0; x < Width; x++) {
                        if (_values[index] == value) {
                            _marks[index] = true;
                            yield return new IntVector2(x, y);
                        }
                        index++;
                    }
                }
            }
            
            public bool IsGridWinning(int value, out int score) {
                var allMarked = MarkValue(value).ToList();
                if (allMarked.Any(IsComplete)) {
                    score = GetGridScore() * value;
                    Finished = true;
                    return true;
                }

                score = 0;
                return false;
            }
            
            public bool IsComplete(IntVector2 position) {
                return IsLineComplete(position.Y)
                       || IsColumnComplete(position.X);
                       // || IsDiagonalComplete(position.X, position.Y);
            }
            
            public bool IsLineComplete(int i) {
                int index = i * Width;
                return _marks.Skip(index).Take(Width).All(x => x);
            }

            public bool IsColumnComplete(int i) {
                return _marks.Skip(i).TakeByColumn(Width, Width).All(x => x);
            }

            public bool IsDiagonalComplete(int x, int y) {
                if (x == y) {
                    if (Enumerable.Range(0, Width).All(i => _marks[i + Width * i])) {
                        return true;
                    }
                }

                if (x == Width - y - 1) {
                    if (Enumerable.Range(0, Width).All(i => _marks[(Width - i - 1) + Width * i])) {
                        return true;
                    }
                }

                return false;
            }

            public int GetGridScore() {
                return _marks.Zip(_values, (b, i) => b ? 0 : i).Sum();
            }
            
            internal void Mark(int x, int y) {
                _marks[x + y * Width] = true;
            }

            internal string GetMarkedGrid() {
                return string.Join("", _marks.Select(x => x ? "X" : " "));
            }
            
            protected bool Equals(Grid other) {
                return _values.SequenceEqual(other._values);
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Grid)obj);
            }

            public override int GetHashCode() {
                int hashcode = 0;
                for (int i = 0; i < Width * Width; i++) {
                    hashcode = (hashcode * 397) ^ _values[i];
                }

                return hashcode;
            }

            public override string ToString() {
                StringBuilder builder = new StringBuilder(Width * Width * 5);
                int index = 0;
                for (int y = 0; y < Width; y++) {
                    for (int x = 0; x < Width; x++) {
                        if (_marks[index]) {
                            builder.Append($"*{_values[index]:00}* ");
                        } else {
                            builder.Append($" {_values[index]:00}  ");
                        }

                        index++;
                    }

                    builder.Append("\n");
                }
                
                return builder.ToString();
            }

            public static Grid Parse(List<string> lines) {
                if (lines.Count != Width) {
                    throw new ArgumentException("Invalid grid line count");
                }

                int[] array = lines.SelectMany(x => x.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    .Select(int.Parse).ToArray();
                return new Grid(array);
            }

        }

        public class Bingo {
            private readonly int[] _randomSequence;
            private readonly Grid[] _grids;

            public Bingo(int[] randomSequence, Grid[] grids) {
                _randomSequence = randomSequence;
                _grids = grids;
            }

            protected bool Equals(Bingo other) {
                return _randomSequence.SequenceEqual(other._randomSequence) && _grids.SequenceEqual(other._grids);
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Bingo)obj);
            }

            public override int GetHashCode() {
                unchecked {
                    return ((_randomSequence != null ? _randomSequence.GetHashCode() : 0) * 397) ^
                           (_grids != null ? _grids.GetHashCode() : 0);
                }
            }

            public int GetWinningScore() {
                foreach (int value in _randomSequence) {
                    Debug.WriteLine(value);
                    foreach (Grid grid in _grids) {
                        if (grid.IsGridWinning(value, out var score)) {
                            return score;
                        }
                    }
                }

                return 0;
            }

            public int GetLastWinningScore() {
                foreach (int value in _randomSequence) {
                    Debug.WriteLine(value);
                    foreach (Grid grid in _grids.Where(x => !x.Finished)) {
                        if (grid.IsGridWinning(value, out var score)) {
                            if (_grids.All(x => x.Finished)) {
                                return score;
                            }
                        }
                    }
                }

                return 0;
            }

            public static Bingo Parse(string[] lines) {
                int[] randomSequence = lines[0].Split(',').Select(x => int.Parse(x)).ToArray();
                var grids = lines.Skip(1).Where(x => !string.IsNullOrEmpty(x)).Batch(5).Select(Grid.Parse).ToArray();
                return new Bingo(randomSequence, grids);
            }
        }
    }
}