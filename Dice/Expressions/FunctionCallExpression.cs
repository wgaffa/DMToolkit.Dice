﻿using Ardalis.GuardClauses;
using System.Collections.Generic;
using System.Linq;

namespace Wgaffa.DMToolkit.Expressions
{
    public class FunctionCallExpression : IExpression
    {
        private readonly List<IExpression> _arguments = new List<IExpression>();

        public string Name { get; }

        public IReadOnlyCollection<IExpression> Arguments => _arguments.AsReadOnly();

        public FunctionCallExpression(string name, IEnumerable<IExpression> arguments)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Guard.Against.Null(arguments, nameof(arguments));

            _arguments = arguments.ToList();
            Name = name;
        }
    }
}
