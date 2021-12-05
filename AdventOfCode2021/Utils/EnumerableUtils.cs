using System.Collections.Generic;

namespace AdventOfCode2021.Utils {
    public static class EnumerableUtils{

        public static IEnumerable<List<T>> Batch<T>(this IEnumerable<T> sequence, int batchSize) {
            using (var enumerator = sequence.GetEnumerator()) {
                while (true) {
                    var list = new List<T>();
                    for (int i = 0; i < batchSize; i++) {
                        if (enumerator.MoveNext()) {
                            list.Add(enumerator.Current);
                        } else {
                            if (list.Count > 0) {
                                yield return list;
                            }
                            yield break;
                        }
                    }

                    yield return list;
                }
            }
        }

        public static IEnumerable<T> TakeByColumn<T>(this IEnumerable<T> sequence, int count, int stride) {
            using (var enumerator = sequence.GetEnumerator()) {
                for (int i = 0; i < count; i++) {
                    if (!enumerator.MoveNext()) {
                        yield break;
                    }

                    yield return enumerator.Current;
                    for (int j = 0; j < stride - 1; j++) {
                        enumerator.MoveNext();
                    }
                }
            }
        }

    }
}