using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode2021.Utils;

namespace AdventOfCode2021 {
    public class Day19 : Exercise {
        public long ExecutePart1(string[] lines) {
            return GetAllBeacons(lines).Count;
        }

        private IReadOnlyList<IntVector3> GetAllBeacons(string[] lines) {
            var scanners = Parse(lines).ToList();
            var beaconsDistancesByScanner = scanners.Select(x => GetBeaconsDistances(scanners[x.Id])).ToList();

            var scanner0 = scanners[0];
            var mainBeaconsDistance = beaconsDistancesByScanner[0];

            var scannersToMatch = new Queue<Scanner>(scanners.Skip(1));
            int failCount = 0;

            while (scannersToMatch.Count > 0) {
                Scanner scanner = scannersToMatch.Dequeue();

                var beaconsDistances = beaconsDistancesByScanner[scanner.Id];
                AxisTransform axisTransform = VectorTransforms.FirstOrDefault(t =>
                    IsTransformBetweenReferentials(t, beaconsDistances, mainBeaconsDistance));

                if (axisTransform != null) {
                    var translation = FindTranslation(scanner.Beacons, scanner0.Beacons, beaconsDistances,
                        mainBeaconsDistance, axisTransform);
                    ReferentialTransform referentialTransform = new ReferentialTransform(axisTransform, translation);

                    var mainBeacons = scanner0.Beacons.Concat(scanner.Beacons.Select(x => referentialTransform.Transform(x)))
                        .Distinct().ToList();
                    
                    Console.WriteLine($"Matched scanner {scanner.Id}, beacon count : {mainBeacons.Count}");
                    
                    scanner0 = new Scanner(0, mainBeacons);
                    mainBeaconsDistance = GetBeaconsDistances(scanner0);
                    failCount = 0;
                }
                else {
                    scannersToMatch.Enqueue(scanner);
                    failCount++;
                }
            }

            var allBeacons = scanner0.Beacons;
            return allBeacons;
        }

        private IntVector3 FindTranslation(IReadOnlyList<IntVector3> srcBeacons, IReadOnlyList<IntVector3> dstBeacons,
            List<BeaconsDistance> srcDistances, List<BeaconsDistance> dstDistances, AxisTransform axisTransform)
        {
            BeaconDistanceComparer comparer = new BeaconDistanceComparer(axisTransform);

            var dictionary = srcDistances.Select(d => (d, dstDistances.Where(x => comparer.Equals(x, d)).ToList()))
                .Where(x => x.Item2.Any())
                .ToDictionary(x => x.d, x => x.Item2);
            var keyValuePair = dictionary.OrderBy(x => x.Value.Count()).First();
            var srcDistance = keyValuePair.Key;
            var dstDistance = keyValuePair.Value[0];

            var offset1 = dstBeacons[dstDistance.Beacon1] - axisTransform.Transform(srcBeacons[srcDistance.Beacon1]);
            var offset2 = dstBeacons[dstDistance.Beacon2] - axisTransform.Transform(srcBeacons[srcDistance.Beacon1]);

            var beacons = dictionary.Keys.SelectMany(x => new [] {srcBeacons[x.Beacon1], srcBeacons[x.Beacon2]}).Distinct().ToList();
            var hasCorrespondingBeacon1 = beacons.Select(x => axisTransform.Transform(x) + offset1).All(x => dstBeacons.Any(dst => dst == x));
            if (hasCorrespondingBeacon1)
            {
                return offset1;
            }
            var hasCorrespondingBeacon2 = beacons.Select(x => axisTransform.Transform(x) + offset2).All(x => dstBeacons.Any(dst => dst == x));
            if (!hasCorrespondingBeacon2)
            {
                throw new Exception();
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

        private bool IsTransformBetweenReferentials(AxisTransform axisTransform,
            List<BeaconsDistance> beaconsDistances0, List<BeaconsDistance> beaconsDistances1) {
            BeaconDistanceComparer comparer = new BeaconDistanceComparer(axisTransform);
            var matchingOffsets = beaconsDistances0.Intersect(beaconsDistances1, comparer).ToList();
            var matchingBeacons = matchingOffsets.SelectMany(x => new[] { x.BeaconPosition1, x.BeaconPosition2 }).Distinct().ToList();
            return matchingBeacons.Count >= 12;
        }

        private static List<BeaconsDistance> GetBeaconsDistances(Scanner scanner) {
            List<BeaconsDistance> beaconsDistances = new List<BeaconsDistance>();
            var beacons = scanner.Beacons;
            for (int i = 0; i < beacons.Count; i++) {
                for (int j = 0; j < beacons.Count; j++) {
                    if (i != j) {
                        beaconsDistances.Add(new BeaconsDistance(scanner.Id, i, j, beacons[i], beacons[j]));
                    }
                }
            }

            return beaconsDistances;
        }


        public static IEnumerable<Scanner> Parse(string[] lines) {
            int scannerCount = 0;
            List<IntVector3> beacons = new List<IntVector3>();
            using (IEnumerator<string> enumerator = lines.Cast<string>().GetEnumerator()) {
                while (enumerator.MoveNext()) {
                    while (enumerator.MoveNext() && !string.IsNullOrEmpty(enumerator.Current)) {
                        beacons.Add(ParsePosition(enumerator.Current));
                    }

                    yield return new Scanner(scannerCount++, beacons);
                    beacons = new List<IntVector3>();
                }
            }
        }

        private static IntVector3 ParsePosition(string line) {
            string[] tokens = line.Split(',');
            return new IntVector3(int.Parse(tokens[0]), int.Parse(tokens[1]), int.Parse(tokens[2]));
        }
        

        public long ExecutePart2(string[] lines) {
            var allBeacons = GetAllBeacons(lines);

            int max = 0;
            for (int i = 0; i < allBeacons.Count; i++) {
                for (int j = i + 1; j < allBeacons.Count; j++) {
                    IntVector3 vector = allBeacons[j] - allBeacons[i];
                    int manhattanDist = Math.Abs(vector.X) + Math.Abs(vector.Y) + Math.Abs(vector.Z);
                    if (manhattanDist > max) {
                        max = manhattanDist;
                    }
                }
            }

            return max;
        }


        public class Scanner {
            public int Id { get; }
            public readonly IReadOnlyList<IntVector3> Beacons;
            public ReferentialTransform TransformToScanner0;
            public ReferentialTransform TransformToScanner1;

            public Scanner(int id, IReadOnlyList<IntVector3> beacons) {
                Id = id;
                Beacons = beacons;
            }
        }
        
        // public class Beacon {
        //     public IntVector3 LocalPosition;
        //
        //     public Beacon(IntVector3 localPosition) {
        //         LocalPosition = localPosition;
        //     }
        // }

        public class BeaconsDistance {
            public readonly int ScannerID;
            public readonly int Beacon1;
            public readonly int Beacon2;
            public readonly IntVector3 BeaconPosition1;
            public readonly IntVector3 BeaconPosition2;
            public readonly int DistanceSq;
            public readonly IntVector3 Offset;

            public BeaconsDistance(int scannerId, int beacon1, int beacon2, IntVector3 beaconPosition1, IntVector3 beaconPosition2) {
                ScannerID = scannerId;
                Beacon1 = beacon1;
                Beacon2 = beacon2;
                Offset = beaconPosition2 - beaconPosition1;
                DistanceSq = Offset.LengthSq;
                BeaconPosition1 = beaconPosition1;
                BeaconPosition2 = beaconPosition2;
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

            VectorTransforms = VectorTransforms1;
        }

    }
}
