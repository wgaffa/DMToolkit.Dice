using Ardalis.GuardClauses;

namespace Wgaffa.DMToolkit.Expressions
{
    public class Repeat : UnaryExpression
    {
        public int RepeatTimes { get; }

        public Repeat(IExpression expressionToRepeat, int repeatTimes = 1)
            : base(expressionToRepeat)
        {
            Guard.Against.NegativeOrZero(repeatTimes, nameof(repeatTimes));

            RepeatTimes = repeatTimes;
        }

        public override string ToString()
        {
            return $"<repeat: {RepeatTimes} {Right}>";
        }
    }
}
