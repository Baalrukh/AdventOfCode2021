using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode2021.Utils;

namespace AdventOfCode2021 {
    public class Day19 : Exercise {
        public long ExecutePart1(string[] lines) {
            var scanners = Parse(lines).ToList();

            var beaconsDistancesByScanner = scanners.Select(x => GetBeaconsDistances(scanners, x.Id)).ToList();

            var referentialTransforms = new Dictionary<(int source, int destination), ReferentialTransform>();

            for (int i = 0; i < scanners.Count; i++) {
                for (int j = 0; j < scanners.Count; j++) {
                    if (i == j)
                    {
                        continue;
                    }

                    AxisTransform axisTransform = VectorTransforms.FirstOrDefault(t =>
                        IsTransformBetweenReferentials(t, beaconsDistancesByScanner[j], beaconsDistancesByScanner[i]));
                    var allMatchingAxisTransforms = VectorTransforms.Where(t =>
                        IsTransformBetweenReferentials(t, beaconsDistancesByScanner[j], beaconsDistancesByScanner[i])).ToList();

                    if (allMatchingAxisTransforms.Count >= 1)
                    {
                        var translation = allMatchingAxisTransforms.Select(x => FindTranslation(scanners[j].Beacons, scanners[i].Beacons,
                            beaconsDistancesByScanner[j], beaconsDistancesByScanner[i], x)).FirstOrDefault(x => x.HasValue);
                        if (translation.HasValue)
                        {
                            referentialTransforms.Add((j, i), new ReferentialTransform(axisTransform, translation.Value));
                        }
                    }

                    // if (axisTransform != null) {
                    //     IntVector3? translation = FindTranslation(scanners[j].Beacons, scanners[i].Beacons,
                    //         beaconsDistancesByScanner[j], beaconsDistancesByScanner[i], axisTransform);
                    //
                    //
                    //     referentialTransforms.Add((j, i), new ReferentialTransform(axisTransform, translation));
                    // }
                }
            }

            foreach (var pair in referentialTransforms.Keys.OrderBy(x => x.source))
            {
                Console.WriteLine($"{pair.source} -> {pair.destination}");
            }

            var allBeaconsInRef0 = scanners.SelectMany(x => TransformAllBeaconsPositions2(x, scanners, referentialTransforms)).Distinct().ToList();
            // allBeaconsInRef0.Sort(Vector3Comparer);

            return allBeaconsInRef0.Count;
        }

        private int Vector3Comparer(IntVector3 a, IntVector3 b)
        {
            var xCompare = a.X.CompareTo(b.X);
            if (xCompare != 0)
            {
                return xCompare;
            }
            var yCompare = a.Y.CompareTo(b.Y);
            if (yCompare != 0)
            {
                return yCompare;
            }
            return a.Z.CompareTo(b.Z);
        }

        private IntVector3? FindTranslation(IReadOnlyList<Beacon> srcBeacons, IReadOnlyList<Beacon> dstBeacons,
            List<BeaconsDistance> srcDistances, List<BeaconsDistance> dstDistances, AxisTransform axisTransform)
        {
            BeaconDistanceComparer comparer = new BeaconDistanceComparer(axisTransform);

            var dictionary = srcDistances.Select(d => (d, dstDistances.Where(x => comparer.Equals(x, d)).ToList()))
                .Where(x => x.Item2.Any())
                .ToDictionary(x => x.d, x => x.Item2);
            var keyValuePair = dictionary.OrderBy(x => x.Value.Count()).First();
            var srcDistance = keyValuePair.Key;
            var dstDistance = keyValuePair.Value[0];

            var offset1 = dstBeacons[dstDistance.Beacon1].LocalPosition - axisTransform.Transform(srcBeacons[srcDistance.Beacon1].LocalPosition);
            var offset2 = dstBeacons[dstDistance.Beacon2].LocalPosition - axisTransform.Transform(srcBeacons[srcDistance.Beacon1].LocalPosition);

            var beacons = dictionary.Keys.SelectMany(x => new [] {srcBeacons[x.Beacon1], srcBeacons[x.Beacon2]}).Distinct().ToList();
            var hasCorrespondingBeacon1 = beacons.Select(x => axisTransform.Transform(x.LocalPosition) + offset1).All(x => dstBeacons.Any(dst => dst.LocalPosition == x));
            if (hasCorrespondingBeacon1)
            {
                return offset1;
            }
            var hasCorrespondingBeacon2 = beacons.Select(x => axisTransform.Transform(x.LocalPosition) + offset2).All(x => dstBeacons.Any(dst => dst.LocalPosition == x));
            if (!hasCorrespondingBeacon2)
            {
                return null;
            }

            return offset2;
        }

        public class ReferentialTransform {
            public readonly AxisTransform AxisTransform;
            public readonly IntVector3 Translation;

            public ReferentialTransform(AxisTransform axisTransform, IntVector3 translation) {
                AxisTransform = axisTransform;
                Translation = translation;
            }

            public IntVector3 Transform(IntVector3 vector3) {
                return AxisTransform.Transform(vector3) + Translation;
            }
        }

        // private ReferentialTransform GetTransformAndTranslation(AxisTransform t, List<List<BeaconsDistance>> beaconsDistancesByScanner, int j, int i) {
        //     bool = IsTransformBetweenReferentials(t, beaconsDistancesByScanner[j], beaconsDistancesByScanner[i]);
        //     if (translation.HasValue) {
        //         return new ReferentialTransform(t, translation.Value);
        //     }
        //
        //     return null;
        // }

        private IEnumerable<IntVector3> TransformAllBeaconsPositions2(Scanner scanner, List<Scanner> scanners,
            Dictionary<(int source, int destination), ReferentialTransform> referentialTransforms) {

            if (scanner.Id == 0)
            {
                return scanner.Beacons.Select(x => x.LocalPosition);
            }

            List<ReferentialTransform> transforms = FindTransformsTo0(scanner.Id, referentialTransforms, new List<int>());
            if (transforms == null)
            {
                Console.WriteLine($"No path from {scanner.Id} to 0");
                return new IntVector3[0];
            }

            return scanner.Beacons.Select(x => transforms.Aggregate(x.LocalPosition, (res, transform) => transform.Transform(res)));

            // if ((scanner.TransformToScanner0 == null) && (scanner.TransformToScanner1 == null)) {
            //     return scanner.Beacons.Select(x => x.LocalPosition);
            // }
            //
            // if (scanner.TransformToScanner0 != null) {
            //     return scanner.Beacons.Select(x => scanner.TransformToScanner0.Transform(x.LocalPosition));
            // }
            //
            // Scanner scanner1 = scanners[1];
            // return scanner.Beacons.Select(x =>
            //     scanner1.TransformToScanner0.Transform(scanner.TransformToScanner1.Transform(x.LocalPosition)));
        }

        private List<ReferentialTransform> FindTransformsTo0(int scannerId,
            Dictionary<(int source, int destination), ReferentialTransform> referentialTransforms, List<int> visitedScanners)
        {
            if (referentialTransforms.TryGetValue((scannerId, 0), out var transform))
            {
                return new List<ReferentialTransform>() {transform};
            }

            foreach (var transition in referentialTransforms.Keys.Where(x => x.source == scannerId))
            {
                if (visitedScanners.Contains(transition.destination))
                {
                    continue;
                }
                visitedScanners.Add(scannerId);
                var transforms = FindTransformsTo0(transition.destination, referentialTransforms, visitedScanners);
                if (transforms != null)
                {
                    transforms.Insert(0, referentialTransforms[transition]);
                    return transforms;
                }
                visitedScanners.RemoveAt(visitedScanners.Count - 1);
            }

            return null;
        }

        private IEnumerable<IntVector3> TransformAllBeaconsPositions(Scanner scanner, List<Scanner> scanners) {
            if ((scanner.TransformToScanner0 == null) && (scanner.TransformToScanner1 == null)) {
                return scanner.Beacons.Select(x => x.LocalPosition);
            }

            if (scanner.TransformToScanner0 != null) {
                return scanner.Beacons.Select(x => scanner.TransformToScanner0.Transform(x.LocalPosition));
            }

            Scanner scanner1 = scanners[1];
            return scanner.Beacons.Select(x =>
                scanner1.TransformToScanner0.Transform(scanner.TransformToScanner1.Transform(x.LocalPosition)));
        }

        private bool IsTransformBetweenReferentials(AxisTransform axisTransform,
            List<BeaconsDistance> beaconsDistances0, List<BeaconsDistance> beaconsDistances1) {
            BeaconDistanceComparer comparer = new BeaconDistanceComparer(axisTransform);
            var matchingOffsets = beaconsDistances0.Intersect(beaconsDistances1, comparer).ToList();
            var matchingBeacons = matchingOffsets.SelectMany(x => new[] { x.Beacon1, x.Beacon2 }).Distinct().ToList();
            return matchingBeacons.Count >= 12;
        }

        private static List<BeaconsDistance> GetBeaconsDistances(List<Scanner> scanners, int scannerID) {
            List<BeaconsDistance> beaconsDistances = new List<BeaconsDistance>();
            var beacons = scanners[scannerID].Beacons;
            for (int i = 0; i < beacons.Count; i++) {
                for (int j = i + 1; j < beacons.Count; j++) {
                    IntVector3 offset = beacons[i].LocalPosition - beacons[j].LocalPosition;
                    beaconsDistances.Add(new BeaconsDistance(scannerID, i, j, offset));
                }
            }

            return beaconsDistances;
        }


        public static IEnumerable<Scanner> Parse(string[] lines) {
            int scannerCount = 0;
            List<Beacon> beacons = new List<Beacon>();
            using (IEnumerator<string> enumerator = lines.Cast<string>().GetEnumerator()) {
                while (enumerator.MoveNext()) {
                    while (enumerator.MoveNext() && !string.IsNullOrEmpty(enumerator.Current)) {
                        beacons.Add(new Beacon(ParsePosition(enumerator.Current)));
                    }

                    yield return new Scanner(scannerCount++, beacons);
                    beacons = new List<Beacon>();
                }
            }
        }

        private static IntVector3 ParsePosition(string line) {
            string[] tokens = line.Split(',');
            return new IntVector3(int.Parse(tokens[0]), int.Parse(tokens[1]), int.Parse(tokens[2]));
        }
        

        public long ExecutePart2(string[] lines) {
            return -2;
        }


        public class Scanner {
            public int Id { get; }
            public readonly IReadOnlyList<Beacon> Beacons;
            public ReferentialTransform TransformToScanner0;
            public ReferentialTransform TransformToScanner1;

            public Scanner(int id, IReadOnlyList<Beacon> beacons) {
                Id = id;
                Beacons = beacons;
            }
        }
        
        public class Beacon {
            public IntVector3 LocalPosition;

            public Beacon(IntVector3 localPosition) {
                LocalPosition = localPosition;
            }
        }

        public class BeaconsDistance {
            public readonly int ScannerID;
            public readonly int Beacon1;
            public readonly int Beacon2;
            public readonly int DistanceSq;
            public readonly IntVector3 Offset;

            public BeaconsDistance(int scannerId, int beacon1, int beacon2, IntVector3 offset) {
                ScannerID = scannerId;
                Beacon1 = beacon1;
                Beacon2 = beacon2;
                DistanceSq = offset.LengthSq;
                Offset = offset;
            }
        }

        public class BeaconDistanceComparer : IEqualityComparer<BeaconsDistance> {
            private AxisTransform _axisTransform;

            public BeaconDistanceComparer(AxisTransform axisTransform) {
                _axisTransform = axisTransform;
            }

            public bool Equals(BeaconsDistance x, BeaconsDistance y) {
                if (x.DistanceSq != y.DistanceSq) {
                    return false;
                }

                return _axisTransform.Transform(y.Offset) == x.Offset;
                // return VectorTransforms.Any(t => t.Transform(y.Offset) == x.Offset);
            }

            public int GetHashCode(BeaconsDistance obj) {
                return obj.DistanceSq;
            }
        }

        public class AxisTransform {
            private int _xIndex;
            private int _xSign;
            private int _yIndex;
            private int _ySign;
            private int _zIndex;
            private int _zSign;

            public AxisTransform(int x, int y, int z) {
                _xIndex = Math.Abs(x) - 1;
                _xSign = x > 0 ? 1 : -1;
                _yIndex = Math.Abs(y) - 1;
                _ySign = y > 0 ? 1 : -1;
                _zIndex = Math.Abs(z) - 1;
                _zSign = z > 0 ? 1 : -1;
            }

            public IntVector3 Transform(IntVector3 vector) {
                return new IntVector3(vector[_xIndex] * _xSign,
                    vector[_yIndex] * _ySign,
                    vector[_zIndex] * _zSign);
            }

            private char GetAxis(int index) {
                switch (index) {
                    case 0: return 'x';
                    case 1: return 'y';
                    case 2: return 'z';
                    default: throw new ArgumentException();
                }
            }
            
            public override string ToString() {
                StringBuilder stringBuilder = new StringBuilder();
                if (_xSign == -1) {
                    stringBuilder.Append('-');
                }
                stringBuilder.Append(GetAxis(_xIndex));

                stringBuilder.Append(',');
                if (_ySign == -1) {
                    stringBuilder.Append('-');
                }
                stringBuilder.Append(GetAxis(_yIndex));

                stringBuilder.Append(',');
                if (_zSign == -1) {
                    stringBuilder.Append('-');
                }
                stringBuilder.Append(GetAxis(_zIndex));
                
                return stringBuilder.ToString();
            }
        }

        public const int X = 1;
        public const int Y = 2;
        public const int Z = 3;

        public static readonly IReadOnlyList<AxisTransform> VectorTransforms;
        public static readonly IReadOnlyList<AxisTransform> VectorTransforms1
            = new[] {
                new AxisTransform(X, Y, Z),
                new AxisTransform(X, Z, -Y),
                new AxisTransform(X, -Y, -Z),
                new AxisTransform(X, -Z, Y),

                new AxisTransform(-X, -Y, Z),
                new AxisTransform(-X, -Z, -Y),
                new AxisTransform(-X, Y, -Z),
                new AxisTransform(-X, Z, Y),

                new AxisTransform(Y, Z, X),
                new AxisTransform(Y, X, -Z),
                new AxisTransform(Y, -Z, -X),
                new AxisTransform(Y, -X, Z),

                new AxisTransform(-Y, -Z, X),
                new AxisTransform(-Y, -X, -Z),
                new AxisTransform(-Y, Z, -X),
                new AxisTransform(-Y, X, Z),

                new AxisTransform(Z, X, Y),
                new AxisTransform(Z, Y, -X),
                new AxisTransform(Z, -X, -Y),
                new AxisTransform(Z, -Y, X),

                new AxisTransform(-Z, -X, Y),
                new AxisTransform(-Z, -Y, -X),
                new AxisTransform(-Z, X, -Y),
                new AxisTransform(-Z, Y, X),
            };

        public static readonly IReadOnlyList<AxisTransform> VectorTransforms2;

        static Day19()
        {
            int[][] axisPermutations = new[]
            {
                new[] {X, Y, Z},
                new[] {X, Z, Y},
                new[] {Y, X, Z},
                new[] {Y, Z, X},
                new[] {Z, X, Y},
                new[] {Z, Y, X},
            };
            int[][] signs = new[]
            {
                new[] { 1,  1,  1},
                new[] { 1,  1, -1},
                new[] { 1, -1,  1},
                new[] { 1, -1, -1},
                new[] {-1,  1,  1},
                new[] {-1,  1, -1},
                new[] {-1, -1,  1},
                new[] {-1, -1, -1},
            };

            List<AxisTransform> axisTransforms = new List<AxisTransform>();
            for (int i = 0; i < axisPermutations.Length; i++)
            {
                for (int j = 0; j < signs.Length; j++)
                {
                    axisTransforms.Add(new AxisTransform(axisPermutations[i][0] * signs[j][0],
                        axisPermutations[i][1] * signs[j][1],
                        axisPermutations[i][2] * signs[j][2]));
                }
            }

            VectorTransforms2 = axisTransforms;

            VectorTransforms = VectorTransforms2;
        }

    }
}
