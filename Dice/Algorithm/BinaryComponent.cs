using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTools.Core.Algorithm
{
    public abstract class BinaryComponent : UnaryComponent
    {
        public BinaryComponent(IComponent left, IComponent right) : base(right)
        {
            _left = left;
        }

        public static IComponent MakeBinary(OperatorType op, IComponent left, IComponent right)
        {
            IComponent result = null;

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

        protected IComponent _left;
    }
}
