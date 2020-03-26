using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Extensions
{
    public static class ObjectExtensions
    {
        public static Maybe<T> NoneIfNull<T>(this T source) where T : class
            => source is null
            ? (Maybe<T>)None.Value
            : Maybe<T>.Some(source);
    }
}