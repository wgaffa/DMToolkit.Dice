using Ardalis.GuardClauses;
using Wgaffa.DMToolkit.Parser;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Expressions
{
    public class DefinitionExpression : IExpression
    {
        public string Name { get; }
        public IExpression Expression { get; }
        internal Symbol Symbol { get; set; }

        public DefinitionExpression(string name, IExpression expression)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Guard.Against.Null(expression, nameof(expression));

            Name = name;
            Expression = expression;
        }

        public override string ToString()
        {
            return $"<decl_def: var={Name} value={Expression}>";
        }
    }
}