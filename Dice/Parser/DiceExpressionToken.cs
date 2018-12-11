using Superpower.Display;

namespace DMTools.Die.Parser
{
    public enum DiceToken
    {
        Undefined,

        [Token(Category = "dice")]
        Dice,

        Number,

        [Token(Category = "operator", Example = "+")]
        Plus,

        [Token(Category = "operator", Example = "-")]
        Minus,

        [Token(Category = "operator", Example = "*")]
        Multiply,

        [Token(Category = "operator", Example = "/")]
        Divide
    }
}