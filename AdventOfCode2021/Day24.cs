using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2021 {
    public class Day24 : Exercise {
        public long ExecutePart1(string[] lines) {
            string input = "13579246899999";
            ALUProgram aluProgram = ALUProgram.Parse(lines);
            ReductionContext reductionContext = aluProgram.Reduce();

            ReducedInstruction zInstruction = reductionContext.RegisterContents[(int)Register.Z];
            Console.WriteLine("Z = " + zInstruction);

            HashSet<string> printedVariables = new HashSet<string>();
            void VariableVisitor(string name) {
                if (printedVariables.Contains(name)) {
                    return;
                }

                printedVariables.Add(name);
                ReducedInstruction variable = reductionContext.LocalVariables[name];
                Console.WriteLine(name + " = " + variable);
                variable.VisitUsedVariables(VariableVisitor);
            }

            zInstruction.VisitUsedVariables(VariableVisitor);
            return -1;
        }

        public long ExecutePart2(string[] lines) {
            return -2;
        }

        public enum Register {
            X, Y, Z, W
        }

        public class ALU {
            public int[] Registers = new int[4];

            public Queue<int> Input;

            public ALU(IEnumerable<char> input) {
                Input = new Queue<int>(input.Select(x => x - '0'));
            }

            public override string ToString() {
                return $"[X={Registers[0]}, Y={Registers[1]}, Z={Registers[2]}, W={Registers[3]}]";
            }
        }

        public class ReductionContext {
            public ReducedInstruction[] RegisterContents = new ReducedInstruction[] {
                LiteralReducedInstruction.Zero,
                LiteralReducedInstruction.Zero,
                LiteralReducedInstruction.Zero,
                LiteralReducedInstruction.Zero,
            };

            public Dictionary<string, ReducedInstruction>
                LocalVariables = new Dictionary<string, ReducedInstruction>();

            private int _nextLocalVariableIndex;
            public int InputIndex;

            public ReducedInstruction AddTmpVariable(Register register) {
                ReducedInstruction instruction = RegisterContents[(int)register];
                if (instruction is VariableReference) {
                    return instruction;
                }

                if (instruction is LiteralReducedInstruction literal) {
                    string existingVariable = LocalVariables
                        .Where(p => (p.Value as LiteralReducedInstruction)?.Value == literal.Value)
                        .Select(x => x.Key)
                        .FirstOrDefault();
                    if (existingVariable != null) {
                        return new VariableReference(existingVariable, literal.EstimateValue());
                    }
                }
                string name = "var" + _nextLocalVariableIndex++;
                LocalVariables.Add(name, instruction);
                RegisterContents[(int)register] = new VariableReference(name, instruction.EstimateValue());
                return RegisterContents[(int)register];
            }
        }

        public interface ALUSource {
            int GetValue(ALU alu);
            ReducedInstruction Reduce(ReductionContext reductionContext);
        }

        public class ValueSource : ALUSource {
            private int _value;

            public int value => _value;

            public ValueSource(int value) {
                _value = value;
            }

            public int GetValue(ALU alu) {
                return _value;
            }

            public ReducedInstruction Reduce(ReductionContext reductionContext) {
                return new LiteralReducedInstruction(_value);
            }

            public override string ToString() => _value.ToString();
        }

        public class RegisterSource : ALUSource {
            private Register _register;

            public Register register => _register;

            public RegisterSource(Register register) {
                _register = register;
            }

            public int GetValue(ALU alu) {
                return alu.Registers[(int)_register];
            }

            public ReducedInstruction Reduce(ReductionContext reductionContext) {
                return reductionContext.AddTmpVariable(_register);
            }

            public override string ToString() => _register.ToString();
        }


        public interface Instruction {
            void Execute(ALU alu);
            void Reduce(ReductionContext reductionContext);
        }

        public class Input : Instruction {
            private Register _register;
            private int InputIndex;

            public Input(Register register) {
                _register = register;
            }

            public void Execute(ALU alu) {
                alu.Registers[(int)_register] = alu.Input.Dequeue();
            }

            public void Reduce(ReductionContext reductionContext) {
                reductionContext.RegisterContents[(int)_register] = new InputReducedInstruction(reductionContext.InputIndex++);
            }

            public override string ToString() => $"inp {_register}";
        }



        public class AddValue : Instruction {
            private Register _destinationRegister;
            private ALUSource _source;

            public AddValue(Register destinationRegister, ALUSource source) {
                _destinationRegister = destinationRegister;
                _source = source;
            }

            public void Execute(ALU alu) {
                alu.Registers[(int)_destinationRegister] += _source.GetValue(alu);
            }

            public void Reduce(ReductionContext reductionContext) {
                ReducedInstruction reducedSource = _source.Reduce(reductionContext);
                ValueEstimate sourceEstimate = reducedSource.EstimateValue();
                if (sourceEstimate.Equals(LiteralValueEstimate.Zero)) {
                    return;
                }

                ValueEstimate estimateValue = reductionContext.RegisterContents[(int)_destinationRegister].EstimateValue();
                if (estimateValue.Equals(LiteralValueEstimate.Zero)) {
                    reductionContext.RegisterContents[(int)_destinationRegister] = reducedSource;
                    return;
                }

                if ((estimateValue is LiteralValueEstimate lit1) && (sourceEstimate is LiteralValueEstimate lit2)) {
                    reductionContext.RegisterContents[(int)_destinationRegister] =
                        new LiteralReducedInstruction(lit1.Value + lit2.Value);
                } else {
                    reductionContext.RegisterContents[(int)_destinationRegister]
                        = new ReducedAddInstruction(reductionContext.RegisterContents[(int)_destinationRegister],
                                                    reducedSource);
                }
            }

            public override string ToString() => $"add {_destinationRegister} {_source}";
        }

        public class MultiplyValue : Instruction {
            private Register _destinationRegister;
            private ALUSource _source;

            public MultiplyValue(Register destinationRegister, ALUSource source) {
                _destinationRegister = destinationRegister;
                _source = source;
            }

            public void Execute(ALU alu) {
                alu.Registers[(int)_destinationRegister] *= _source.GetValue(alu);
            }

            public void Reduce(ReductionContext reductionContext) {
                ReducedInstruction reducedSource = _source.Reduce(reductionContext);
                ValueEstimate estimateValue = reducedSource.EstimateValue();
                if (estimateValue.Equals(LiteralValueEstimate.One)) {
                    return;
                }
                if (estimateValue.Equals(LiteralValueEstimate.Zero)) {
                    reductionContext.RegisterContents[(int)_destinationRegister] = LiteralReducedInstruction.Zero;
                    return;
                }

                ValueEstimate dstEstimate = reductionContext.RegisterContents[(int)_destinationRegister].EstimateValue();
                if (dstEstimate.Equals(LiteralValueEstimate.Zero)) {
                } else if (dstEstimate.Equals(LiteralValueEstimate.One)) {
                    reductionContext.RegisterContents[(int)_destinationRegister] = LiteralReducedInstruction.Zero;
                    reductionContext.RegisterContents[(int)_destinationRegister] = reducedSource;
                } else {
                    reductionContext.RegisterContents[(int)_destinationRegister]
                        = new ReducedMultiplyInstruction(reductionContext.RegisterContents[(int)_destinationRegister],
                                                         reducedSource);
                }
            }

            public override string ToString() => $"mul {_destinationRegister} {_source}";
        }

        public class DivideValue : Instruction {
            private Register _destinationRegister;
            private ALUSource _source;

            public DivideValue(Register destinationRegister, ALUSource source) {
                _destinationRegister = destinationRegister;
                _source = source;
            }

            public void Execute(ALU alu) {
                alu.Registers[(int)_destinationRegister] /= _source.GetValue(alu);
            }

            public void Reduce(ReductionContext reductionContext) {
                ReducedInstruction reducedSource = _source.Reduce(reductionContext);
                ValueEstimate srcEstimate = reducedSource.EstimateValue();
                ReducedInstruction dstInstruction = reductionContext.RegisterContents[(int)_destinationRegister];
                if (srcEstimate is LiteralValueEstimate srcLiteral) {
                    if (srcLiteral.Value == 1) {
                        return;
                    }

                    ValueEstimate estimateValue = dstInstruction.EstimateValue();
                    if (estimateValue is LiteralValueEstimate literal) {
                        reductionContext.RegisterContents[(int)_destinationRegister] =
                            new LiteralReducedInstruction(literal.Value / srcLiteral.Value);
                        return;
                    }

                    if (estimateValue is RangeEstimate rangeEstimate) {
                        int minDiv = rangeEstimate.Min / srcLiteral.Value;
                        int maxDiv = rangeEstimate.Max / srcLiteral.Value;
                        if (minDiv == maxDiv) {
                            reductionContext.RegisterContents[(int)_destinationRegister] =
                                new LiteralReducedInstruction(minDiv);
                        }
                    }
                }

                reductionContext.RegisterContents[(int)_destinationRegister]
                    = new ReducedDivideInstruction(dstInstruction,
                                                reducedSource);
            }

            public override string ToString() => $"div {_destinationRegister} {_source}";
        }

        public class ModuloValue : Instruction {
            private Register _destinationRegister;
            private ALUSource _source;

            public ModuloValue(Register destinationRegister, ALUSource source) {
                _destinationRegister = destinationRegister;
                _source = source;
            }

            public void Execute(ALU alu) {
                alu.Registers[(int)_destinationRegister] %= _source.GetValue(alu);
            }

            public void Reduce(ReductionContext reductionContext) {
                if (reductionContext.RegisterContents[(int)_destinationRegister] == LiteralReducedInstruction.Zero) {
                    return;
                }

                reductionContext.RegisterContents[(int)_destinationRegister]
                    = new ReducedModuloInstruction(reductionContext.RegisterContents[(int)_destinationRegister],
                                                _source.Reduce(reductionContext));
            }

            public override string ToString() => $"mod {_destinationRegister} {_source}";
        }

        public class EqualValue : Instruction {
            private Register _destinationRegister;
            private ALUSource _source;

            public EqualValue(Register destinationRegister, ALUSource source) {
                _destinationRegister = destinationRegister;
                _source = source;
            }

            public void Execute(ALU alu) {
                alu.Registers[(int)_destinationRegister] =
                    alu.Registers[(int)_destinationRegister] == _source.GetValue(alu) ? 1 : 0;
            }

            public void Reduce(ReductionContext reductionContext) {
                ReducedInstruction reducedSource = _source.Reduce(reductionContext);
                ValueEstimate dstEstimate = reductionContext.RegisterContents[(int)_destinationRegister].EstimateValue();
                ValueEstimate sourceEstimate = reducedSource.EstimateValue();
                if (sourceEstimate.Equals(dstEstimate)) {
                    reductionContext.RegisterContents[(int)_destinationRegister] = LiteralReducedInstruction.Zero;
                } else if (!sourceEstimate.Intersect(dstEstimate)) {
                    reductionContext.RegisterContents[(int)_destinationRegister] = LiteralReducedInstruction.One;
                } else {
                    reductionContext.RegisterContents[(int)_destinationRegister]
                        = new ReducedEqualInstruction(reductionContext.RegisterContents[(int)_destinationRegister],
                                                      _source.Reduce(reductionContext));
                }
            }

            public override string ToString() => $"eq {_destinationRegister} {_source}";
        }

        public class ALUProgram {
            private List<Instruction> _instructions;

            public ALUProgram(List<Instruction> instructions) {
                _instructions = instructions;
            }

            public ALU Execute(string input) {
                ALU alu = new ALU(input);
                foreach (Instruction instruction in _instructions) {
                    instruction.Execute(alu);
                }

                return alu;
            }

            public ReductionContext Reduce() {
                ReductionContext reductionContext = new ReductionContext();
                for (int i = 0; i < _instructions.Count; i++) {
                    Instruction instruction = _instructions[i];
                    instruction.Reduce(reductionContext);
                }

                return reductionContext;
            }

            public static ALUProgram Parse(string[] lines) {
                return new ALUProgram(lines.Select(ParseInstruction).ToList());
            }

            private static Instruction ParseInstruction(string line) {
                string[] tokens = line.Split(' ');
                switch (tokens[0]) {
                    case "inp": return new Input(ParseRegister(tokens[1]));
                    case "add": return new AddValue(ParseRegister(tokens[1]), ParseRegisterOrValue(tokens[2]));
                    case "mul": return new MultiplyValue(ParseRegister(tokens[1]), ParseRegisterOrValue(tokens[2]));
                    case "div": return new DivideValue(ParseRegister(tokens[1]), ParseRegisterOrValue(tokens[2]));
                    case "mod": return new ModuloValue(ParseRegister(tokens[1]), ParseRegisterOrValue(tokens[2]));
                    case "eql": return new EqualValue(ParseRegister(tokens[1]), ParseRegisterOrValue(tokens[2]));
                    default:
                        throw new Exception("Unknown instruction : " + line);
                }
            }

            private static Register ParseRegister(string token) {
                switch (token) {
                    case "x" : return Register.X;
                    case "y" : return Register.Y;
                    case "z" : return Register.Z;
                    case "w" : return Register.W;
                    default: throw new Exception("Unknown register : " + token);
                }
            }

            private static ALUSource ParseRegisterOrValue(string token) {
                switch (token) {
                    case "x" : return new RegisterSource(Register.X);
                    case "y" : return new RegisterSource(Register.Y);
                    case "z" : return new RegisterSource(Register.Z);
                    case "w" : return new RegisterSource(Register.W);
                    default:
                        return new ValueSource(int.Parse(token));
                }
            }
        }

        public interface ValueEstimate {
            ValueEstimate Add(ValueEstimate other);
            ValueEstimate Multiply(ValueEstimate other);
            ValueEstimate Divide(ValueEstimate other);
            ValueEstimate Modulo(ValueEstimate other);
            ValueEstimate IsEqual(ValueEstimate other);
            bool Intersect(ValueEstimate other);
        }

        public class LiteralValueEstimate : ValueEstimate {
            public readonly int Value;

            public static readonly LiteralValueEstimate Zero = new LiteralValueEstimate(0);
            public static readonly LiteralValueEstimate One = new LiteralValueEstimate(1);

            public LiteralValueEstimate(int value) {
                Value = value;
            }

            protected bool Equals(LiteralValueEstimate other) {
                return Value == other.Value;
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) {
                    return false;
                }

                if (ReferenceEquals(this, obj)) {
                    return true;
                }

                if (obj.GetType() != this.GetType()) {
                    return false;
                }

                return Equals((LiteralValueEstimate)obj);
            }

            public override int GetHashCode() {
                return Value;
            }

            public ValueEstimate Add(ValueEstimate other) {
                if (other is LiteralValueEstimate literal) {
                    return new LiteralValueEstimate(Value + literal.Value);
                }

                return other.Add(this);
            }

            public ValueEstimate Multiply(ValueEstimate other) {
                if (other is LiteralValueEstimate literal) {
                    return new LiteralValueEstimate(Value * literal.Value);
                }

                return other.Multiply(this);
            }

            public ValueEstimate Divide(ValueEstimate other) {
                if (other is LiteralValueEstimate literal) {
                    return new LiteralValueEstimate(Value / literal.Value);
                }

                RangeEstimate rangeEstimate = (RangeEstimate)other;
                return new RangeEstimate(Value / rangeEstimate.Max, Value / rangeEstimate.Min);
            }

            public ValueEstimate Modulo(ValueEstimate other) {
                if (other is LiteralValueEstimate literal) {
                    return new LiteralValueEstimate(Value % literal.Value);
                }

                RangeEstimate rangeEstimate = (RangeEstimate)other;
                return new RangeEstimate(0, rangeEstimate.Max);
            }

            public ValueEstimate IsEqual(ValueEstimate other) {
                if (other is LiteralValueEstimate literal) {
                    return new LiteralValueEstimate(Value == literal.Value ? 0 : 1);
                }

                return other.IsEqual(this);
            }

            public bool Intersect(ValueEstimate other) {
                if (other is LiteralValueEstimate literal) {
                    return Value == literal.Value;
                }

                return other.Intersect(this);
            }
        }

        public class RangeEstimate : ValueEstimate {
            public readonly int Min;
            public readonly int Max;

            public RangeEstimate(int min, int max) {
                Min = min;
                Max = max;
            }

            public ValueEstimate Add(ValueEstimate other) {
                if (other is LiteralValueEstimate literal) {
                    return new RangeEstimate(Min + literal.Value, Max + literal.Value);
                }

                RangeEstimate range = (RangeEstimate)other;
                return new RangeEstimate(Min + range.Min, Max + range.Max);
            }

            public ValueEstimate Multiply(ValueEstimate other) {
                if (other is LiteralValueEstimate literal) {
                    return new RangeEstimate(Min * literal.Value, Max * literal.Value);
                }

                RangeEstimate range = (RangeEstimate)other;
                return new RangeEstimate(Min * range.Min, Max * range.Max);
            }

            public ValueEstimate Divide(ValueEstimate other) {
                if (other is LiteralValueEstimate literal) {
                    return new RangeEstimate(Min / literal.Value, Max / literal.Value);
                }

                RangeEstimate range = (RangeEstimate)other;
                return new RangeEstimate(Min / range.Max, Max / range.Min);
            }

            public ValueEstimate Modulo(ValueEstimate other) {
                if (other is LiteralValueEstimate literal) {
                    if (Max < literal.Value) {
                        return this;
                    }
                    return new RangeEstimate(0, literal.Value - 1);
                }

                RangeEstimate range = (RangeEstimate)other;
                return new RangeEstimate(0, range.Max);
            }

            public ValueEstimate IsEqual(ValueEstimate other) {
                return new RangeEstimate(0, 1);
            }

            public bool Intersect(ValueEstimate other) {
                if (other is LiteralValueEstimate literal) {
                    return (literal.Value >= Min) && (literal.Value <= Max);
                }

                RangeEstimate range = (RangeEstimate)other;
                return (range.Max >= Min) && (range.Min <= Max);
            }
        }

        public interface ReducedInstruction {
            ValueEstimate EstimateValue();
            void VisitUsedVariables(Action<string> variableVisitor);
        }

        public class LiteralReducedInstruction : ReducedInstruction {
            public readonly int Value;

            public LiteralReducedInstruction(int value) {
                Value = value;
            }

            public override string ToString() {
                return Value.ToString();
            }

            public ValueEstimate EstimateValue() {
                return new LiteralValueEstimate(Value);
            }

            public void VisitUsedVariables(Action<string> variableVisitor) {
            }

            public static readonly LiteralReducedInstruction Zero = new LiteralReducedInstruction(0);
            public static readonly LiteralReducedInstruction One = new LiteralReducedInstruction(1);
        }

        public class InputReducedInstruction : ReducedInstruction {
            public readonly int Index;

            public InputReducedInstruction(int index) {
                Index = index;
            }

            public override string ToString() {
                return $"Input({Index})";
            }

            public ValueEstimate EstimateValue() {
                return new RangeEstimate(1, 9);
            }

            public void VisitUsedVariables(Action<string> variableVisitor) {
            }
        }

        public class ReducedAddInstruction : ReducedInstruction {
            public readonly ReducedInstruction Left;
            public readonly ReducedInstruction Right;

            public ReducedAddInstruction(ReducedInstruction left, ReducedInstruction right) {
                Left = left;
                Right = right;
            }

            public override string ToString() {
                return $"({Left} + {Right})";
            }

            public ValueEstimate EstimateValue() {
                return Left.EstimateValue().Add(Right.EstimateValue());
            }

            public void VisitUsedVariables(Action<string> variableVisitor) {
                Left.VisitUsedVariables(variableVisitor);
                Right.VisitUsedVariables(variableVisitor);
            }
        }

        public class ReducedMultiplyInstruction : ReducedInstruction {
            public readonly ReducedInstruction Left;
            public readonly ReducedInstruction Right;

            public ReducedMultiplyInstruction(ReducedInstruction left, ReducedInstruction right) {
                Left = left;
                Right = right;
            }

            public override string ToString() {
                return $"({Left} * {Right})";
            }

            public ValueEstimate EstimateValue() {
                return Left.EstimateValue().Multiply(Right.EstimateValue());
            }

            public void VisitUsedVariables(Action<string> variableVisitor) {
                Left.VisitUsedVariables(variableVisitor);
                Right.VisitUsedVariables(variableVisitor);
            }
        }

        public class ReducedDivideInstruction : ReducedInstruction {
            public readonly ReducedInstruction Left;
            public readonly ReducedInstruction Right;

            public ReducedDivideInstruction(ReducedInstruction left, ReducedInstruction right) {
                Left = left;
                Right = right;
            }

            public override string ToString() {
                return $"({Left} / {Right})";
            }

            public ValueEstimate EstimateValue() {
                return Left.EstimateValue().Divide(Right.EstimateValue());
            }

            public void VisitUsedVariables(Action<string> variableVisitor) {
                Left.VisitUsedVariables(variableVisitor);
                Right.VisitUsedVariables(variableVisitor);
            }
        }

        public class ReducedModuloInstruction : ReducedInstruction {
            public readonly ReducedInstruction Left;
            public readonly ReducedInstruction Right;

            public ReducedModuloInstruction(ReducedInstruction left, ReducedInstruction right) {
                Left = left;
                Right = right;
            }

            public override string ToString() {
                return $"({Left} % {Right})";
            }

            public ValueEstimate EstimateValue() {
                return Left.EstimateValue().Modulo(Right.EstimateValue());
            }

            public void VisitUsedVariables(Action<string> variableVisitor) {
                Left.VisitUsedVariables(variableVisitor);
                Right.VisitUsedVariables(variableVisitor);
            }
        }

        public class ReducedEqualInstruction : ReducedInstruction {
            public readonly ReducedInstruction Left;
            public readonly ReducedInstruction Right;

            public ReducedEqualInstruction(ReducedInstruction left, ReducedInstruction right) {
                Left = left;
                Right = right;
            }

            public override string ToString() {
                return $"({Left} == {Right} ? 0 : 1)";
            }

            public ValueEstimate EstimateValue() {
                return Left.EstimateValue().IsEqual(Right.EstimateValue());
            }

            public void VisitUsedVariables(Action<string> variableVisitor) {
                Left.VisitUsedVariables(variableVisitor);
                Right.VisitUsedVariables(variableVisitor);
            }
        }

        public class VariableReference : ReducedInstruction {
            private string _variableName;
            private ValueEstimate _valueEstimate;

            public VariableReference(string variableName, ValueEstimate valueEstimate) {
                _variableName = variableName;
                _valueEstimate = valueEstimate;
            }

            public ValueEstimate EstimateValue() {
                return _valueEstimate;
            }

            public void VisitUsedVariables(Action<string> variableVisitor) {
                variableVisitor(_variableName);
            }

            public override string ToString() {
                return _variableName;
            }
        }
    }
}
