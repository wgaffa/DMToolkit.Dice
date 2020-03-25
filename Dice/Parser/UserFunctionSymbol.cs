using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Parser
{
    public class UserFunctionSymbol : FunctionSymbol
    {
        public IExpression Body { get; internal set; }

        public UserFunctionSymbol(
            string name,
            Maybe<Symbol> type,
            IExpression body)
            : this(name, type, body, Array.Empty<Symbol>())
        {
        }

        public UserFunctionSymbol(
            string name,
            Maybe<Symbol> type,
            IExpression body,
            IEnumerable<Symbol> parameters)
            : base(name, type, parameters)
        {
            Guard.Against.Null(body, nameof(body));

            Body = body;
        }
    }
}