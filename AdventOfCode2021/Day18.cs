using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2021 {
    public class Day18 : Exercise {
        public long ExecutePart1(string[] lines) {
            return Sum(lines.Select(Parse)).GetMagnitude();
        }

        public long ExecutePart2(string[] lines) {
            var snailNodes = lines.Select(Parse).ToList();

            int maxMagnitude = 0;
            for (int y = 0; y < snailNodes.Count; y++) {
                for (int x = 0; x < snailNodes.Count; x++) {
                    if (x != y) {
                        SnailNode left = snailNodes[x];
                        SnailNode right = snailNodes[y];
                        int magnitude = Add(left, right).GetMagnitude();
                        if (magnitude > maxMagnitude) {
                            maxMagnitude = magnitude;
                        }
                    }
                }
            }
            
            return maxMagnitude;
        }

        public static SnailNode Sum(IEnumerable<SnailNode> nodes) {
            return nodes.Aggregate((SnailNode)null, (resultNode, node) => resultNode == null ? node : Add(resultNode, node));
        }
        

        public interface SnailNode {
            ReduceAction Split();
            ReduceAction Explode(int depth);
            void AddLeft(int value);
            void AddRight(int value);
            int GetMagnitude();
            SnailNode Clone();
        }

        public class Pair : SnailNode {
            public SnailNode Left { get; private set; }
            public SnailNode Right { get; private set; }

            public Pair(SnailNode left, SnailNode right) {
                Left = left;
                Right = right;
            }

            protected bool Equals(Pair other) {
                return Equals(Left, other.Left) && Equals(Right, other.Right);
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Pair)obj);
            }

            public override int GetHashCode() {
                unchecked {
                    return ((Left != null ? Left.GetHashCode() : 0) * 397) ^ (Right != null ? Right.GetHashCode() : 0);
                }
            }

            public ReduceAction Split() {
                switch (Left.Split()) {
                    case ReplaceNodeAction replaceNodeAction:
                        Left = replaceNodeAction.Node;
                        return StopReduceActionInstance;
                    case StopReduceAction _:
                        return StopReduceActionInstance;
                    case NoReduceAction _:
                        break;
                    default:
                        throw new NotImplementedException();
                }
                
                switch (Right.Split()) {
                    case ReplaceNodeAction replaceNodeAction:
                        Right = replaceNodeAction.Node;
                        return StopReduceActionInstance;
                    case StopReduceAction _:
                        return StopReduceActionInstance;
                    case NoReduceAction _:
                        return NoReduceActionInstance;
                    default:
                        throw new NotImplementedException();
                }
            }

            public ReduceAction Explode(int depth) {
                if (depth < 5) {
                    switch (Left.Explode(depth + 1)) {
                        case NoReduceAction _:
                            break;
                        case ExplodeReduceAction explodeReduceAction:
                            Left = new NumberNode(0);
                            Right.AddLeft(explodeReduceAction.Right);
                            return new AddLeftAction(explodeReduceAction.Left);
                        case AddRightAction addRightAction:
                            Right.AddLeft(addRightAction.Value);
                            return StopReduceActionInstance;
                        case AddLeftAction addLeftAction:
                            return addLeftAction;
                        case StopReduceAction _:
                            return StopReduceActionInstance;
                        default:
                            throw new NotImplementedException();
                    }

                    switch (Right.Explode(depth + 1)) {
                        case NoReduceAction _:
                            break;
                        case ExplodeReduceAction explodeReduceAction:
                            Left.AddRight(explodeReduceAction.Left);
                            Right = new NumberNode(0);
                            return new AddRightAction(explodeReduceAction.Right);
                        case AddLeftAction addLeftAction:
                            Left.AddRight(addLeftAction.Value);
                            return StopReduceActionInstance;
                        case AddRightAction addRightAction:
                            return addRightAction;
                        case StopReduceAction _:
                            return StopReduceActionInstance;
                        default:
                            throw new NotImplementedException();
                    }

                    return NoReduceActionInstance;
                }

                if (!(Left is NumberNode leftNumber) || !(Right is NumberNode rightNumber)) {
                    throw new ArgumentException("Expecting both nodes to be numbers");
                }

                return new ExplodeReduceAction(leftNumber.Value, rightNumber.Value);
            }

            public void AddLeft(int value) {
                Left.AddLeft(value);
            }

            public void AddRight(int value) {
                Right.AddRight(value);
            }

            public int GetMagnitude() {
                return Left.GetMagnitude() * 3 + Right.GetMagnitude() * 2;
            }

            public SnailNode Clone() {
                return new Pair(Left.Clone(), Right.Clone());
            }

            public override string ToString() {
                return $"[{Left},{Right}]";
            }

            public static Pair FromNumbers(int left, int right) {
                return new Pair(new NumberNode(left), new NumberNode(right));
            }
        }

        public class NumberNode : SnailNode {
            public int Value { get; private set; }

            public NumberNode(int value) {
                Value = value;
            }

            protected bool Equals(NumberNode other) {
                return Value == other.Value;
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((NumberNode)obj);
            }

            public override int GetHashCode() {
                return Value;
            }

            public ReduceAction Split() {
                if (Value >= 10) {
                    int half = Value / 2;
                    return new ReplaceNodeAction(Pair.FromNumbers(half, Value - half));
                }
                return NoReduceActionInstance;
            }

            public ReduceAction Explode(int depth) {
                return NoReduceActionInstance;
            }

            public void AddLeft(int value) {
                Value += value;
            }

            public void AddRight(int value) {
                Value += value;
            }

            public int GetMagnitude() {
                return Value;
            }

            public SnailNode Clone() {
                return new NumberNode(Value);
            }

            public override string ToString() {
                return Value.ToString();
            }
        }

        public interface ReduceAction {
            
        }
        public class NoReduceAction : ReduceAction {}
        public class StopReduceAction : ReduceAction {}

        public class ReplaceNodeAction : ReduceAction {
            public readonly SnailNode Node;

            public ReplaceNodeAction(SnailNode node) {
                Node = node;
            }

            protected bool Equals(ReplaceNodeAction other) {
                return Equals(Node, other.Node);
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((ReplaceNodeAction)obj);
            }

            public override int GetHashCode() {
                return (Node != null ? Node.GetHashCode() : 0);
            }
        }

        public class ExplodeReduceAction : ReduceAction {
            public readonly int Left;
            public readonly int Right;

            public ExplodeReduceAction(int left, int right) {
                Left = left;
                Right = right;
            }

            protected bool Equals(ExplodeReduceAction other) {
                return Left == other.Left && Right == other.Right;
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((ExplodeReduceAction)obj);
            }

            public override int GetHashCode() {
                unchecked {
                    return (Left * 397) ^ Right;
                }
            }
        }

        public class AddLeftAction : ReduceAction {
            public readonly int Value;

            public AddLeftAction(int value) {
                Value = value;
            }

            protected bool Equals(AddLeftAction other) {
                return Value == other.Value;
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((AddLeftAction)obj);
            }

            public override int GetHashCode() {
                return Value;
            }
        }

        public class AddRightAction : ReduceAction {
            public readonly int Value;

            public AddRightAction(int value) {
                Value = value;
            }

            protected bool Equals(AddRightAction other) {
                return Value == other.Value;
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((AddRightAction)obj);
            }

            public override int GetHashCode() {
                return Value;
            }
        }
        
        public static readonly NoReduceAction NoReduceActionInstance = new NoReduceAction();
        public static readonly StopReduceAction StopReduceActionInstance = new StopReduceAction();
        
        
        public static SnailNode Parse(string text) {
            int i = 0;
            return Parse(text, ref i);
        }

        private static SnailNode Parse(string text, ref int i) {
            if (text[i] == '[') {
                i++;
                SnailNode left = Parse(text, ref i);
                i++; // ,
                SnailNode right = Parse(text, ref i);
                i++; // ]
                return new Pair(left, right);
            }

            return new NumberNode(text[i++] - '0');
        }

        public static SnailNode Add(SnailNode left, SnailNode right) {
            Pair node = new Pair(left.Clone(), right.Clone());
            return Reduce(node);
        }
        
        public static SnailNode Reduce(SnailNode node) {
            ReduceAction reduceAction;
            do {
                while (node.Explode(1) != NoReduceActionInstance) {
                }

                reduceAction = node.Split();
            } while (reduceAction != NoReduceActionInstance);

            return node;
        }
    }
}
