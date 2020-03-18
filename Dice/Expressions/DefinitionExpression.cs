using Ardalis.GuardClauses;

namespace Wgaffa.DMToolkit.Expressions
{
    public class DefinitionExpression : IExpression
    {
        public string Name { get; }
        public IExpression Expression { get; }

        public DefinitionExpression(string name, IExpression expression)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Guard.Against.Null(expression, nameof(expression));

            Name = name;
            Expression = expression;
        }
    }
}