using Ardalis.GuardClauses;
using System.Collections.Generic;
using Wgaffa.Functional;

namespace Wgaffa.DMToolkit.Interpreters
{
    public enum RecordType
    {
        Program
    }

    public class ActivationRecord
    {
        private readonly Dictionary<string, object> _members = new Dictionary<string, object>();

        public string Name { get; }
        public RecordType Type { get; }
        public int NestingLevel { get; }

        public object this[string identifier]
        {
            get => _members[identifier];
            set => _members[identifier] = value;
        }

        public ActivationRecord(string name, RecordType type, int nestingLevel)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));

            Name = name;
            Type = type;
            NestingLevel = nestingLevel;
        }

        public Maybe<object> Find(string identifier) =>
            _members.TryGetValue(identifier, out var obj)
            ? obj
            : None.Value;
    }
}