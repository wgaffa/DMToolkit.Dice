using System;
using System.Diagnostics.CodeAnalysis;

namespace Wgaffa.DMToolkit.Types
{
    public sealed class Unit : IEquatable<Unit>
    {
        public static Unit Value { get; } = new Unit();

        private Unit()
        { }

        public bool Equals([AllowNull] Unit other)
        {
            return !(other is null);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (GetType() != obj.GetType())
                return false;

            return Equals(obj as Unit);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override string ToString()
        {
            return "()";
        }
    }
}