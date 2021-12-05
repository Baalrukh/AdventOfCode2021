using System.Collections.Generic;
using AdventOfCode2021.Utils;
using NUnit.Framework;

namespace AdventOfCode2021.Test {
    [TestFixture]
    public class EnumerableUtilsTest{

        [Test]
        public void TestBatchNormal() {
            CollectionAssert.AreEqual(
                new List<List<int>>()
                    { new List<int>() { 1, 2, 3 }, new List<int>() { 4, 5, 6 }, new List<int>() { 7, 8, 9 } },
                new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }.Batch(3));
        }

        [Test]
        public void TestBatchNotEnoughElementsForLast() {
            CollectionAssert.AreEqual(
                new List<List<int>>()
                    { new List<int>() { 1, 2, 3 }, new List<int>() { 4, 5, 6 }, new List<int>() { 7, 8, } },
                new[] { 1, 2, 3, 4, 5, 6, 7, 8, }.Batch(3));
        }
        
        [Test]
        public void TestTakeByColumn() {
            CollectionAssert.AreEqual(
                    new List<int>() { 1, 4, 7 },
                new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12}.TakeByColumn(3, 3));
        }
        
        [Test]
        public void TestTakeByColumnNotEnoughElements() {
            CollectionAssert.AreEqual(
                    new List<int>() { 1, 5 },
                new[] { 1, 2, 3, 4, 5, 6, 7, 8}.TakeByColumn(3, 4));
        }
    }
}