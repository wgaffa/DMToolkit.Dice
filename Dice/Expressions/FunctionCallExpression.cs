﻿using Ardalis.GuardClauses;
using System.Collections.Generic;
using System.Linq;
using Wgaffa.DMToolkit.Parser;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Expressions
{
    public class FunctionCallExpression : IExpression
    {
        private readonly List<IExpression> _arguments = new List<IExpression>();

        public string Name { get; }
        internal Maybe<ISymbol> Symbol { get; set; } = None.Value;

        public IReadOnlyList<IExpression> Arguments => _arguments.AsReadOnly();

        public FunctionCallExpression(string name, IEnumerable<IExpression> arguments)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Guard.Against.Null(arguments, nameof(arguments));

            _arguments = arguments.ToList();
            Name = name;
        }

        public override string ToString()
        {
            return $"<call: {Name} args={string.Join(' ', Arguments)}>";
        }
    }
}
