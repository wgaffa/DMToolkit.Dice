using System;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Extensions
{
    public static class MaybeExtension
    {
        public static Maybe<T> Nothing<T>(this Maybe<T> source, Action ifNone)
        {
            if (source is None<T>)
                ifNone();

            return source;
        }

        public static Maybe<T> NoneIfNull<T>(this Maybe<T> source)
            => source ?? None.Value;

        public static Maybe<T> Do<T>(this Maybe<T> source, Action<T> action)
        {
            if (source is Some<T> some)
                action(some.Reduce(default(T)));

            return source;
        }
    }
}
