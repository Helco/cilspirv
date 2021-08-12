using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace cilspirv.SourceGen
{
    partial class SourceGen
    {
        private static void GenerateInstructions(CoreGrammar.Rootobject coreGrammar, string basePath)
        {
            var instructions = coreGrammar.instructions.Where(i => i.@class != "@exclude" && i.@class != "Reserved");
            foreach (var instruction in instructions)
                GenerateInstruction(instruction, basePath);
        }

        private static void GenerateInstruction(CoreGrammar.Instruction instruction, string basePath)
        {
            instruction.operands ??= Array.Empty<CoreGrammar.Operand>();
            var duplicateGroups = instruction.operands
                .GroupBy(op => op.name)
                .Where(g => g.Count() > 1);
            foreach (var duplicateGroup in duplicateGroups)
            {
                int i = 1;
                foreach (var operand in duplicateGroup)
                    operand.name = MapOperandName(operand) + i++;
            }

            var dirPath = Path.Combine(basePath, instruction.@class);
            Directory.CreateDirectory(dirPath);
            using var output = new StreamWriter(Path.Combine(dirPath, instruction.opname + ".cs"));
            output.WriteLines(Header());
            foreach (var operand in instruction.operands)
                output.WriteLines(Operand(operand));
            output.WriteLine();
            output.WriteLines(MetaProperties());
            output.WriteLine();
            output.WriteLines(AllIDsProperty());
            output.WriteLine();
            output.WriteLines(DefaultCtor());
            output.WriteLine();
            output.WriteLines(CodeCtor());
            output.WriteLine();
            output.WriteLines(Write());
            output.WriteLines(Footer());

            IEnumerable<string> Header()
            {
                yield return "// This file was generated. Do not modify.";
                yield return "using System;";
                yield return "using System.Linq;";
                yield return "using System.Collections.Generic;";
                yield return "using System.Collections.Immutable;";

                yield return "";
                yield return "namespace cilspirv.Spirv.Ops";
                yield return "{";

                var obsolete = Obsolete(instruction.lastVersion);
                if (obsolete != null)
                    yield return "    " + obsolete;
                var dependsOn = InstructionDependsOn(instruction);
                if (dependsOn != null)
                    yield return "    " + dependsOn;

                yield return $"    public sealed record {instruction.opname} : {MapInstructionClassName(instruction.@class)}";
                yield return  "    {";
            }

            static IEnumerable<string> Operand(CoreGrammar.Operand operand)
            {
                yield return $"        public {MapOperandType(operand)} {MapOperandName(operand)} {{ get; init; }}";
            }

            IEnumerable<string> MetaProperties()
            {
                yield return $"        public override OpCode OpCode => OpCode.{instruction.opname};";

                var wordCount = string.Join(" + ", instruction.operands
                    .Select(MapOperandWordCount)
                    .Prepend("1"));
                yield return $"        public override int WordCount => {wordCount};";

                var resultOperand = instruction.operands.FirstOrDefault(o => o.kind == "IdResult");
                if (resultOperand != null)
                    yield return $"        public override ID? ResultID => {MapOperandName(resultOperand)};";

                var resultTypeOperand = instruction.operands.FirstOrDefault(o => o.kind == "IdResultType");
                if (resultTypeOperand != null)
                    yield return $"        public override ID? ResultTypeID => {MapOperandName(resultTypeOperand)};";
            }

            IEnumerable<string> AllIDsProperty()
            {
                var idOps = instruction.operands.Where(o => o.kind.StartsWith("Id"));
                var pairOps = instruction.operands.Where(o => o.kind.StartsWith("Pair")).ToArray();
                var fixedOps = idOps.Where(o => o.quantifier == null).ToArray();
                var listOps = idOps.Where(o => o.quantifier == "*").ToArray();
                var pairListOps = pairOps.Where(o => o.quantifier == "*").ToArray();
                var optOps = idOps.Where(o => o.quantifier == "?").ToArray();
                var optPairOps = pairOps.Where(o => o.quantifier == "?").ToArray();
                var hasFixed = pairOps.Any() || fixedOps.Any();
                var hasList = listOps.Any() || pairListOps.Any();
                var hasOpt = optOps.Any() || optPairOps.Any();

                if (!hasFixed && !hasList && !hasOpt)
                    yield break;
                var fixedIds = fixedOps
                    .Select(MapOperandName)
                    .Concat(pairOps.SelectMany(GetPairIDs));
                var fixedIdArray = $"new[] {{ {string.Join(", ", fixedIds)} }}";
                if (!hasList && !hasOpt)
                {
                    yield return $"        public override IEnumerable<ID> AllIDs => {fixedIdArray};";
                    yield break;
                }

                yield return $"        public override IEnumerable<ID> AllIDs";
                yield return "        {";
                yield return "            get";
                yield return "            {";
                yield return "                var result = Enumerable.Empty<ID>();";

                foreach (var listOp in listOps)
                    yield return $"                result = result.Concat({MapOperandName(listOp)});";
                foreach (var pairListOp in pairListOps)
                {
                    var name = MapOperandName(pairListOp);
                    foreach (var pairId in GetPairIDs(pairListOp))
                        yield return $"                result = result.Concat({name}.Select(x => {pairId.Replace(name, "x")}));";
                }

                if (hasOpt)
                {
                    var optIds = optOps
                        .Select(MapOperandName)
                        .Concat(optPairOps.SelectMany(GetOptPairIDs));
                    var optIdArray = $"new[] {{ {string.Join(", ", optIds)} }}";
                    yield return $"                result = result.Concat({optIdArray}";
                    yield return $"                    .Where(o => o.HasValue)";
                    yield return $"                    .Select(o => o!.Value));";
                }

                yield return $"                return result;";
                yield return "            }";
                yield return "        }";
            }

            IEnumerable<string> DefaultCtor()
            {
                yield return $"        public {instruction.opname}() {{}}";
            }

            IEnumerable<string> CodeCtor()
            {
                yield return $"        private {instruction.opname}(IReadOnlyList<uint> codes, Range range)";
                yield return "        {";
                yield return "            var (start, end) = range.GetOffsetAndLength(codes.Count);";
                yield return "            end += start;";
                yield return "            var i = start;";

                foreach (var operand in instruction.operands)
                {
                    var name = MapOperandName(operand);
                    var space = "            ";
                    if (operand.quantifier == "?")
                    {
                        yield return $"{space}if (i < end)";
                        space += "    ";
                    }
                    if (operand.quantifier != "*")
                        yield return $"{space}{name} = {ConvertFromRaw(operand, "codes[i++]")};";
                    else if (operand.kind.StartsWith("Pair"))
                    {
                        yield return $"{space}{name} = Enumerable.Repeat(0, (end - i) / 2)";
                        yield return $"{space}    .Select(_ => {ConvertFromRaw(operand, "codes[i++]")})";
                        yield return $"{space}    .ToImmutableArray();";
                    }
                    else
                    {
                        yield return $"{space}{name} = codes.Skip(i).Take(end - i)";
                        yield return $"{space}    .Select(x => {ConvertFromRaw(operand, "x")})";
                        yield return $"{space}    .ToImmutableArray();";
                    }
                }

                yield return "        }";
            }

            IEnumerable<string> Write()
            {
                yield return $"        public override void Write(Span<uint> codes)";
                yield return "        {";
                yield return "            if (codes.Length < WordCount)";
                yield return "                throw new ArgumentException(\"Output span too small\", nameof(codes));";
                yield return "            var i = 0;";
                yield return "            codes[i++] = InstructionCode;";

                foreach (var operand in instruction.operands)
                {
                    var name = MapOperandName(operand);
                    var space = "            ";
                    if (operand.quantifier == "?")
                    {
                        yield return $"{space}if ({name}.HasValue)";
                        yield return $"{space}{{";
                        name += ".Value";
                        space += "    ";
                    }
                    if (operand.quantifier != "*")
                        yield return $"{space}{ConvertToRaw(operand, name)}";
                    else
                    {
                        yield return $"{space}foreach (var x in {name})";
                        yield return $"{space}{{";
                        yield return $"{space}    {ConvertToRaw(operand, "x")}";
                    }
                    if (operand.quantifier != null)
                        yield return "            }";
                }

                yield return "        }";
            }

            static IEnumerable<string> Footer()
            {
                yield return "    }";
                yield return "}";
                yield return "";
            }

            static string MapOperandType(CoreGrammar.Operand operand)
            {
                var baseType = operand.kind switch
                {
                    "LiteralInteger" => "LiteralNumber",
                    "LiteralString" => "LiteralString",
                    "LiteralContextDependentNumber" => "ImmutableArray<LiteralNumber>",
                    "LiteralExtInstInteger" => "uint",
                    "LiteralSpecConstantOpInteger" => "OpCode",
                    "PairLiteralIntegerIdRef" => "(LiteralNumber, ID)",
                    "PairIdRefLiteralInteger" => "(ID, LiteralNumber)",
                    "PairIdRefIdRef" => "(ID, ID)",
                    _ when operand.kind.StartsWith("Id") => "ID",
                    _ => operand.kind
                };

                return (operand.quantifier ?? "") switch
                {
                    "" => baseType,
                    "?" => baseType + "?",
                    "*" => $"ImmutableArray<{baseType}>",
                    _ => throw new NotSupportedException($"Unsupported quantifier \"{operand.quantifier}\"")
                };
            }

            static string MapOperandName(CoreGrammar.Operand operand)
            {
                if (operand.name == null)
                    return operand.kind.StartsWith("Id")
                        ? operand.kind.Substring(2)
                        : operand.kind;
                else if (operand.name.Contains('\n')) // "'Member 0 type', + \n'member 1 "... to "Members"
                    return operand.name.Split('\'', ' ')[1] + 's';
                else if (operand.name.IndexOfAny(new[] { ',' }) >= 0)
                    return "Operands";
                else if (operand.name == "The name of the opaque type.")
                    return "TypeName";
                else
                    return operand.name.Replace("'", "").Replace(" ", "").Replace("~", "");
            }

            static string MapOperandWordCount(CoreGrammar.Operand operand)
            {
                var name = MapOperandName(operand);
                var baseCount = operand.kind switch
                {
                    "LiteralString" => $"{name}.WordCount",
                    "LiteralContextDependentNumber" => $"{name}.Length",
                    _ when operand.kind.StartsWith("Pair") => "2",
                    _ => "1"
                };

                return (operand.quantifier ?? "") switch
                {
                    "" => baseCount,
                    "?" => $"({name}.HasValue ? {baseCount.Replace(".", ".Value.")} : 0)",
                    "*" => operand.kind switch
                    {
                        "LiteralString" => $"{name}.Sum(x => x.WordCount)",
                        "LiteralContextDependentNumber" => $"{name}.Sum(x => x.Length)",
                        _ when operand.kind.StartsWith("Pair") => $"2 * {name}.Length",
                        _ => $"{name}.Length"
                    },
                    _ => throw new NotSupportedException($"Unsupported quantifier \"{operand.quantifier}\"")
                };
            }

            static IEnumerable<string> GetPairIDs(CoreGrammar.Operand operand) => GetPairIDsEx(operand, ".");
            static IEnumerable<string> GetOptPairIDs(CoreGrammar.Operand operand) => GetPairIDsEx(operand, "?.");

            static IEnumerable<string> GetPairIDsEx(CoreGrammar.Operand operand, string access)
            {
                var name = MapOperandName(operand);
                return operand.kind switch
                {
                    "PairLiteralIntegerIdRef" => new[] { $"{name}{access}Item2" },
                    "PairIdRefLiteralInteger" => new[] { $"{name}{access}Item1" },
                    "PairIdRefIdRef" => new[] { $"{name}{access}Item1", $"{name}{access}Item2" },
                    _ => throw new NotSupportedException($"Unsupported pair operand \"{operand.kind}\"")
                };
            }

            static string ConvertFromRaw(CoreGrammar.Operand operand, string rawValue) => operand.kind switch
            {
                "LiteralInteger" => "(LiteralNumber)" + rawValue,
                "LiteralString" => "new LiteralString(codes, ref i)",
                "LiteralContextDependentNumber" => "codes.Skip(i).Take(end - i).Select(n => (LiteralNumber)n).ToImmutableArray()",
                "LiteralExtInstInteger" => rawValue,
                "LiteralSpecConstantOpInteger" => $"(OpCode){rawValue}",
                "PairLiteralIntegerIdRef" => $"(new LiteralNumber({rawValue}), new ID({rawValue}))",
                "PairIdRefLiteralInteger" => $"(new ID({rawValue}), new LiteralNumber({rawValue}))",
                "PairIdRefIdRef" => $"(new ID({rawValue}), new ID({rawValue}))",
                _ when operand.kind.StartsWith("Id") => $"new ID({rawValue})",
                _ => $"({operand.kind}){rawValue}"
            };

            static string ConvertToRaw(CoreGrammar.Operand operand, string value) => operand.kind switch
            {
                "LiteralInteger" => $"codes[i++] = {value}.Value;",
                "LiteralString" => $"{value}.Write(codes, ref i);",
                "LiteralContextDependentNumber" => $"{value}.Select(v => v.Value).ToArray().CopyTo(codes.Slice(i)); i += {value}.Length;",
                _ when operand.kind.StartsWith("Pair") => $"codes[i++] = {value}.Item1.Value; codes[i++] = {value}.Item2.Value;",
                _ when operand.kind.StartsWith("Id") => $"codes[i++] = {value}.Value;",
                _ => $"codes[i++] = (uint){value};"
            };
        }
    }
}
