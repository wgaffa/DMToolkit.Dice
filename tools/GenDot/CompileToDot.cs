using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wgaffa.DMToolkit.Expressions;
using Wgaffa.DMToolkit.Extensions;
using Wgaffa.DMToolkit.Statements;

namespace GenDot
{
    public class CompileToDot
    {
        private readonly string _name;
        private int _counter = 1;
        private readonly StringBuilder _body = new StringBuilder();

        public CompileToDot(string name)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));

            _name = name;
        }

        public string Evaluate(IExpression expression)
        {
            var header = $"digraph {_name} {{\n";
            const string footer = "}";

            int program = Visit((dynamic)expression);

            return header + _body.ToString() + footer;
        }

        private int Visit(IExpression expression)
        {
            _body.Append(Node(_counter, expression, "default"));
            return _counter++;
        }

        private int Visit(Block compound)
        {
            int id = _counter++;
            _body.Append(Node(id, compound));

            var ids = compound.Body.Select(expr => (int)Visit((dynamic)expr));

            foreach (var item in ids)
            {
                _body.Append(Link(id, item));
            }

            return id;
        }

        private int Visit(VariableDeclaration vardecl)
        {
            int id = _counter++;
            _body.Append(Node(id, vardecl, string.Join(", ", vardecl.Names)));

            vardecl.InitialValue
                .Map(expr => (int)Visit((dynamic)expr))
                .Do(x => _body.Append(Link(id, x)));

            return id;
        }

        private int Visit(Function function)
        {
            int id = _counter++;
            _body.Append(Node(id, function, function.Identifier));

            int bodyId = Visit((dynamic)function.Body);

            _body.Append(Link(id, bodyId));

            return id;
        }

        private int Visit(FunctionCall functionCall)
        {
            int id = _counter++;
            _body.Append(Node(id, functionCall, functionCall.Name));

            var ids = functionCall.Arguments.Select(expr => (int)Visit((dynamic)expr));

            foreach (var item in ids)
            {
                _body.Append(Link(id, item));
            }

            return id;
        }

        private int Visit(Definition definition)
        {
            int id = _counter++;
            _body.Append(Node(id, definition, definition.Name));

            int right = Visit((dynamic)definition.Expression);

            _body.Append(Link(id, right));

            return id;
        }

        private int Visit(Variable variable)
        {
            _body.Append(Node(_counter, variable, variable.Identifier));

            return _counter++;
        }

        private int Visit(Assignment assignment)
        {
            int id = _counter++;
            _body.Append(Node(id, assignment, assignment.Identifier));

            int expr = Visit((dynamic)assignment.Expression);

            _body.Append(Link(id, expr));

            return id;
        }

        private int Visit(Number number)
        {
            _body.Append(Node(_counter, number, number.Value.ToString()));
            return _counter++;
        }

        private int Visit(BinaryExpression binary)
        {
            int id = _counter++;
            _body.Append(Node(id, binary));

            int left = Visit((dynamic)binary.Left);
            int right = Visit((dynamic)binary.Right);

            _body.Append(Link(id, left));
            _body.Append(Link(id, right));

            return id;
        }

        private int Visit(UnaryExpression unary)
        {
            int id = _counter++;
            _body.Append(Node(id, unary));

            int right = Visit((dynamic)unary.Right);

            _body.Append(Link(id, right));

            return id;
        }

        private static string GetName(IExpression expression)
        {
            return expression.GetType().Name;
        }

        private static string Label(IExpression expression) =>
            $"[label=\"{GetName(expression)}\"]";

        private static string Label(IExpression expression, string meta) =>
            $"[label=\"{GetName(expression)}\\n{meta}\"]";

        private static string Node(int id, IExpression expression) =>
            $"\tnode{id} {Label(expression)}\n";

        private static string Node(int id, IExpression expression, string meta) =>
            $"\tnode{id} {Label(expression, meta)}\n";

        private static string Link(int from, int to) =>
            $"\tnode{from} -> node{to}\n";
    }
}
