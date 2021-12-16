using System.Collections.Generic;
using NUnit.Framework;

namespace AdventOfCode2021.Test {
    [TestFixture]
    public class Day16Tests {
        private static Day16.BinStream CreateBinStream(string text) {
            Day16.BinStream binStream = new Day16.BinStream(new Day16.HexHalfHalfByteStream(text));
            return binStream;
        }

        [Test]
        public void TestHexByteStream() {
            var stream = new Day16.HexHalfHalfByteStream("05A");
            Assert.IsTrue(stream.CanRead());
            Assert.AreEqual(0, stream.Read());
            Assert.IsTrue(stream.CanRead());
            Assert.AreEqual(5, stream.Read());
            Assert.IsTrue(stream.CanRead());
            Assert.AreEqual(10, stream.Read());
            Assert.IsFalse(stream.CanRead());
        }
        
        [Test]
        public void TestBinStream_NoOverflow() {
            // 110100101111111000101000
            Day16.BinStream binStream = CreateBinStream("D2FE28");
            Assert.AreEqual(0b11, binStream.Read(2));
            Assert.AreEqual(0b0, binStream.Read(1));
        }

        [Test]
        public void TestBinStream_Overflow() {
            // 110 1001 01111111000101000
            Day16.BinStream binStream = CreateBinStream("D2FE28");
            binStream.Read(3);
            Assert.AreEqual(0b1001, binStream.Read(4));
        }
        
        [Test]
        public void TestBinStream_MultiByteValue() {
            // 110 1001011 1111100 0101000
            Day16.BinStream binStream = CreateBinStream("D2FE28");
            binStream.Read(3);
            Assert.AreEqual(0b1001011, binStream.Read(7));
        }

        [Test]
        public void TestReadLiteral() {
            Day16.PacketReader packetReader = new Day16.PacketReader(CreateBinStream("D2FE28"));
            Assert.AreEqual(new Day16.LiteralPacket(6, 2021), packetReader.ReadPacket());
        }

        [Test]
        public void TestReadOperator_BitCount() {
            byte[] data = new byte[] {0b0011, 0b1000, 0b0000, 0b0000, 0b0110, 0b1111, 0b0100, 0b0101, 0b0010, 0b1001, 0b0001, 0b0010, 0b0000, 0b0000};
            Day16.BinStream binStream = new Day16.BinStream(new NumericHalfByteStream(data));
            Day16.PacketReader packetReader = new Day16.PacketReader(binStream);
            Day16.OperatorPacket packet = packetReader.ReadPacket() as Day16.OperatorPacket;
            Assert.NotNull(packet);
            Assert.AreEqual(2, packet.Packets.Count);
        }

        [Test]
        public void TestReadOperator_PacketCount() {
            byte[] data = new byte[] {0b1110, 0b1110, 0b0000, 0b0000, 0b1101, 0b0100, 0b0000, 0b1100, 0b1000, 0b0010, 0b0011, 0b0000, 0b0110, 0b0000};
            Day16.BinStream binStream = new Day16.BinStream(new NumericHalfByteStream(data));
            Day16.PacketReader packetReader = new Day16.PacketReader(binStream);
            Day16.OperatorPacket packet = packetReader.ReadPacket() as Day16.OperatorPacket;
            Assert.NotNull(packet);
            Assert.AreEqual(3, packet.Packets.Count);
        }
        
        [Test]
        public void TestPart1() {
            Assert.AreEqual(16, new Day16().ExecutePart1(new [] {"8A004A801A8002F478"}));
            Assert.AreEqual(12, new Day16().ExecutePart1(new [] {"620080001611562C8802118E34"}));
            Assert.AreEqual(23, new Day16().ExecutePart1(new [] {"C0015000016115A2E0802F182340"}));
            Assert.AreEqual(31, new Day16().ExecutePart1(new [] {"A0016C880162017C3686B18A3D4780"}));
        }

        [Test]
        public void TestPart2() {
            Assert.AreEqual(3, new Day16().ExecutePart2(new[] { "C200B40A82" }));
            Assert.AreEqual(54, new Day16().ExecutePart2(new[] { "04005AC33890" }));
            Assert.AreEqual(7, new Day16().ExecutePart2(new[] { "880086C3E88112" }));
            Assert.AreEqual(9, new Day16().ExecutePart2(new[] { "CE00C43D881120" }));
            Assert.AreEqual(1, new Day16().ExecutePart2(new[] { "D8005AC2A8F0" }));
            Assert.AreEqual(0, new Day16().ExecutePart2(new[] { "F600BC2D8F" }));
            Assert.AreEqual(0, new Day16().ExecutePart2(new[] { "9C005AC2F8F0" }));
            Assert.AreEqual(1, new Day16().ExecutePart2(new[] { "9C0141080250320F1802104A08" }));
        }


        private class NumericHalfByteStream : Day16.HalfByteStream {
            private readonly IReadOnlyList<byte> _bytes;
            private int _index;

            public NumericHalfByteStream(IReadOnlyList<byte> bytes) {
                _bytes = bytes;
            }

            public bool CanRead() {
                return _index < _bytes.Count;
            }

            public byte Read() {
                return _bytes[_index++];
            }
        }
    }
}
