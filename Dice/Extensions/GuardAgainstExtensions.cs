using Ardalis.GuardClauses;
using System;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Extensions
{
    public static class GuardAgainstExtensions
    {
        public static void NullOrNone<T>(this IGuardClause source, Maybe<T> maybe, string parameterName)
        {
            source.Null(maybe, parameterName);

            if (maybe is None<T>)
                throw new ArgumentException("Maybe cannot be None", parameterName);
        }
    }
}
