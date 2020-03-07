using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Wgaffa.DMToolkit.Extensions
{
    public static class EnumerableExtension
    {
        public static string StringJoin(this IEnumerable<string> source, string separator)
            => string.Join(separator, source);

        public static IEnumerable<T> Without<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            Guard.Against.Null(first, nameof(first));
            Guard.Against.Null(second, nameof(second));

            var firstList = first.ToList();
            second.Each(x => firstList.Remove(x));

            return firstList;
        }

        public static IEnumerable<T> Each<T>(this IEnumerable<T> source, Action<T> action)
        {
            Guard.Against.Null(source, nameof(source));
            Guard.Against.Null(action, nameof(action));

            foreach (var item in source)
            {
                action(item);
            }

            return source;
        }

        public static IEnumerable<T> AppendRange<T>(this IEnumerable<T> source, IEnumerable<T> append)
        {
            Guard.Against.Null(source, nameof(source));
            Guard.Against.Null(append, nameof(append));

            foreach (var item in source)
            {
                yield return item;
            }

            foreach (var item in append)
            {
                yield return item;
            }
        }
    }
}
