using Ardalis.GuardClauses;
using System.Collections.Generic;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Interpreters
{
    public enum RecordType
    {
        Program,
        Function,
        Definition,
    }

    public class ActivationRecord
    {
        private readonly Dictionary<string, object> _members = new Dictionary<string, object>();

        public string Name { get; }
        public RecordType Type { get; }
        public int NestingLevel { get; }
        public Maybe<ActivationRecord> AccessLink { get; internal set; } = None.Value;
        public Maybe<ActivationRecord> ControlLink { get; internal set; } = None.Value;

        public object this[string identifier]
        {
            get => _members[identifier];
            set => _members[identifier] = value;
        }

        public ActivationRecord(string name, RecordType type, int nestingLevel, Maybe<ActivationRecord> callee)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Guard.Against.Null(callee, nameof(callee));

            Name = name;
            Type = type;
            NestingLevel = nestingLevel;
            AccessLink = callee;
        }

        public ActivationRecord(string name, RecordType type, int nestingLevel)
            : this(name, type, nestingLevel, None.Value)
        {
        }

        public Maybe<object> Find(string identifier) =>
            _members.TryGetValue(identifier, out var obj)
            ? Maybe<object>.Some(obj)
            : (Maybe<object>)Maybe<object>.None();

        public Maybe<ActivationRecord> Follow(int count)
        {
            Guard.Against.Negative(count, nameof(count));

            if (count == 0)
                return this;

            return AccessLink.Bind(x => x.Follow(count - 1));
        }
    }
}