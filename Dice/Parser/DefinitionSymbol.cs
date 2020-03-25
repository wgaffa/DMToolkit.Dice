using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Parser
{
    public class DefinitionSymbol : Symbol, IEquatable<DefinitionSymbol>
    {
        public IExpression Expression { get; }

        public DefinitionSymbol(string name, IExpression expression)
            : base(name, None.Value)
        {
            Guard.Against.Null(expression, nameof(expression));

            Expression = expression;
        }

        #region Equality
        public bool Equals(DefinitionSymbol other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            return Equals(obj as DefinitionSymbol);
        }

        public static bool operator ==(DefinitionSymbol left, DefinitionSymbol right) =>
            EqualityComparer<DefinitionSymbol>.Default.Equals(left, right);

        public static bool operator !=(DefinitionSymbol left, DefinitionSymbol right) =>
            !(left == right);
        #endregion

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 213 + Name.GetHashCode();
                hash = hash * 213 + Type.GetHashCode();

                return hash;
            }
        }
    }
}