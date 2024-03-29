﻿using System;
using System.Collections.Immutable;
using System.Linq;

namespace cilspirv.Spirv
{
    /// <summary>
    /// A numeric value consuming one or more words. 
    /// When a numeric value is larger than one word, low-order words appear first.
    /// </summary>
    public readonly struct LiteralNumber
    {
        public readonly uint Value;

        public LiteralNumber(uint val)
        {
            Value = val;
        }

        public LiteralNumber(byte[] val, int start)
        {
            Value = BitConverter.ToUInt32(val, start);
        }

        public static ImmutableArray<LiteralNumber> ArrayFor(byte[] vals)
        {
            var nrs = new LiteralNumber[vals.Length / 4];
            for (var i = 0; i < vals.Length; i += 4)
                nrs[i / 4] = new LiteralNumber(vals, i);
            return nrs.ToImmutableArray();
        }

        public static implicit operator LiteralNumber(float val) => new LiteralNumber(BitConverter.GetBytes(val), 0);
        public static implicit operator LiteralNumber(int val) => new LiteralNumber(BitConverter.GetBytes(val), 0);
        public static implicit operator LiteralNumber(bool val) => new LiteralNumber(val ? 1u : 0u);
        public static implicit operator LiteralNumber(uint val) => new LiteralNumber(val);

        public static ImmutableArray<LiteralNumber> ArrayFor(int val) => ArrayFor(BitConverter.GetBytes(val));
        public static ImmutableArray<LiteralNumber> ArrayFor(uint val) => ArrayFor(BitConverter.GetBytes(val));
        public static ImmutableArray<LiteralNumber> ArrayFor(float val) => ArrayFor(BitConverter.GetBytes(val));
        public static ImmutableArray<LiteralNumber> ArrayFor(long val) => ArrayFor(BitConverter.GetBytes(val));
        public static ImmutableArray<LiteralNumber> ArrayFor(ulong val) => ArrayFor(BitConverter.GetBytes(val));
        public static ImmutableArray<LiteralNumber> ArrayFor(double val) => ArrayFor(BitConverter.GetBytes(val));

        public static ImmutableArray<LiteralNumber> Array(params uint[] vals) => vals.Select(v => new LiteralNumber(v)).ToImmutableArray();

        public override string ToString() => Value.ToString();
    }

    public static class LiteralNumberExtensions
    {
        public static byte[] ToByteArray(this LiteralNumber[] nrs) => nrs.SelectMany(nr => BitConverter.GetBytes(nr.Value)).ToArray();
        public static int ToInt32(this LiteralNumber[] nrs) => BitConverter.ToInt32(nrs.ToByteArray(), 0);
        public static uint ToUInt32(this LiteralNumber[] nrs) => BitConverter.ToUInt32(nrs.ToByteArray(), 0);
        public static long ToInt64(this LiteralNumber[] nrs) => BitConverter.ToInt64(nrs.ToByteArray(), 0);
        public static ulong ToUInt64(this LiteralNumber[] nrs) => BitConverter.ToUInt64(nrs.ToByteArray(), 0);
        public static float ToFloat32(this LiteralNumber[] nrs) => BitConverter.ToSingle(nrs.ToByteArray(), 0);
        public static double ToFloat64(this LiteralNumber[] nrs) => BitConverter.ToDouble(nrs.ToByteArray(), 0);
    }
}
