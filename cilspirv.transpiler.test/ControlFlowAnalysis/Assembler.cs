using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using NUnit.Framework;

namespace cilspirv.transpiler.test
{
    public static class Assembler
    {
        private static readonly RegexOptions Options = RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled;
        private static readonly Regex TypePragmaRegex = new Regex(@"^\s*\.(param|return|local)\s+([\w\.\+]+)(\[\])?(&)?\s*$", Options);
        private static readonly Regex InstrPragmaRegex = new Regex(@"^\s*(\w+:)?\s*(\w[\w\.]+)(\s+.+)?\s*$", Options);
        private static readonly Regex TokenRegex = new Regex(@"^([\w\.\+]+)(::\w+)?$", Options);

        private static readonly object myAssemblyLock = new object();
        private static AssemblyDefinition? myAssembly;
        private static AssemblyDefinition MyAssembly
        {
            get
            {
                if (myAssembly != null)
                    return myAssembly;
                lock(myAssemblyLock)
                {
                    return myAssembly ??= AssemblyDefinition.ReadAssembly(
                        typeof(Assembler).Assembly.Location);
                }
            }
        }

        private enum PragmaKind
        {
            Parameter,
            Return,
            Local
        }

        private static IReadOnlyDictionary<string, OpCode> opCodeByName = typeof(OpCodes)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Select(f => (OpCode)f.GetValue(null)!)
            .ToDictionary(o => o.Name, o => o);

        public static MethodDefinition Parse(string input, string funcName = "Func")
        {
            var lines = input
                .Split("\n")
                .Select(l => l.Split("//")[0].Trim())
                .Where(l => l.Length > 0);

            var methodDef = new MethodDefinition(funcName, MethodAttributes.Static, MyAssembly.MainModule.ImportReference(typeof(void)));
            methodDef.Body = new MethodBody(methodDef);

            var pragmas = lines
                .Select(l => TypePragmaRegex.Match(l))
                .Where(m => m.Success)
                .Select(ParsePragma);
            foreach (var (pragmaKind, pragmaType) in pragmas)
            {
                switch (pragmaKind)
                {
                    case PragmaKind.Parameter: methodDef.Parameters.Add(new ParameterDefinition(pragmaType)); break;
                    case PragmaKind.Return: methodDef.ReturnType = pragmaType; break;
                    case PragmaKind.Local: methodDef.Body.Variables.Add(new VariableDefinition(pragmaType)); break;
                    default: throw new NotSupportedException("Unsupported pragma kind: " + pragmaKind);
                }
            }

            var instrTuples = lines
                .Select(l => InstrPragmaRegex.Match(l))
                .Where(m => m.Success)
                .Select(PreParseInstruction)
                .ToArray();
            var instrs = instrTuples
                .Select(t => t.instr)
                .ToArray();
            var instrByLabel = instrTuples
                .Where(i => i.label != null)
                .ToDictionary(t => t.label![0..^1], t => t.instr);
            var offset = 0;
            for (int i = 0; i < instrs.Length; i++)
            {
                if (instrTuples[i].operand != null)
                    instrs[i].Operand = ParseOperand(instrs[i], instrTuples[i].operand!.Trim());
                instrs[i].Offset = offset;
                offset += instrs[i].GetSize();
                methodDef.Body.Instructions.Add(instrs[i]);
            }

            var invalidLine = lines
                .Where(l => !TypePragmaRegex.IsMatch(l) && !InstrPragmaRegex.IsMatch(l))
                .FirstOrDefault();
            if (invalidLine != null)
                throw new InvalidOperationException("Invalid line: " + invalidLine);

            return methodDef;

            object ParseOperand(Instruction instr, string operand) => instr.OpCode.OperandType switch
            {
                OperandType.ShortInlineI => sbyte.Parse(operand),
                OperandType.InlineI => int.Parse(operand),
                OperandType.InlineI8 => long.Parse(operand),
                OperandType.ShortInlineR => float.Parse(operand, CultureInfo.InvariantCulture),
                OperandType.InlineR => double.Parse(operand, CultureInfo.InvariantCulture),
                OperandType.InlineArg => methodDef.Parameters[short.Parse(operand)],
                OperandType.ShortInlineArg => methodDef.Parameters[byte.Parse(operand)],
                OperandType.InlineVar => methodDef.Body.Variables[short.Parse(operand)],
                OperandType.ShortInlineVar=> methodDef.Body.Variables[byte.Parse(operand)],

                OperandType.InlineBrTarget => instrByLabel[operand],
                OperandType.ShortInlineBrTarget => instrByLabel[operand],
                OperandType.InlineSwitch => operand
                    .Split(",")
                    .Select(t => instrByLabel[t.Trim()])
                    .ToArray(),

                OperandType.InlineMethod => (MethodReference)ParseToken(operand),
                OperandType.InlineField => (FieldReference)ParseToken(operand),
                OperandType.InlineType => (TypeReference)ParseToken(operand),
                OperandType.InlineTok => ParseToken(operand),

                OperandType.InlineString => operand.StartsWith('\"') && operand.EndsWith('\"')
                    ? operand.Substring(1, operand.Length - 2)
                    : operand,

                _ => throw new NotSupportedException("Unsupported operand type: " + instr.OpCode.OperandType)
            };
        }

        static (PragmaKind, TypeReference) ParsePragma(Match match)
        {
            var isParam = match.Groups[1].Value.ToLowerInvariant() switch
            {
                "param" => PragmaKind.Parameter,
                "return" => PragmaKind.Return,
                "local" => PragmaKind.Local,
                _ => throw new InvalidOperationException("Invalid pragma kind: " + match.Groups[1].Value)
            };
            var baseType = Type.GetType(match.Groups[2].Value);
            if (baseType == null)
                throw new InvalidOperationException("Invalid type name: " + match.Groups[2].Value);
            var typeRef = MyAssembly.MainModule.ImportReference(baseType);
            if (match.Groups[3].Success)
                typeRef = new ArrayType(typeRef);
            if (match.Groups[4].Success)
                typeRef = new ByReferenceType(typeRef);
            return (isParam, typeRef);
        }

        static (string? label, Instruction instr, string? operand) PreParseInstruction(Match match)
        {
            var label = match.Groups[1].Success ? match.Groups[1].Value : null;
            var operand = match.Groups[3].Success ? match.Groups[3].Value : null;

            var opCodeName = match.Groups[2].Value;
            if (!opCodeByName.TryGetValue(opCodeName, out var opCode))
                throw new InvalidOperationException("Invalid op: " + opCodeName);
            var instr = Instruction.Create(OpCodes.Break);
            instr.OpCode = opCode;
            return (label, instr, operand);
        }

        static IMetadataTokenProvider ParseToken(string operand)
        {
            var match = TokenRegex.Match(operand);
            if (!match.Success)
                throw new InvalidOperationException("Invalid token format: " + operand);

            var type = Type.GetType(match.Groups[1].Value);
            if (type == null)
                throw new InvalidOperationException("Invalid type name: " + match.Groups[1].Value);
            var typeRef = MyAssembly.MainModule.ImportReference(type);
            if (!match.Groups[2].Success)
                return typeRef;

            var member = match.Groups[2].Value.Substring(2); // skip the ::
            return
                typeRef.Resolve().Methods.FirstOrDefault(m => m.Name == member) as IMetadataTokenProvider ??
                typeRef.Resolve().Fields.FirstOrDefault(f => f.Name == member) ??
                throw new InvalidOperationException("Invalid member name: " + member);
        }
    }
}
