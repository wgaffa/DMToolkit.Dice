using System.Text;

namespace Wgaffa.DMToolkit.Expressions
{
    public class AdditionExpression : BinaryExpression
    {
        public AdditionExpression(IExpression left, IExpression right) : base(left, right)
        {
        }
    }
}
