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

            var referentialTransforms = new Dictionary<(int source, int destination), VectorTransform>();

            for (int i = 0; i < scanners.Count; i++) {
                for (int j = 1; j < scanners.Count; j++) {
                    VectorTransform vectorTransform = VectorTransforms.FirstOrDefault(t =>
                        IsTransformBetweenReferentials(t, beaconsDistancesByScanner[j], beaconsDistancesByScanner[i]));

                    if (vectorTransform != null) {
                        referentialTransforms.Add((j, i), vectorTransform);
                    }
                }
            }

            for (int i = 1; i < scanners.Count; i++) {
                if (referentialTransforms.TryGetValue((i, 0), out var vectorTransform)) {
                    scanners[i].TransformToScanner0 = vectorTransform;
                }
                if (referentialTransforms.TryGetValue((i, 1), out vectorTransform)) {
                    scanners[i].TransformToScanner1 = vectorTransform;
                }
            }

            var allBeaconsInRef0 = scanners.Take(2).SelectMany(x => TransformAllBeaconsPositions(x, scanners)).Distinct().ToList();


            return -1;
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

        private bool IsTransformBetweenReferentials(VectorTransform vectorTransform,
            List<BeaconsDistance> beaconsDistances0, List<BeaconsDistance> beaconsDistances1) {
            var matchingOffsets = beaconsDistances0.Intersect(beaconsDistances1, new BeaconDistanceComparer(vectorTransform));
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
            public VectorTransform TransformToScanner0;
            public VectorTransform TransformToScanner1;

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
            private VectorTransform _vectorTransform;

            public BeaconDistanceComparer(VectorTransform vectorTransform) {
                _vectorTransform = vectorTransform;
            }

            public bool Equals(BeaconsDistance x, BeaconsDistance y) {
                if (x.DistanceSq != y.DistanceSq) {
                    return false;
                }

                return _vectorTransform.Transform(y.Offset) == x.Offset;
                // return VectorTransforms.Any(t => t.Transform(y.Offset) == x.Offset);
            }

            public int GetHashCode(BeaconsDistance obj) {
                return obj.DistanceSq;
            }
        }

        public class VectorTransform {
            private int _xIndex;
            private int _xSign;
            private int _yIndex;
            private int _ySign;
            private int _zIndex;
            private int _zSign;

            public VectorTransform(int x, int y, int z) {
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

        public static readonly IReadOnlyList<VectorTransform> VectorTransforms
            = new[] {
                new VectorTransform(X, Y, Z),
                new VectorTransform(X, Z, -Y),
                new VectorTransform(X, -Y, -Z),
                new VectorTransform(X, -Z, Y),

                new VectorTransform(-X, -Y, Z),
                new VectorTransform(-X, -Z, -Y),
                new VectorTransform(-X, Y, -Z),
                new VectorTransform(-X, Z, Y),

                new VectorTransform(Y, Z, X),
                new VectorTransform(Y, X, -Z),
                new VectorTransform(Y, -Z, -X),
                new VectorTransform(Y, -X, Z),

                new VectorTransform(-Y, -Z, X),
                new VectorTransform(-Y, -X, -Z),
                new VectorTransform(-Y, Z, -X),
                new VectorTransform(-Y, X, Z),

                new VectorTransform(Z, Z, Y),
                new VectorTransform(Z, Y, -X),
                new VectorTransform(Z, -X, -Y),
                new VectorTransform(Z, -Y, X),

                new VectorTransform(-Z, -X, Y),
                new VectorTransform(-Z, -Y, -X),
                new VectorTransform(-Z, X, -Y),
                new VectorTransform(-Z, Y, X),
            };
    }
}
