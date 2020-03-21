using Ardalis.GuardClauses;

namespace Wgaffa.DMToolkit.Interpreters.Errors
{
    public sealed class ErrorCategory
    {
        public static readonly ErrorCategory Variable = new ErrorCategory("VAR");

        public string Name { get; }

        private ErrorCategory(string name)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));

            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}