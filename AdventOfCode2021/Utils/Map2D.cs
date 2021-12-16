using System;
using System.Text;

namespace AdventOfCode2021.Utils {
    public class Map2D<T> {
        private readonly T[,] _values;
        public readonly int Width;
        public readonly int Height;


        private Map2D(T[,] values, int width, int height) {
            _values = values;
            Width = width;
            Height = height;
        }

        public T this[IntVector2 position] => _values[position.X + 1, position.Y + 1];
        public T this[int x, int y] {
            get => _values[x + 1, y + 1];
            set => _values[x + 1, y + 1] = value;
        }

        public override string ToString() {
            StringBuilder stringBuilder = new StringBuilder((Width + 1) * Height);
            for (int y = 1; y < Height + 1; y++) {
                for (int x = 1; x < Width + 1; x++) {
                    stringBuilder.Append(_values[x, y]);
                }

                stringBuilder.Append('\n');
            }

            return stringBuilder.ToString();
        }

        public static Map2D<T> Create(int width, int height, Func<IntVector2, T> elementFactory, Func<T> borderValueFactory) {
            var map = new T[width+2, height+2];
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    map[x + 1, y + 1] = elementFactory(new IntVector2(x, y));
                }
            }

            for (int y = 0; y < height; y++) {
                map[0, y + 1] = borderValueFactory();
                map[width + 1, y + 1] = borderValueFactory();
            }

            for (int x = 0; x < width; x++) {
                map[x + 1, 0] = borderValueFactory();
                map[x + 1, height + 1] = borderValueFactory();
            }
            map[0, 0] = borderValueFactory();
            map[width + 1, 0] = borderValueFactory();
            map[width + 1, height + 1] = borderValueFactory();
            map[0, height + 1] = borderValueFactory();

            return new Map2D<T>(map, width, height);
        }

        public static Map2D<T> Parse<T>(string[] lines, Func<int, T> elementFactory, Func<T> borderValueFactory) {
            int width = lines[0].Length;
            int height = lines.Length;

            var map = new T[width+2, height+2];
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    map[x + 1, y + 1] = elementFactory(lines[y][x] - '0');
                }
            }

            for (int y = 0; y < height; y++) {
                map[0, y + 1] = borderValueFactory();
                map[width + 1, y + 1] = borderValueFactory();
            }

            for (int x = 0; x < width; x++) {
                map[x + 1, 0] = borderValueFactory();
                map[x + 1, height + 1] = borderValueFactory();
            }
            map[0, 0] = borderValueFactory();
            map[width + 1, 0] = borderValueFactory();
            map[width + 1, height + 1] = borderValueFactory();
            map[0, height + 1] = borderValueFactory();

            return new Map2D<T>(map, width, height);
        }
    }
}