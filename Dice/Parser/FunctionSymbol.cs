﻿using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Parser
{
    public class FunctionSymbol : ISymbol
    {
        private readonly List<ISymbol> _parameters = new List<ISymbol>();

        public string Name { get; }
        public Maybe<ISymbol> Type { get; }
        public IReadOnlyList<ISymbol> Parameters => _parameters.AsReadOnly();
        public Func<IEnumerable<double>, double> Call { get; }

        public FunctionSymbol(string name, Maybe<ISymbol> type, Func<IEnumerable<double>, double> func)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Guard.Against.Null(type, nameof(type));
            Guard.Against.Null(func, nameof(func));

            Name = name;
            Type = type;
            Call = func;
        }

        public FunctionSymbol(string name, Maybe<ISymbol> type, Func<IEnumerable<double>, double> func, IEnumerable<ISymbol> parameters)
            : this(name, type, func)
        {
            Guard.Against.Null(parameters, nameof(parameters));

            _parameters = parameters.ToList();
        }
    }
}