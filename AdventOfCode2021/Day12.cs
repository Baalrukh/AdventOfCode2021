using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode2021.Utils;

namespace AdventOfCode2021 {
    public class Day12 : Exercise {
        public long ExecutePart1(string[] lines) {
            var nodes = Parse(lines);
            return EnumerateAllPaths(nodes, 0).Count();
        }

        public long ExecutePart2(string[] lines) {
            var nodes = Parse(lines);
            return EnumerateAllPaths(nodes, 1).Count();
        }


        public static IEnumerable<List<Node>> EnumerateAllPaths(Dictionary<string, Node> nodes, int smallNodeRevisitCount) {
            Node node = nodes["start"];

            foreach (var path in EnumerateAllPaths(node, new List<Node>(), smallNodeRevisitCount)) {
                yield return path;
            }
        }

        private static IEnumerable<List<Node>> EnumerateAllPaths(Node node, List<Node> path, int smallNodeRevisitCount) {
            path.Add(node);
            if (node.NodeType == NodeType.End) {
                yield return path;
                yield break;
            }
            foreach (Node neighbour in node.Neighbours) {
                if (neighbour.NodeType == NodeType.Start) {
                    continue;
                }
                int nextSmallNodeRevisitCount = smallNodeRevisitCount;
                if ((neighbour.NodeType == NodeType.Small) && path.Contains(neighbour)) {
                    if (nextSmallNodeRevisitCount == 0) {
                        continue;
                    }
                    nextSmallNodeRevisitCount--;
                }

                var subPath = new List<Node>(path);
                foreach (var foundPath in EnumerateAllPaths(neighbour, subPath, nextSmallNodeRevisitCount)) {
                    yield return foundPath;
                }
            }
        }

        public static Dictionary<string, Node> Parse(string[] lines) {
            var tokenizedLines = lines.Select(x => x.Split('-')).ToList();
            var nodes = tokenizedLines.SelectMany(x => x)
                .Distinct()
                .ToDictionary(x => x, x => new Node(x, GetNodeType(x)));

            foreach (string[] tokens in tokenizedLines) {
                Node node1 = nodes[tokens[0]];
                Node node2 = nodes[tokens[1]];
                node1.Neighbours.Add(node2);
                node2.Neighbours.Add(node1);
            }

            return nodes;
        }

        private static NodeType GetNodeType(string name) {
            switch (name) {
                case "start": return NodeType.Start;
                case "end": return NodeType.End;
                default:
                    return char.IsUpper(name[0]) ? NodeType.Large : NodeType.Small;
            }
        }

        public enum NodeType {
            Start,
            End,
            Small,
            Large
        }
        
        public class Node {
            public readonly string Name;
            public readonly NodeType NodeType;
            public readonly List<Node> Neighbours = new List<Node>();

            public Node(string name, NodeType nodeType) {
                Name = name;
                NodeType = nodeType;
            }

            protected bool Equals(Node other) {
                return Name == other.Name && NodeType == other.NodeType && Neighbours.SequenceEqual(other.Neighbours);
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Node)obj);
            }

            public override string ToString() {
                string neighboursNames = string.Join(",", Neighbours.Select(x => x.Name));
                return $"{Name} ({neighboursNames})";
            }

            public override int GetHashCode() {
                unchecked {
                    int hashCode = (Name != null ? Name.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ NodeType.GetHashCode();
                    hashCode = (hashCode * 397) ^ Neighbours.GetCollectionHashCode();
                    return hashCode;
                }
            }
        }

    }
}
