using System;
using System.Collections.Generic;
using System.Linq;

namespace Zenviro.Bushido
{
    public static class Extensions
    {
        public static bool Contains(this string source, string value, StringComparison comparisonType)
        {
            return source.IndexOf(value, comparisonType) >= 0;
        }

        public static bool ListEquals<T>(IList<T> a, IList<T> b)
        {
            return (a != null || b == null) && (b != null || a == null)
                && ((a == null) || (a.Count == b.Count && !a.Where((t, i) => !t.Equals(b[i])).Any()));
        }
    }
}
