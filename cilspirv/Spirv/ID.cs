using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace cilspirv.Spirv
{
    /// <summary>
    /// A SSA-style ID
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct ID : IEquatable<ID>, IComparable<ID>
    {
        /// <summary>
        /// Return the invalid ID zero
        /// </summary>
        public static ID Invalid => new ID(0);

        /// <summary>
        /// Numerical value
        /// </summary>
        public readonly uint Value;

        public ID(uint id) => Value = id;

        public override string ToString() => "#" + Value;

        public override int GetHashCode() => (int)Value;
        public bool Equals(ID other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is ID id && Value == id.Value;
        public int CompareTo(ID other) => Math.Sign(Value - other.Value);
        public static bool operator ==(ID left, ID right) => left.Value == right.Value;
        public static bool operator !=(ID left, ID right) => left.Value != right.Value;
        public static bool operator <(ID left, ID right) => left.Value < right.Value;
        public static bool operator <=(ID left, ID right) => left.Value <= right.Value;
        public static bool operator >(ID left, ID right) => left.Value > right.Value;
        public static bool operator >=(ID left, ID right) => left.Value >= right.Value;
    }
}
