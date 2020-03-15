using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Wgaffa.DMToolkit
{
    public class RollResult
    {
        private readonly List<int> _keep;
        private readonly List<int> _discard;

        public IReadOnlyList<int> Keep => _keep.AsReadOnly();
        public IReadOnlyList<int> Discard => _discard.AsReadOnly();

        public RollResult(IEnumerable<int> keep)
            : this(keep, Array.Empty<int>())
        {
        }

        public RollResult(IEnumerable<int> keep, IEnumerable<int> discard)
        {
            Guard.Against.Null(keep, nameof(keep));
            Guard.Against.Null(discard, nameof(discard));

            _keep = keep.ToList();
            _discard = discard.ToList();
        }
    }
}
