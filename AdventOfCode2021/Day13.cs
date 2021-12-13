using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode2021.Utils;

namespace AdventOfCode2021 {
    public class Day13 : Exercise
    {
        private const string VerticalFolding = "fold along x";

        public long ExecutePart1(string[] lines)
        {
            var codePage = CodePage.Parse(lines);
// codePage.Plot();
            codePage.ApplyFirstFolding();
// codePage.Plot();
            return codePage.Points.Count;
        }


        public long ExecutePart2(string[] lines) {
            var codePage = CodePage.Parse(lines);
// codePage.Plot();
            codePage.ApplyAllFolding();
codePage.Plot();
            return codePage.Points.Count;
        }

        public class CodePage
        {
            private List<IntVector2> _points;
            private List<Folding> _foldings;

            public List<IntVector2> Points => _points;

            public CodePage(List<IntVector2> points, List<Folding> foldings)
            {
                _points = points;
                _foldings = foldings;
            }

            private static Folding ParseFolding(string text)
            {
                var tokens = text.Split('=');
                FoldingOrientation orientation = tokens[0] == VerticalFolding ? FoldingOrientation.Vertical : FoldingOrientation.Horizontal;
                return new Folding(orientation, int.Parse(tokens[1]));
            }

            private static IntVector2 ParsePoint(string text)
            {
                var tokens = text.Split(',');
                return new IntVector2(int.Parse(tokens[0]), int.Parse(tokens[1]));
            }

            public static CodePage Parse(string[] lines)
            {
                var emptyLineIndex = Array.IndexOf(lines, "");
                var points = lines.Take(emptyLineIndex).Select(ParsePoint).ToList();
                var foldings = lines.Skip(emptyLineIndex + 1).Select(ParseFolding).ToList();

                return new CodePage(points, foldings);
            }

            public void ApplyFirstFolding()
            {
                _points = _points.Select(x => _foldings[0].Apply(x)).Distinct().ToList();
            }

            public void ApplyAllFolding()
            {
                foreach (var folding in _foldings)
                {
                    _points = _points.Select(x => folding.Apply(x)).Distinct().ToList();
                }
            }

            public void Plot()
            {
                int y = 0;
                int x = 0;
                _points.Sort(PointComparer);
                var enumerator = _points.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var point = enumerator.Current;
                    while (y < point.Y)
                    {
                        Console.WriteLine();
                        y++;
                        x = 0;
                    }

                    while (x < point.X)
                    {
                        Console.Write(" ");
                        x++;
                    }
                    Console.Write("#");
                    x++;
                }

                Console.WriteLine();
            }

            private int PointComparer(IntVector2 a, IntVector2 b)
            {
                var yCompare = a.Y.CompareTo(b.Y);
                if (yCompare != 0)
                {
                    return yCompare;
                }

                return a.X.CompareTo(b.X);
            }

        }

        public enum FoldingOrientation
        {
            Horizontal,
            Vertical
        }

        public class Folding
        {
            private readonly FoldingOrientation _orientation;
            private readonly int _distance;

            public Folding(FoldingOrientation orientation, int distance)
            {
                _orientation = orientation;
                _distance = distance;
            }

            public IntVector2 Apply(IntVector2 point)
            {
                if (_orientation == FoldingOrientation.Horizontal)
                {
                    if (point.Y > _distance)
                    {
                        return new IntVector2(point.X, 2 * _distance - point.Y);
                    }

                    return point;
                }

                if (point.X > _distance)
                {
                    return new IntVector2(2 * _distance - point.X, point.Y);
                }

                return point;
            }
        }
    }
}
