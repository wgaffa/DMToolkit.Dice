using Ardalis.GuardClauses;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.DMToolkit.Parser;

namespace Wgaffa.DMToolkit.Statements
{
    public class Definition : IStatement
    {
        public string Name { get; }
        public IExpression Expression { get; }
        internal Symbol Symbol { get; set; }

        public Definition(string name, IExpression expression)
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