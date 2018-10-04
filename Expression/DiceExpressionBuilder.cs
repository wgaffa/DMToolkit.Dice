using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Dice.Expression
{
    public class DiceExpressionBuilder
    {
        public DiceExpressionBuilder()
        {
            _expression = new Constant(0);
        }

        public DiceExpressionBuilder Addition(IDiceExpression expression)
        {
            _expression = new BinaryOperation((x, y) => x + y, _expression, expression);
            return this;
        }

        public DiceExpressionBuilder Subtract(IDiceExpression expression)
        {
            _expression = new BinaryOperation((x, y) => x - y, _expression, expression);
            return this;
        }

        public int Evaluate()
        {
            return _expression.Evaluate();
        }

        private IDiceExpression _expression;
    }
}
