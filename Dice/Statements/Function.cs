﻿using Ardalis.GuardClauses;
using System.Collections.Generic;
using System.Linq;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.DMToolkit.Parser;

namespace Wgaffa.DMToolkit.Statements
{
    public class Function : IStatement
    {
        private readonly List<KeyValuePair<string, string>> _parameters =
            new List<KeyValuePair<string, string>>();
        internal FunctionSymbol Symbol;

        public string Identifier { get; }
        public IStatement Body { get; }
        public string ReturnType { get; }
        public IReadOnlyList<KeyValuePair<string, string>> Parameters =>
            _parameters.AsReadOnly();

        public Function(string identifier, IStatement body, string returnType)
            : this(identifier, body, returnType, new List<KeyValuePair<string, string>>())
        {
        }

        public Function(string identifier, IStatement body, string returnType, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            Guard.Against.NullOrWhiteSpace(identifier, nameof(identifier));
            Guard.Against.NullOrWhiteSpace(returnType, nameof(returnType));
            Guard.Against.Null(parameters, nameof(parameters));

            _parameters = parameters.ToList();
            Identifier = identifier;
            Body = body;
            ReturnType = returnType;
        }

        public override string ToString()
        {
            var parameters = _parameters.Select(kv => $"{kv.Key}:{kv.Value}").ToList();
            var paramString = parameters.Count == 0 ? string.Empty : $" params={string.Join(' ', parameters)}";
            return $"<func: var={Identifier} return={ReturnType} body={Body}{paramString}>";
        }
    }
}