using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wgaffa.DMToolkit.Interpreters.Errors
{
    public class SemanticError
    {
        public string Category { get; }
        public int Code { get; }
        public string Message { get; }

        public SemanticError(string category, int code, string message)
        {
            Guard.Against.NullOrWhiteSpace(category, nameof(category));
            Guard.Against.NegativeOrZero(code, nameof(code));
            Guard.Against.NullOrWhiteSpace(message, nameof(message));

            Category = category;
            Code = code;
            Message = message;
        }

        public override string ToString()
        {
            return $"{Category}{Code}: {Message}";
        }
    }
}
