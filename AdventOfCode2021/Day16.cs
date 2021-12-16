using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2021 {
    public class Day16 : Exercise {
        public enum PackValueType {
            Sum,
            Product,
            Min,
            Max,
            Literal,
            GreaterThan,
            LessThan,
            EqualTo
        }
        
        public long ExecutePart1(string[] lines) {
            BinStream binStream = new BinStream(new HexHalfHalfByteStream(lines[0]));

            PacketReader packetReader = new PacketReader(binStream);
            Packet packet = packetReader.ReadPacket();
            return packet.Accept(new SumPacketVersionVisitor());
        }

        public long ExecutePart2(string[] lines) {
            BinStream binStream = new BinStream(new HexHalfHalfByteStream(lines[0]));

            PacketReader packetReader = new PacketReader(binStream);
            Packet packet = packetReader.ReadPacket();
            
            return packet.GetValue();
        }
        
        public interface HalfByteStream {
            bool CanRead();
            byte Read();
        }
        
        public class HexHalfHalfByteStream : HalfByteStream {
            private string _text;
            private int _cursor;

            public HexHalfHalfByteStream(string text) {
                _text = text;
            }

            public bool CanRead() {
                return _cursor < _text.Length;
            }

            public byte Read() {
                char c = _text[_cursor++];
                if (c <= '9') {
                    return (byte)(c - '0');
                }
                return (byte)(c - 'A' + 10);
            }
        }
        
        public class BinStream {
            private readonly HalfByteStream _halfByteStream;
            private byte _byte;
            private int _remainingBits;
            
            public int Position { get; private set; }

            public BinStream(HalfByteStream halfByteStream) {
                _halfByteStream = halfByteStream;
            }

            public uint Read(int bitCount) {
                uint result = 0;
                while (bitCount > 0) {
                    if (_remainingBits == 0) {
                        if (!_halfByteStream.CanRead()) {
                            throw new IOException("Reached the end of the stream");
                        }

                        _byte = _halfByteStream.Read();
                        _remainingBits = 4;
                    }
                    
                    if (_remainingBits >= bitCount) {
                        result <<= bitCount;
                        result = (uint)(result | (uint)(_byte >> (_remainingBits - bitCount)) & ((1 << bitCount) - 1));
                        _remainingBits -= bitCount;
                        Position += bitCount;
                        bitCount = 0;
                    } else {
                        result <<= _remainingBits;
                        result |= (uint)(_byte & ((1 << _remainingBits) - 1));
                        bitCount -= _remainingBits;
                        Position += _remainingBits;
                        _remainingBits = 0;
                    }
                }
                return result;
            }
        }
        
        public interface PacketVisitor<T> {
            T Visit(LiteralPacket packet);
            T Visit(OperatorPacket packet);
        }

        public interface Packet {
            uint Version { get; }
            long GetValue();
            T Accept<T>(PacketVisitor<T> visitor);
        }

        public class LiteralPacket : Packet {
            public uint Version { get; }
            private long _value;

            public LiteralPacket(uint version, long value) {
                _value = value;
                Version = version;
            }

            public long GetValue() {
                return _value;
            }

            public T Accept<T>(PacketVisitor<T> visitor) {
                return visitor.Visit(this);
            }

            protected bool Equals(LiteralPacket other) {
                return _value == other._value && Version == other.Version;
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((LiteralPacket)obj);
            }

            public override int GetHashCode() {
                unchecked {
                    return ((int)_value * 397) ^ (int)Version;
                }
            }
        }

        public class OperatorPacket : Packet {
            public readonly IReadOnlyList<Packet> Packets;

            public PackValueType ValueType { get; }

            public OperatorPacket(uint version, PackValueType valueType, List<Packet> packets) {
                ValueType = valueType;
                Packets = packets;
                Version = version;
            }

            public uint Version { get; }
            
            public long GetValue() {
                switch (ValueType) {
                    case PackValueType.Sum:
                        return Packets.Sum(packet => packet.GetValue());
                    case PackValueType.Product:
                        return Packets.Aggregate(1L, (product, packet) => product * packet.GetValue());
                    case PackValueType.Min:
                        return Packets.Min(packet => packet.GetValue());
                    case PackValueType.Max:
                        return Packets.Max(packet => packet.GetValue());
                    case PackValueType.GreaterThan:
                        return Packets[0].GetValue() > Packets[1].GetValue() ? 1 : 0;
                    case PackValueType.LessThan:
                        return Packets[0].GetValue() < Packets[1].GetValue() ? 1 : 0;
                    case PackValueType.EqualTo:
                        return Packets[0].GetValue() == Packets[1].GetValue() ? 1 : 0;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            public T Accept<T>(PacketVisitor<T> visitor) {
                return visitor.Visit(this);
            }
        }

        public class SumPacketVersionVisitor : PacketVisitor<uint> {
            public uint Visit(LiteralPacket packet) {
                return packet.Version;
            }

            public uint Visit(OperatorPacket packet) {
                return (uint)(packet.Version + packet.Packets.Sum(x => x.Accept(this)));
            }
        }

        public class PrintVisitor : PacketVisitor<string> {
            public string Visit(LiteralPacket packet) {
                return packet.GetValue().ToString();
            }

            public string Visit(OperatorPacket packet) {
                switch (packet.ValueType) {
                    case PackValueType.Sum:
                        return "(" + string.Join(" + ", packet.Packets.Select(x => x.Accept(this))) + ")";
                    case PackValueType.Product:
                        return "(" + string.Join(" * ", packet.Packets.Select(x => x.Accept(this))) + ")";
                    case PackValueType.Min:
                        return "Min(" + string.Join(", ", packet.Packets.Select(x => x.Accept(this))) + ")";
                    case PackValueType.Max:
                        return "Max(" + string.Join(", ", packet.Packets.Select(x => x.Accept(this))) + ")";
                    case PackValueType.GreaterThan:
                        return "(" + packet.Packets[0].Accept(this) + " >? " + packet.Packets[1].Accept(this) + ")";
                    case PackValueType.LessThan:
                        return "(" + packet.Packets[0].Accept(this) + " <? " + packet.Packets[1].Accept(this) + ")";
                    case PackValueType.EqualTo:
                        return "(" + packet.Packets[0].Accept(this) + " == " + packet.Packets[1].Accept(this) + ")";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        public class PacketReader {
            private readonly BinStream _binStream;

            public PacketReader(BinStream binStream) {
                _binStream = binStream;
            }

            public Packet ReadPacket() {
                uint versionID = _binStream.Read(3);  
                PackValueType valueType = (PackValueType)_binStream.Read(3);

                if (valueType == PackValueType.Literal) {
                    return new LiteralPacket(versionID, ReadLiteral(_binStream));
                } else {
                    return ReadOperatorPacket(versionID, valueType);
                }
            }

            private Packet ReadOperatorPacket(uint versionId, PackValueType valueType) {
                uint lengthType = _binStream.Read(1);
                List<Packet> packets; 
                if (lengthType == 0) {
                    uint bitCount = _binStream.Read(15);
                    int startPosition = _binStream.Position;
                    packets = new List<Packet>();
                    while (_binStream.Position - startPosition < bitCount) {
                        packets.Add(ReadPacket());
                    }
                } else {
                    uint packetCount = _binStream.Read(11);
                    packets = Enumerable.Range(0, (int)packetCount).Select(x => ReadPacket()).ToList();
                }

                return new OperatorPacket(versionId, valueType, packets);
            }
            
            private long ReadLiteral(BinStream binStream) {
                long result = 0;
                bool hasNextBlock = true;

                while (hasNextBlock) {
                    uint block = binStream.Read(5);
                    result = (result << 4) | (block & 0b1111);
                    hasNextBlock = (block & 0b10000) != 0;
                }

                return result;
            }
        }
    }
}
