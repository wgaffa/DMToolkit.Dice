using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using Wgaffa.DMToolkit.Interpreters;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Parser
{
    public class BuiltinFunctionSymbol : FunctionSymbol
    {
        public Func<ActivationRecord, double> Call { get; }

        public BuiltinFunctionSymbol(
            string name,
            Maybe<Symbol> type,
            Func<ActivationRecord, double> func)
            : this(name, type, func, Array.Empty<Symbol>())
        {
        }

        public BuiltinFunctionSymbol(
            string name,
            Maybe<Symbol> type,
            Func<ActivationRecord, double> func,
            IEnumerable<Symbol> parameters)
            : base(name, type, parameters)
        {
            Guard.Against.Null(func, nameof(func));

            Call = func;
        }
    }
}