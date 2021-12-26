using AdventOfCode2021;
using NUnit.Framework;

namespace AdventOfCode2021.Test {
    [TestFixture]
    public class Day24Tests {
        private static readonly string[] _sampleLines = new[] {
            ""
        };

        [Test]
        public void TestALU_1() {
            string[] lines = { "inp x", "mul x -1" };
            Day24.ALUProgram aluProgram = Day24.ALUProgram.Parse(lines);
            Day24.ALU alu = aluProgram.Execute("5");
            Assert.AreEqual(-5, alu.Registers[(int)Day24.Register.X]);
        }

        [Test]
        public void TestALU_2() {
            string[] lines = { "inp z",
                "inp x",
                "mul z 3",
                "eql z x", };
            Day24.ALUProgram aluProgram = Day24.ALUProgram.Parse(lines);
            Assert.AreEqual(1, aluProgram.Execute("13").Registers[(int)Day24.Register.Z]);
            Assert.AreEqual(0, aluProgram.Execute("14").Registers[(int)Day24.Register.Z]);
        }

    }
}
