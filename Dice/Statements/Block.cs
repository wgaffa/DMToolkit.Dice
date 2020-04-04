using Ardalis.GuardClauses;
using System.Collections.Generic;
using System.Linq;

namespace Wgaffa.DMToolkit.Statements
{
    public class Block : IStatement
    {
        private readonly List<IStatement> _statements;

        public IReadOnlyList<IStatement> Body => _statements.AsReadOnly();

        public Block(IEnumerable<IStatement> statements)
        {
            Guard.Against.Null(statements, nameof(statements));

            _statements = statements.ToList();
        }

        public override string ToString()
        {
            return $"<block: {string.Join(' ', _statements)}>";
        }
    }
}