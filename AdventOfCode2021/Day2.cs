using System.Collections.Generic;
using System.Linq;
using AdventOfCode2021.Utils;

namespace AdventOfCode2021
{
    public class Day2 : Exercise
    {
        private delegate IntVector2 PositionCommand(IntVector2 state, int value);
        private delegate SubmarineState SubmarineCommand(SubmarineState state, int value);

        private static readonly Dictionary<string, PositionCommand> _positionCommands = new Dictionary<string, PositionCommand>()
        {
            {"forward", (state, value) => state + new IntVector2(1, 0) * value },
            {"down", (state, value) => state + new IntVector2(0, 1) * value},
            {"up", (state, value) => state + new IntVector2(0, -1) * value},
        };

        private static readonly Dictionary<string, SubmarineCommand> _submarineCommands = new Dictionary<string, SubmarineCommand>()
        {
            {"forward", (state, value) => state.AimAdvance(value)},
            {"down", (state, value) => state.Aim(value)},
            {"up", (state, value) => state.Aim(-value)},
        };

        public int ExecutePart1(string[] lines)
        {
            var finalPosition = GetFinalPosition(lines);
            return finalPosition.X * finalPosition.Y;
        }

        public static IntVector2 GetFinalPosition(string[] lines)
        {
            var finalPosition = lines.Select(x => x.Split(' ')).Aggregate(new IntVector2(), Advance);
            return finalPosition;
        }

        private static IntVector2 Advance(IntVector2 pos, string[] tokens)
        {
            int amount = int.Parse(tokens[1]);
            return _positionCommands[tokens[0]](pos, amount);
        }

        public int ExecutePart2(string[] lines)
        {
            var finalPosition = GetFinalPositionAdvanced(lines);
            return finalPosition.X * finalPosition.Y;
        }

        public static IntVector2 GetFinalPositionAdvanced(string[] lines)
        {
            var finalState = lines.Select(x => x.Split(' ')).Aggregate(new SubmarineState(), AdvanceWithAim);
            return finalState.Position;
        }

        private static SubmarineState AdvanceWithAim(SubmarineState state, string[] tokens)
        {
            int amount = int.Parse(tokens[1]);
            return _submarineCommands[tokens[0]](state, amount);
        }


        public struct SubmarineState
        {
            public readonly IntVector2 Position;
            private readonly int _aim;

            public SubmarineState(IntVector2 position, int aim)
            {
                Position = position;
                _aim = aim;
            }

            public SubmarineState Aim(int amount)
            {
                return new SubmarineState(Position, _aim + amount);
            }

            public SubmarineState AimAdvance(int amount)
            {
                return new SubmarineState(Position + new IntVector2(1, _aim) * amount, _aim);
            }

            public bool Equals(SubmarineState other)
            {
                return Position.Equals(other.Position) && _aim == other._aim;
            }

            public override bool Equals(object obj)
            {
                return obj is SubmarineState other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Position.GetHashCode() * 397) ^ _aim;
                }
            }
        }
    }
}