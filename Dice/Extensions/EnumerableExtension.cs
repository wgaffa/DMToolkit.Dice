using System.Collections.Generic;

namespace Wgaffa.DMToolkit.Extensions
{
    public static class EnumerableExtension
    {
        public static string StringJoin(this IEnumerable<string> source, string separator)
            => string.Join(separator, source);
    }
}
