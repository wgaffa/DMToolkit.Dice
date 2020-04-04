using Superpower;
using Superpower.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.DMToolkit.Statements;
using Wgaffa.DMToolkit.Types;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Parser
{
    public static class DiceNotationParser
    {
        private enum OperatorType
        {
            Addition,
            Subtraction,
            Multiplication,
            Division,
        }

        private static readonly TextParser<IExpression> DiceParser =
            from rolls in Numerics.IntegerInt32.OptionalOrDefault(1)
            from _ in Character.EqualTo('d')
            from sides in Numerics.IntegerInt32.Or(Character.EqualTo('%').Value(100))
            select (IExpression)new DiceRoll(new Dice(sides), rolls);

        private static readonly TextParser<int> RepeatParser =
            from repeat in Numerics.IntegerInt32
            from _ in Character.EqualTo('x')
            select repeat;

        private static readonly TextParser<int> KeepCount =
            from keepIdentifier in Character.EqualTo('k')
            from count in Numerics.IntegerInt32
            select count;

        private static readonly TokenListParser<DiceNotationToken, OperatorType> Addition =
            Token.EqualTo(DiceNotationToken.Plus).Value(OperatorType.Addition);

        private static readonly TokenListParser<DiceNotationToken, OperatorType> Subtraction =
            Token.EqualTo(DiceNotationToken.Minus).Value(OperatorType.Subtraction);

        private static readonly TokenListParser<DiceNotationToken, OperatorType> Multiplication =
            Token.EqualTo(DiceNotationToken.Multiplication).Value(OperatorType.Multiplication);

        private static readonly TokenListParser<DiceNotationToken, OperatorType> Division =
            Token.EqualTo(DiceNotationToken.Divide).Value(OperatorType.Division);

        private static readonly TokenListParser<DiceNotationToken, DropType> DropLowest =
            Token.EqualToValue(DiceNotationToken.Identifier, "L").Value(DropType.Lowest);

        private static readonly TokenListParser<DiceNotationToken, DropType> DropHighest =
            Token.EqualToValue(DiceNotationToken.Identifier, "H").Value(DropType.Highest);

        private static readonly TokenListParser<DiceNotationToken, string> Variable =
            from identifier in Token.EqualTo(DiceNotationToken.Identifier)
            select identifier.ToStringValue();

        private static readonly TokenListParser<DiceNotationToken, IExpression> String =
            from str in Token.EqualTo(DiceNotationToken.String)
            select (IExpression)new Expressions.String(str.ToStringValue()[1..]);

        private static readonly TokenListParser<DiceNotationToken, IExpression> Reference =
            from name in Variable
            select (IExpression)new Variable(name);

        private static readonly TokenListParser<DiceNotationToken, IExpression> Dice =
            Token.EqualTo(DiceNotationToken.Dice)
            .Apply(DiceParser);

        private static TokenListParser<DiceNotationToken, IExpression> Keep(IExpression expr) =>
            from lparen in Token.EqualTo(DiceNotationToken.LParen)
            from count in Token.EqualTo(DiceNotationToken.Identifier).Apply(KeepCount)
            from rparen in Token.EqualTo(DiceNotationToken.RParen)
            select (IExpression)new Keep(expr, count);

        private static TokenListParser<DiceNotationToken, IExpression> Drop(IExpression expr) =>
            from minus in Token.EqualTo(DiceNotationToken.Minus)
            from drop in DropHighest.Or(DropLowest)
            select (IExpression)new Drop(expr, drop);

        private static readonly TokenListParser<DiceNotationToken, IExpression> DiceNotation =
            Dice.Then(d => Keep(d).Or(Drop(d).Try()).OptionalOrDefault(d));

        private static readonly TokenListParser<DiceNotationToken, IExpression> Number =
            Token.EqualTo(DiceNotationToken.Number)
            .Apply(Numerics.DecimalDouble)
            .Select(n => (IExpression)new Literal(n));

        private static readonly TokenListParser<DiceNotationToken, IExpression> Constant =
            DiceNotation.Or(Reference).Or(Number).Or(String);

        private static readonly TokenListParser<DiceNotationToken, IExpression> Factor =
            (from lparen in Token.EqualTo(DiceNotationToken.LParen)
             from expr in Parse.Ref(() => Expr)
             from rparen in Token.EqualTo(DiceNotationToken.RParen)
             select expr)
            .Or(Constant);

        private static readonly TokenListParser<DiceNotationToken, IExpression> List =
            (from lbracket in Token.EqualTo(DiceNotationToken.LBracket)
             from expr in Parse.Ref(() => Expr).AtLeastOnceDelimitedBy(Token.EqualTo(DiceNotationToken.Comma))
             from rbracket in Token.EqualTo(DiceNotationToken.RBracket)
             select (IExpression)new ListExpression(expr))
            .Named("list");

        private static readonly TokenListParser<DiceNotationToken, IExpression> Repeat =
            Token.EqualTo(DiceNotationToken.Repeat)
            .Apply(RepeatParser)
            .Then(x => List.Or(Factor).Select(f => (IExpression)new Repeat(f, x)))
            .Named("repeat");

        private static readonly TokenListParser<DiceNotationToken, IExpression> FunctionCall =
            from identifier in Token.EqualTo(DiceNotationToken.Identifier).Apply(Identifier.CStyle)
            from lparen in Token.EqualTo(DiceNotationToken.LParen)
            from expr in Expr.ManyDelimitedBy(Token.EqualTo(DiceNotationToken.Comma))
            from rparen in Token.EqualTo(DiceNotationToken.RParen)
            select (IExpression)new FunctionCall(identifier.ToStringValue(), expr);

        private static readonly TokenListParser<DiceNotationToken, IExpression> Operand =
            (
            from _ in Token.EqualTo(DiceNotationToken.Minus)
            from factor in Factor
            select (IExpression)new Negate(factor)
            )
            .Or(List)
            .Or(Repeat)
            .Or(FunctionCall.Try())
            .Or(Factor)
            .Named("expression");

        private static readonly TokenListParser<DiceNotationToken, IExpression> Term =
            Parse.Chain(Multiplication.Or(Division), Operand, MakeBinary);

        private static readonly TokenListParser<DiceNotationToken, IExpression> Expr =
            Parse.Chain(Addition.Or(Subtraction), Term, MakeBinary);

        private static readonly TokenListParser<DiceNotationToken, IExpression> Assign =
            from name in Variable
            from equal in Token.EqualTo(DiceNotationToken.Equal)
            from right in Expr
            select (IExpression)new Assignment(name, right);

        private static readonly TokenListParser<DiceNotationToken, IExpression> Expression =
            Assign.Try().Or(Expr);

        private static readonly TokenListParser<DiceNotationToken, IStatement> Definition =
            from def in Token.EqualToValue(DiceNotationToken.Keyword, "def")
            from name in Token.EqualTo(DiceNotationToken.Identifier)
            from eq in Token.EqualTo(DiceNotationToken.Equal)
            from expr in Expr
            from semi in Token.EqualTo(DiceNotationToken.SemiColon)
            select (IStatement)new Definition(name.ToStringValue(), expr);

        private static readonly TokenListParser<DiceNotationToken, KeyValuePair<string, string>[]> Params =
            (from type in Token.EqualTo(DiceNotationToken.Identifier)
             from name in Token.EqualTo(DiceNotationToken.Identifier)
             select new KeyValuePair<string, string>(type.ToStringValue(), name.ToStringValue()))
            .ManyDelimitedBy(Token.EqualTo(DiceNotationToken.Comma));

        private static readonly TokenListParser<DiceNotationToken, IStatement> FuncDecl =
            from type in Token.EqualTo(DiceNotationToken.Identifier)
            from name in Token.EqualTo(DiceNotationToken.Identifier)
            from lparen in Token.EqualTo(DiceNotationToken.LParen)
            from par in Params
            from rparen in Token.EqualTo(DiceNotationToken.RParen)
            from body in Parse.Ref(() => Block)
            select (IStatement)new Function(name.ToStringValue(), body, type.ToStringValue(), par);

        private static readonly TokenListParser<DiceNotationToken, (string[] names, string type)> VariableDeclaration =
            from type in Token.EqualTo(DiceNotationToken.Identifier)
            from names in Variable.AtLeastOnceDelimitedBy(Token.EqualTo(DiceNotationToken.Comma))
            select (names, type.ToStringValue());

        private static readonly TokenListParser<DiceNotationToken, Maybe<IExpression>> VariableInitializer =
            from eq in Token.EqualTo(DiceNotationToken.Equal)
            from value in Expr
            select (Maybe<IExpression>)Maybe<IExpression>.Some(value);

        private static readonly TokenListParser<DiceNotationToken, IStatement> VarDecl =
            from decl in VariableDeclaration
            from init in VariableInitializer.OptionalOrDefault(None.Value)
            from semi in Token.EqualTo(DiceNotationToken.SemiColon)
            select (IStatement)new VariableDeclaration(decl.names, decl.type, init);

        private static readonly TokenListParser<DiceNotationToken, IStatement> ExpressionStatement =
            from expr in Expression
            from semi in Token.EqualTo(DiceNotationToken.SemiColon)
            select (IStatement)new ExpressionStatement(expr);

        private static readonly TokenListParser<DiceNotationToken, IStatement> ReturnStatement =
            from ret in Token.EqualToValue(DiceNotationToken.Keyword, "return")
            from expr in Expression.OptionalOrDefault(new Literal(Unit.Value))
            from semi in Token.EqualTo(DiceNotationToken.SemiColon)
            select (IStatement)new Return(expr);

        private static readonly TokenListParser<DiceNotationToken, IStatement> Statement =
            ExpressionStatement
            .Or(ReturnStatement)
            .Named("statement");

        private static readonly TokenListParser<DiceNotationToken, IStatement> Declaration =
            FuncDecl.Try()
            .Or(VarDecl.Try())
            .Or(Definition)
            .Or(Statement);

        private static readonly TokenListParser<DiceNotationToken, IStatement> Block =
            from stmts in Declaration.Many()
            from end in Token.EqualToValue(DiceNotationToken.Keyword, "end")
            select stmts.Length == 1 ? stmts[0] : new Block(stmts);

        private static IExpression MakeBinary(OperatorType @operator, IExpression left, IExpression right)
        {
            return @operator switch
            {
                OperatorType.Addition => new Addition(left, right),
                OperatorType.Subtraction => new Subtraction(left, right),
                OperatorType.Multiplication => new Multiplication(left, right),
                OperatorType.Division => new Division(left, right),
                _ => throw new InvalidOperationException(),
            };
        }

        public static TokenListParser<DiceNotationToken, IStatement> Program =
            (from declarations in Declaration.Many()
            select declarations.Length == 1 ? declarations[0] : new Block(declarations))
            .AtEnd();
    }
}