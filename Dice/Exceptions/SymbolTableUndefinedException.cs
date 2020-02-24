using System;
using System.Runtime.Serialization;

namespace Wgaffa.DMToolkit.Exceptions
{
    [Serializable]
    public class SymbolTableUndefinedException : Exception
    {
        public SymbolTableUndefinedException() : base()
        {
        }

        public SymbolTableUndefinedException(string message) : base(message)
        {
        }

        public SymbolTableUndefinedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SymbolTableUndefinedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
