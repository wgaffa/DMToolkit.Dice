using Superpower.Display;

namespace DMTools.Dice.Parser
{
    public enum DiceToken
    {
        Undefined,

        //Dice,

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