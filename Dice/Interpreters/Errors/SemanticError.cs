using Ardalis.GuardClauses;

namespace Wgaffa.DMToolkit.Interpreters.Errors
{
    public sealed class SemanticError
    {
        public delegate SemanticError CreateError(string additional);

        public static readonly CreateError VariableUndefined = x =>
            new SemanticError(ErrorCategory.Variable, 1, $"undefined variable {x}");
        public static readonly CreateError VariableUnknownType = x =>
            new SemanticError(ErrorCategory.Variable, 2, $"unrecognized type {x}");
        public static readonly CreateError VariableAlreadyDeclared = x =>
            new SemanticError(ErrorCategory.Variable, 3, $"{x} already declared");

        public ErrorCategory Category { get; }
        public int Code { get; }
        public string Message { get; }

        private SemanticError(ErrorCategory category, int code, string message)
        {
            Guard.Against.Null(category, nameof(category));
            Guard.Against.NegativeOrZero(code, nameof(code));
            Guard.Against.NullOrWhiteSpace(message, nameof(message));

            Category = category;
            Code = code;
            Message = message;
        }

        public override string ToString()
        {
            return $"{Category}{Code:000}: {Message}";
        }
    }
}