using System;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace AdventOfCode2021.Test {
    [TestFixture]
    public class Day18Tests {
        private static readonly string[] _sampleLines = new[] {
            "[[[0,[5,8]],[[1,7],[9,6]]],[[4,[1,2]],[[1,4],2]]]",
            "[[[5,[2,8]],4],[5,[[9,9],0]]]",
            "[6,[[[6,2],[5,6]],[[7,6],[4,7]]]]",
            "[[[6,[0,7]],[0,9]],[4,[9,[9,0]]]]",
            "[[[7,[6,4]],[3,[1,3]]],[[[5,5],1],9]]",
            "[[6,[[7,3],[3,2]]],[[[3,8],[5,7]],4]]",
            "[[[[5,4],[7,7]],8],[[8,3],8]]",
            "[[9,3],[[9,9],[6,[4,9]]]]",
            "[[2,[[7,7],7]],[[5,8],[[9,3],[0,2]]]]",
            "[[[[5,2],5],[8,[3,7]]],[[5,[7,5]],[4,4]]]",
        };

        [Test]
        public void TestParse() {
            Assert.AreEqual(new Day18.Pair(new Day18.Pair(new Day18.NumberNode(1), new Day18.NumberNode(2)), new Day18.NumberNode(3)), Day18.Parse("[[1,2],3"));
        }

        [Test]
        public void TestSplitNumberNodeReturnsNoActionIfValueIsLessThan10() {
            Assert.AreEqual(Day18.NoReduceActionInstance, new Day18.NumberNode(1).Split());
        }

        [Test]
        public void TestSplitNumberNodeReturnsSplitActionIfValueIs10OrMore() {
            Assert.AreEqual(new Day18.ReplaceNodeAction(Day18.Pair.FromNumbers(5, 5)), new Day18.NumberNode(10).Split());
            Assert.AreEqual(new Day18.ReplaceNodeAction(Day18.Pair.FromNumbers(7, 8)), new Day18.NumberNode(15).Split());
        }

        [Test]
        public void TestSplitPairChangesLeftIfLeftReturnsReplaceNodeActionAndReturnsStopReduce() {
            Day18.Pair pair = Day18.Pair.FromNumbers(15, 5);
            Day18.ReduceAction reduceAction = pair.Split();
            Assert.AreEqual(Day18.StopReduceActionInstance, reduceAction);
            Assert.AreEqual(new Day18.Pair(Day18.Pair.FromNumbers(7, 8), new Day18.NumberNode(5)), pair);
        }

        [Test]
        public void TestSplitPairDoesNotCallreduceOnRightIfLeftIsChanged() {
            var rightMock = new Mock<Day18.SnailNode>();
            rightMock.Setup(x => x.Split()).Throws(new Exception("Should not be called"));
            Day18.Pair pair = new Day18.Pair(new Day18.NumberNode(15), rightMock.Object);
            Day18.ReduceAction reduceAction = pair.Split();
            Assert.AreEqual(Day18.StopReduceActionInstance, reduceAction);
            Assert.AreEqual(new Day18.Pair(Day18.Pair.FromNumbers(7, 8), rightMock.Object), pair);
        }

        [Test]
        public void TestSplitPairReturnsStopReduceActionIfLeftNodeReturnsStopReduceAction() {
            var leftMock = new Mock<Day18.SnailNode>();
            leftMock.Setup(x => x.Split()).Returns(Day18.StopReduceActionInstance);
            Day18.Pair pair = new Day18.Pair(leftMock.Object, new Day18.NumberNode(5));
            Day18.ReduceAction reduceAction = pair.Split();
            Assert.AreEqual(Day18.StopReduceActionInstance, reduceAction);
            Assert.AreEqual(new Day18.Pair(leftMock.Object, new Day18.NumberNode(5)), pair);
        }
        
        [Test]
        public void TestSplitPairChangesRightIfRightReturnsReplaceNodeActionAndReturnsStopReduce() {
            Day18.Pair pair = Day18.Pair.FromNumbers(5, 15);
            Day18.ReduceAction reduceAction = pair.Split();
            Assert.AreEqual(Day18.StopReduceActionInstance, reduceAction);
            Assert.AreEqual(new Day18.Pair(new Day18.NumberNode(5), Day18.Pair.FromNumbers(7, 8)), pair);
        }

        [Test]
        public void TestExplodePairReturnsNoReduceActionIfDepthIsLessThan5AndBothNodesReturnsNoReduceAction() {
            FakeNode fakeNode1 = new FakeNode(Day18.NoReduceActionInstance);
            FakeNode fakeNode2 = new FakeNode(Day18.NoReduceActionInstance);
            Day18.Pair pair = new Day18.Pair(fakeNode1, fakeNode2);
            Assert.AreEqual(Day18.NoReduceActionInstance, pair.Explode(2));
            Assert.AreEqual(new Day18.Pair(fakeNode1, fakeNode2), pair);
        }

        [Test]
        public void TestExplodeReturnsExplodeActionIfDepthIs5() {
            Day18.Pair pair = Day18.Pair.FromNumbers(9, 8);
            Assert.AreEqual(new Day18.ExplodeReduceAction(9, 8), pair.Explode(5));
        }

        [Test]
        public void TestPairReplacesLeftWith0AndAddInnerRightToRightNodeAndReturnsAddLeftIfInnerLeftExplodes() {
            Day18.Pair innerPair = Day18.Pair.FromNumbers(9, 6);
            Day18.Pair pair = new Day18.Pair(innerPair, new Day18.NumberNode(1));
            
            Assert.AreEqual(new Day18.AddLeftAction(9), pair.Explode(4));
            Assert.AreEqual(Day18.Pair.FromNumbers(0, 7), pair);
        }
        
        [Test]
        public void TestPairReplacesRightWith0AndAddInnerLeftToLeftNodeAndReturnsAddRightIfInnerRightExplodes() {
            Day18.Pair innerPair = Day18.Pair.FromNumbers(6, 9);
            Day18.Pair pair = new Day18.Pair(new Day18.NumberNode(1), innerPair);
            
            Assert.AreEqual(new Day18.AddRightAction(9), pair.Explode(4));
            Assert.AreEqual(Day18.Pair.FromNumbers(7, 0), pair);
        }

        [Test]
        public void TestPairExplodeAddsValueToRightIfLeftNodeReturnsAddRightActionAndReturnsStop() {
            FakeNode left = new FakeNode(new Day18.AddRightAction(5));
            Day18.Pair pair = new Day18.Pair(left, new Day18.NumberNode(2));
            Assert.AreEqual(Day18.StopReduceActionInstance, pair.Explode(4));
            Assert.AreEqual(new Day18.Pair(left, new Day18.NumberNode(7)), pair);
        }

        [Test]
        public void TestPairExplodeAddsValueToLeftIfRightNodeReturnsAddLeftActionAndReturnsStop() {
            FakeNode right = new FakeNode(new Day18.AddLeftAction(5));
            Day18.Pair pair = new Day18.Pair(new Day18.NumberNode(2), right);
            Assert.AreEqual(Day18.StopReduceActionInstance, pair.Explode(4));
            Assert.AreEqual(new Day18.Pair(new Day18.NumberNode(7), right), pair);
        }
        
        [Test]
        public void TestPairExplodeReturnsAddLeftActionIfLeftReturnsAddLeftAction() {
            Day18.AddLeftAction addLeftAction = new Day18.AddLeftAction(5);
            FakeNode left = new FakeNode(addLeftAction);
            Day18.Pair pair = new Day18.Pair(left, new Day18.NumberNode(2));
            Assert.AreEqual(addLeftAction, pair.Explode(4));
            Assert.AreEqual(new Day18.Pair(left, new Day18.NumberNode(2)), pair);
        }
        
        [Test]
        public void TestPairExplodeReturnsAddRightActionIfRightReturnsAddRightAction() {
            Day18.AddRightAction addRightAction = new Day18.AddRightAction(5);
            FakeNode right = new FakeNode(addRightAction);
            Day18.Pair pair = new Day18.Pair(new Day18.NumberNode(2), right);
            Assert.AreEqual(addRightAction, pair.Explode(4));
            Assert.AreEqual(new Day18.Pair(new Day18.NumberNode(2), right), pair);
        }

        private string Explode(string text) {
            var node = Day18.Parse(text);
            node.Explode(1);
            return node.ToString();
        }
        
        [Test]
        public void TestExplode() {
            Assert.AreEqual("[[[[0,9],2],3],4]", Explode("[[[[[9,8],1],2],3],4]"));
            Assert.AreEqual("[7,[6,[5,[7,0]]]]", Explode("[7,[6,[5,[4,[3,2]]]]]"));
            Assert.AreEqual("[[6,[5,[7,0]]],3]", Explode("[[6,[5,[4,[3,2]]]],1]"));
            Assert.AreEqual("[[3,[2,[8,0]]],[9,[5,[4,[3,2]]]]]", Explode("[[3,[2,[1,[7,3]]]],[6,[5,[4,[3,2]]]]]"));
            Assert.AreEqual("[[3,[2,[8,0]]],[9,[5,[7,0]]]]", Explode("[[3,[2,[8,0]]],[9,[5,[4,[3,2]]]]]"));
        }

        [Test]
        public void TestMagnitude() {
            Assert.AreEqual(8, new Day18.NumberNode(8).GetMagnitude());
            Assert.AreEqual(29, Day18.Pair.FromNumbers(9, 1).GetMagnitude());
            Assert.AreEqual(129, Day18.Parse("[[9,1],[1,9]]").GetMagnitude());
        }

        [Test]
        public void TestAdd() {
            var node = Day18.Add(Day18.Parse("[[[[4,3],4],4],[7,[[8,4],9]]]"), Day18.Parse("[1,1]"));
            Assert.AreEqual("[[[[0,7],4],[[7,8],[6,0]]],[8,1]]", node.ToString());
        }

        [Test]
        public void TestSampleSum() {
            Assert.AreEqual("[[[[6,6],[7,6]],[[7,7],[7,0]]],[[[7,7],[7,7]],[[7,8],[9,9]]]]", Day18.Sum(_sampleLines.Select(Day18.Parse)).ToString());
        }
        
        [Test]
        public void TestPart1() {
            Assert.AreEqual(4140, new Day18().ExecutePart1(_sampleLines));
        }

        [Test]
        public void TestPart2() {
            Assert.AreEqual(3993, new Day18().ExecutePart2(_sampleLines));
        }


        private class FakeNode : Day18.SnailNode {
            private readonly Day18.ReduceAction _reduceAction;

            public FakeNode(Day18.ReduceAction reduceAction) {
                _reduceAction = reduceAction;
            }

            public Day18.ReduceAction Split() {
                return _reduceAction;
            }

            public Day18.ReduceAction Explode(int depth) {
                return _reduceAction;
            }

            public void AddLeft(int value) {
                throw new NotImplementedException();
            }

            public void AddRight(int value) {
                throw new NotImplementedException();
            }

            public int GetMagnitude() {
                throw new NotImplementedException();
            }

            public Day18.SnailNode Clone() {
                throw new NotImplementedException();
            }
        }
    }
}
