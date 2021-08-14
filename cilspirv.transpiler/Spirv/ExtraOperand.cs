﻿using System;
using System.Collections.Immutable;
using System.Linq;
using static cilspirv.Spirv.DebugUtils;

namespace cilspirv.Spirv
{
    public enum ExtraOperandKind
    {
        Numeric,
        String,
        ID,
        Enum,
        Unknown
    }

    public readonly struct ExtraOperand : IEquatable<ExtraOperand>
    {
        private readonly ExtraOperandKind kind;
        private readonly ImmutableArray<LiteralNumber> numeric;
        private readonly LiteralString textual;
        private readonly Type? enumType;
        private readonly uint rawValue;

        public int WordCount => kind switch
        {
            ExtraOperandKind.Numeric => numeric.Length,
            ExtraOperandKind.String => textual.WordCount,
            _ => 1
        };

        public ExtraOperand(uint rawValue)
        {
            kind = ExtraOperandKind.Unknown;
            numeric = default;
            textual = default;
            enumType = default;
            this.rawValue = rawValue;
        }

        public ExtraOperand(LiteralNumber number)
        {
            kind = ExtraOperandKind.Numeric;
            numeric = ImmutableArray.Create(number);
            textual = default;
            enumType = default;
            rawValue = default;
        }

        public ExtraOperand(ImmutableArray<LiteralNumber> number)
        {
            kind = ExtraOperandKind.Numeric;
            numeric = number;
            textual = default;
            enumType = default;
            rawValue = default;
        }

        public ExtraOperand(ID id)
        {
            kind = ExtraOperandKind.ID;
            numeric = default;
            textual = default;
            enumType = default;
            rawValue = id.Value;
        }

        public ExtraOperand(Enum @enum)
        {
            kind = ExtraOperandKind.Enum;
            numeric = default;
            textual = default;
            enumType = @enum.GetType();
            rawValue = unchecked((uint)@enum.GetHashCode());
        }

        public ExtraOperand(LiteralString text)
        {
            kind = ExtraOperandKind.String;
            numeric = default;
            textual = text;
            enumType = default;
            rawValue = default;
        }

        public static implicit operator ExtraOperand(int val) => new ExtraOperand((LiteralNumber)val);
        public static implicit operator ExtraOperand(uint val) => new ExtraOperand((LiteralNumber)val);
        public static implicit operator ExtraOperand(float val) => new ExtraOperand((LiteralNumber)val);
        public static implicit operator ExtraOperand(double val) => new ExtraOperand(LiteralNumber.ArrayFor(val));
        public static implicit operator ExtraOperand(string val) => new ExtraOperand(val);
        public static implicit operator ExtraOperand(Enum val) => new ExtraOperand(val);

        public void Write(Span<uint> code, Func<ID, uint> mapID)
        {
            int i = 0;
            Write(code, ref i, mapID);
        }

        public void Write(Span<uint> code, ref int i, Func<ID, uint> mapID)
        {
            switch (kind)
            {
                case ExtraOperandKind.Numeric:
                    foreach (var number in numeric)
                        code[i++] = number.Value;
                    break;

                case ExtraOperandKind.String:
                    textual.Write(code, ref i);
                    break;

                case ExtraOperandKind.ID:
                    code[i++] = mapID(new ID(rawValue));
                    break;

                case ExtraOperandKind.Enum:
                case ExtraOperandKind.Unknown:
                    code[i++] = rawValue;
                    break;

                default: throw new NotSupportedException($"Unsupported extra operand type {kind}");
            }
        }

        public override string ToString() => kind switch
        {
            ExtraOperandKind.Numeric => StrOf(numeric),
            ExtraOperandKind.ID => new ID(rawValue).ToString(),
            ExtraOperandKind.String => textual.ToString(),
            ExtraOperandKind.Unknown => rawValue.ToString("X8"),
            ExtraOperandKind.Enum => Enum.GetName(enumType!, rawValue) ?? $"<unknown {enumType}>",
            _ => throw new NotSupportedException($"Unsupported extra operand type {kind}")
        };

        public bool Equals(ExtraOperand other) =>
            kind == other.kind &&
            numeric.SequenceEqual(other.numeric) &&
            textual == other.textual &&
            enumType == other.enumType &&
            rawValue == other.rawValue;

        public override int GetHashCode() =>
            HashCode.Combine(kind, numeric, textual, enumType, rawValue);

        public override bool Equals(object? obj) => obj is ExtraOperand operand && Equals(operand);
        public static bool operator ==(ExtraOperand left, ExtraOperand right) => left.Equals(right);
        public static bool operator !=(ExtraOperand left, ExtraOperand right) => !(left == right);
    }
}