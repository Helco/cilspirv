using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cilspirv.Spirv
{
    internal static class DebugUtils
    {
        public static string StrOf(ID id) => id.ToString();

        public static string StrOf(LiteralNumber nr) => nr.ToString();

        public static string StrOf(LiteralString str) => str.ToString();

        public static string StrOf(ID? id) =>
            string.Format("<{0}>", id?.ToString() ?? "null");

        public static string StrOf(ID[] ids) =>
            string.Format("[{0}]", ids == null ? "null"
            : ids.Length == 0 ? " "
            : ids.Select(i => i.ToString()).Aggregate((s1, s2) => s1 + ", " + s2));

        public static string StrOf(LiteralNumber[] nrs) =>
            string.Format("[{0}]", nrs == null ? "null"
            : nrs.Length == 0 ? " "
            : nrs.Select(i => i.ToString()).Aggregate((s1, s2) => s1 + ", " + s2));

        public static string StrOf<T>(T e) where T : struct => e.ToString() ?? "null";

        public static string StrOf<T>(T[] es) =>
            string.Format("[{0}]", es == null ? "null"
            : es.Length == 0 ? " "
            : es.Select(i => i?.ToString() ?? "null").Aggregate((s1, s2) => s1 + ", " + s2));

        public static string StrOf<U, V>((U, V)[] p) =>
            string.Format("[{0}]", p == null ? "null"
            : p.Length == 0 ? " "
            : p.Select(i => i.ToString()).Aggregate((s1, s2) => s1 + ", " + s2));
    }
}
