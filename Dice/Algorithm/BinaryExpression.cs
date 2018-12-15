using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Die.Algorithm
{
    public abstract class BinaryExpression : UnaryExpression
    {
        public BinaryExpression(IDiceExpression left, IDiceExpression right) : base(right)
        {
            _left = left;
        }

        public static IDiceExpression MakeBinary(OperatorType op, IDiceExpression left, IDiceExpression right)
        {
            IDiceExpression result = null;

            switch (op)
            {
                case OperatorType.Addition:
                    result = new Adder(left, right);
                    break;
                case OperatorType.Subtraction:
                    result = new Subtracter(left, right);
                    break;
                case OperatorType.Multiplication:
                    result = new Multiplier(left, right);
                    break;
                case OperatorType.Division:
                    result = new Divider(left, right);
                    break;
                default:
                    throw new ArgumentException("operator type not supported", "op");
            }

            return result;
        }

        protected IDiceExpression _left;
    }
}
