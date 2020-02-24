using System;
using System.Runtime.Serialization;

namespace Wgaffa.DMToolkit.Exceptions
{

    [Serializable]
    public class VariableNotDefinedException : Exception
    {
        public string Variable { get; } = string.Empty;

        public VariableNotDefinedException(string variable)
        {
            Variable = variable;
        }

        public VariableNotDefinedException(string variable, string message) : base(message)
        {
            Variable = variable;
        }

        public VariableNotDefinedException(string variable, string message, Exception inner) : base(message, inner)
        {
            Variable = variable;
        }

        protected VariableNotDefinedException(
          SerializationInfo info,
          StreamingContext context) : base(info, context)
        {
            if (info != null)
            {
                Variable = info.GetString("Variable");
            }
        }

        public VariableNotDefinedException() : base()
        {
        }

        public VariableNotDefinedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info == null)
                return;

            info.AddValue("Variable", Variable);
        }
    }
}
