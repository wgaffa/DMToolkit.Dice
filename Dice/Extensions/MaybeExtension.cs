using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
