using Ardalis.GuardClauses;

namespace Wgaffa.DMToolkit.Expressions
{
    public class RepeatExpression : UnaryExpression
    {
        public int RepeatTimes { get; }

        public RepeatExpression(IExpression expressionToRepeat, int repeatTimes = 1)
            : base(expressionToRepeat)
        {
            Guard.Against.NegativeOrZero(repeatTimes, nameof(repeatTimes));

            RepeatTimes = repeatTimes;
        }

        public override string ToString()
        {
            return $"{RepeatTimes}x({Right})";
        }
    }
}
